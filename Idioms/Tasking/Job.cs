using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Expiring;
using Decoratid.Idioms.Tasking.Decorations;
using Decoratid.Storidioms.Evicting;
using System;
using System.Threading;

namespace Decoratid.Idioms.Tasking
{
    /// <summary>
    /// An asynchronous task that itself is a collection of tasks.  Is the "managing task" of all the tasks it has.
    /// </summary>
    /// <remarks>
    /// Designed so that any jobs instantiated with a reference to the same job store will be able to contribute to
    /// the processing of the tasks.  In this way we can just add instances to scale out.  However, I imagine this should
    /// only be done when you're dealing with long complicated jobs where you might want to take advantage of parallelization.
    /// </remarks>
    public class Job : DecoratedTaskBase
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// provide store (with defined eviction strategies)
        /// </summary>
        /// <param name="store"></param>
        public Job(string id, LogicOfTo<IHasId, IExpirable> evictionPolicy)
            : base(StrategizedTask.NewBlank(id))
        {
            /*Overall process:
             * -create a managing task (this guy) to manage and store all of the subtasks.
             *  -the core of this managing task is a blank strategized core which is decorated
             *  with a bunch of behaviours that help accomplish the managing task's purpose.
             *  
             * The layers are described below:
             * -Core
             *      -PerformLogic - on perform, injects polling strategy checkTriggers() that moves the tasks along
             *      -CancelLogic - cancels all cancellable tasks
             * -Async
             *       -Complete trigger: When all tasks complete
             *       -Error trigger:When any tasks go wrong
             * -Events
             *       -OnCancelled, release waithandle
             *       -OnComplete, release waithandle
             *       -OnError, release waithandle
             * -Polling 
             *       -run checkTriggers as injected above
             * 
             * The job task also contains the TaskStore which all contains all of the tasks to manage.
             * 
             * RunToCompletion() kicks the job off and waits until it is complete.
             */

            //create new in memory store for the tasks to live in
            var taskStore = NaturalInMemoryStore.New().MakeTaskStore(evictionPolicy);
            this.TaskStore = taskStore;

            //get the core task (is also the Decorated property as we're a 2-layer cake at this point)
            StrategizedTask coreTask = this.Core as StrategizedTask;

            //define the Perform and Cancel Logic
            coreTask.Performs( Logic.New(
               () =>
               {
                   //the perform task is to turn on the polling process
                   this.FindDecoratorOf<IPollingDecoration>(false)
                       .SetBackgroundAction(LogicOf<ITask>.New(
                           (x) =>
                           {
                               this.checkTriggers();
                           })
                       );
               }));
            coreTask.Cancels( Logic.New(() => { this.CancelTasks(); }));

            //define the action to flip the waithandle that is waited on in RunToCompletion 
            Action flipWaitHandle = () =>
            {
                lock (_stateLock)
                {
                    if (this.Status == DecoratidTaskStatusEnum.Cancelled ||
            this.Status == DecoratidTaskStatusEnum.Complete ||
            this.Status == DecoratidTaskStatusEnum.Errored)
                        Monitor.Pulse(_stateLock);
                }
            };

            //now decorate this core task with the layers described above

            //decorate as asynch (under the hood this will decorate with Trigger Conditions). Also implies that we need a polling process to check the conditions
            ITask decoratedTask = coreTask.IsAsynchronous(
                        StrategizedCondition.New(() => { return this.AreTasksComplete(); }),
                        StrategizedCondition.New(() => { return this.HaveTasksErrored(); }));
            //decorate with event handlers to flip the wait handle
            decoratedTask = decoratedTask.Eventing().DoOnTaskCancelled(flipWaitHandle).DoOnTaskCompleted(flipWaitHandle).DoOnTaskErrored(flipWaitHandle);
            //decorate with polling without a polling strategy
            decoratedTask = decoratedTask.DecorateWithPolling();

            //set the task store
            decoratedTask.TaskStore = this.TaskStore;
            decoratedTask.Save();

            //inject decoration that we've just built 
            this.ReplaceDecorated((decorated) =>
            {
                return decoratedTask;
            });

            //subscribe to eviction event where we initiate a cancel if it happens
            this.TaskStore.ItemEvicted += Job_ItemEvicted;
        }
        #endregion

        #region Async Hooks, Event Handlers
        /// <summary>
        /// if any item is evicted, it triggers a cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Job_ItemEvicted(object sender, EventArgOf<Tuple<IHasId, IExpirable>> e)
        {
            this.CancelTask();
        }
        #endregion

        #region Job Task Strategies
        private bool HaveTasksErrored()
        {
            //if there are any error or cancelled tasks, mark error is triggered
            var list = this.TaskStore.Search<ITask>(new SearchFilterOf<ITask>((x) =>
            {
                return (x.Status == DecoratidTaskStatusEnum.Errored || x.Status == DecoratidTaskStatusEnum.Cancelled);
            }));

            return list.Count > 0;
        }
        private bool AreTasksComplete()
        {
            bool returnValue = true;

            //if every job except this one has succeeded then we're complete
            var list = this.TaskStore.GetAll<ITask>();

            foreach (var each in list)
            {
                if (each.Status != DecoratidTaskStatusEnum.Complete)
                {
                    returnValue = false;
                    break;
                }
            }
            return returnValue;
        }
        private bool CancelTasks()
        {
            //get from most dependent to least, and unwrap the onion of dependencies
            this.TaskStore.GetAll<ITask>().WithEach
            (task =>
            {
                if (task.Status == DecoratidTaskStatusEnum.InProcess || task.Status == DecoratidTaskStatusEnum.Pending)
                {
                    try
                    {
                        task.CancelTask();
                    }
                    catch (Exception ex)
                    {
                        task.MarkTaskError(new ApplicationException("Task cancellation error", ex));
                    }
                    finally
                    {
                        //commit work to store
                        task.Save();
                    }
                }
            });

            return true;
        }
        /// <summary>
        /// examines all pending/inprocess tasks for trigger conditions, and triggers them
        /// </summary>
        /// <returns></returns>
        private void checkTriggers()
        {
            //if job is not in progress, skip 
            if (this.Status != DecoratidTaskStatusEnum.InProcess)
                return;

            try
            {

                //get tasks from least to most dependent
                var list = this.TaskStore.GetAll<ITask>();

                //done yet?
                if (!(list.Exists(x => x.Status == DecoratidTaskStatusEnum.Pending || x.Status == DecoratidTaskStatusEnum.InProcess)))
                    return;

                #region Mark Complete Triggers
                list.WithEach(task =>
                {
                    if (task is IHasConditionalTaskTriggers)
                    {
                        IHasConditionalTaskTriggers cTask = task as IHasConditionalTaskTriggers;
                        cTask.CheckTriggers();
                    }
                    else
                    {
                        task.PerformTask();
                    }
                });
                #endregion
            }
            catch
            {
            }
        }
        #endregion

        #region Overrides
        public override IDecorationOf<ITask> ApplyThisDecorationTo(ITask thing)
        {
            Job rv = new Job(this.Id, this.TaskStore.ExpirableFactory);
            //copy the store over
            rv.TaskStore = this.TaskStore;
            return rv;
        }
        #endregion

        #region Run Methods
        public void RunToCompletion()
        {
            this.PerformTask();

            lock (_stateLock)
                while (this.Status != DecoratidTaskStatusEnum.Cancelled &&
                this.Status != DecoratidTaskStatusEnum.Complete &&
                this.Status != DecoratidTaskStatusEnum.Errored)
                    Monitor.Wait(_stateLock);

            //we're done


        }
        #endregion

        #region Fluent Methods

        public Job AddTask(ITask task)
        {
            Condition.Requires(task).IsNotNull();
            task.TaskStore = this.TaskStore;
            this.TaskStore.SaveItem(task);
            return this;
        }
        /// <summary>
        /// adds a task all other non-complete tasks depend their next transition on.
        /// </summary>
        /// <param name="interruptTask"></param>
        /// <returns></returns>
        public Job AddInterruptingTask(ITask interruptTask)
        {
            lock (this._stateLock)
            {
                //add the task (setting the id)
                this.AddTask(interruptTask);

                var tasks = this.TaskStore.GetAll<ITask>();
                tasks.WithEach(eachtask =>
                {
                    //wraplace the task
                    var wrappedTask = eachtask.DependsOn(interruptTask.Id);
                    wrappedTask.Save();
                });
            }
            return this;
        }

        public Job RemoveInterruptingTask(string interruptTaskId)
        {
            lock (this._stateLock)
            {
                var tasks = this.TaskStore.GetAll<ITask>();
                tasks.WithEach(eachtask =>
                {
                    //wraplace the task
                    var wrappedTask = eachtask.DoesNotDependOn(interruptTaskId);
                    wrappedTask.Save();
                });
            }
            return this;
        }
        #endregion

        #region Static Fluent Methods
        public static Job New(string id, LogicOfTo<IHasId, IExpirable> evictionPolicy)
        {
            return new Job(id, evictionPolicy);
        }
        public static Job NewWithNeverExpireDefault(string id)
        {
            return new Job(id, LogicOfTo<IHasId, IExpirable>.New((x) => { return EvictionPolicy.BuildNeverExpirable(); }));
        }


        #endregion
    }
}
