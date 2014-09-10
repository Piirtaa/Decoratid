using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Store;
using Sandbox.Store.CoreStores;
using Sandbox.Store.Decorations.StoreOf;
using Sandbox.Extensions;

namespace Sandbox.Thingness.Fluency
{
    /*
     *  new FluentAction(ConditionalData data, Action<ConditionalData> behaviour)
     *  .ThenDo(ConditionalData data, Action<ConditionalData> behaviour)
     *  .ThenDo(FluentAction)
     *  .ThenDo(Action<ConditionalData> behaviour) - uses the most recent conditional data
     *  
     * 
     * so Fluent Action has
     *  -ConditionalData
     *  -Action<ConditionalData>
     *  -background polling process examining conditional data
     *      
     *  -method called ThenDo that takes a FluentAction and returns the fluent action being taken
     *      -this adds fluentaction to a store of fluentactions that each fluent action keeps around 
     *          (eg. a context of the bag of actions)
     * 
     *  -calling Do will begin a timedout (or not) action, which itself will polling the conditional data, and when that
     *      is ready, we beging.  and continue in this way
     *      
     *  -the results of each completed action (eg. that it was completed are also kept in the store of actions)
     *      -we keep 1 store - the action stack
     *      
     *          index | Action or Function | Condition | status | error | result
     * 
     * 
     */

    #region State Graph Enums
    public enum FluentItemStatusEnum
    {
        Pending,
        Complete,
        Error
    }

    public enum FluentItemStateTransitionEnum
    {
        MarkComplete,
        MarkError
    }
    #endregion

    /// <summary>
    /// an activity that happens upon receipt of conditional data
    /// </summary>
    public interface IFluentActivity : IHasId<int>
    {
        /// <summary>
        /// the conditional data
        /// </summary>
        ConditionalDataOf<object> ConditionalData { get; set; }
        /// <summary>
        /// the data that comes from ConditionalData
        /// </summary>
        object Data { get; set; }

        FluentItemStatusEnum Status { get; }

        /// <summary>
        /// state transition 
        /// </summary>
        void MarkComplete();
        /// <summary>
        /// state transition
        /// </summary>
        /// <param name="ex"></param>
        void MarkError(Exception ex);
        /// <summary>
        /// the error set by markerror
        /// </summary>
        Exception Error { get; }

        void Do();

        /// <summary>
        /// method to set the ordinal of the activity
        /// </summary>
        /// <param name="id"></param>
        void SetId(int id);

        /// <summary>
        /// fires when an activit is finished
        /// </summary>
        event EventHandler<EventArgOf<IFluentAction>> ActivityFinished;
    }
    /// <summary>
    /// a fluent activity that uses an action delegate
    /// </summary>
    public interface IFluentAction : IFluentActivity
    {
        Action<object> Action { get; }
    }
    /// <summary>
    /// a fluent activit that uses a function delegate
    /// </summary>
    public interface IFluentFunction : IFluentActivity
    {
        object Result { get; set; }
        Func<object, object> Function { get; }
    }

    public abstract class FluentActivityBase : DisposableBase, IFluentActivity
    {
        #region Declarations
        protected readonly object _stateLock = new object();
        protected StateMachineGraph<FluentItemStatusEnum, FluentItemStateTransitionEnum> _stateMachine = null;
        #endregion

        #region Ctor
        public FluentActivityBase()
            : base()
        {
            //define the graph
            _stateMachine = new StateMachineGraph<FluentItemStatusEnum, FluentItemStateTransitionEnum>(FluentItemStatusEnum.Pending);
            _stateMachine.AllowTransition(FluentItemStatusEnum.Pending, FluentItemStatusEnum.Complete, FluentItemStateTransitionEnum.MarkComplete);
            _stateMachine.AllowTransition(FluentItemStatusEnum.Pending, FluentItemStatusEnum.Error, FluentItemStateTransitionEnum.MarkError);

            this.Store = EncodedInMemoryStore.New().WithIsOfUniqueId<IFluentActivity>();
            this.Id = 0;
            this.Store.SaveItem(this);
        }
        #endregion

        #region IHasId
        public int Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public ConditionalDataOf<object> ConditionalData { get; set; }
        public FluentItemStatusEnum Status { get { return this._stateMachine.CurrentState; } }
        public Exception Error { get; set; }
        public IStoreOfUniqueId<IFluentActivity> Store { get; private set; }
        private BackgroundBase Background { get; set; }
        public event EventHandler<EventArgOf<IFluentActivity>> ActivityFinished;
        #endregion

        #region Methods
        public void SetId(int id) { this.Id = id; }
        /// <summary>
        /// begins the activity chain
        /// </summary>
        public void Do()
        {
            this.Background = new BackgroundBase(true, 5000, this.CheckConditions);

            this.ActivityFinished += FluentActivityBase_ActivityFinished;
        }

        void FluentActivityBase_ActivityFinished(object sender, EventArgOf<IFluentActivity> e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Fluent Methods
        public FluentActivityBase ThenDo(IFluentActivity activity)
        {
            Condition.Requires(activity).IsNotNull();

            lock (this._stateLock)
            {
                int nextId = this.GetActivities().Last().Id + 1;
                activity.SetId(nextId);

                this.Store.SaveItem(activity);
            }
            return this;
        }
        #endregion

        #region Events
        public void OnActivityFinished()
        {
            //skip if no listeners attached
            if (this.ActivityFinished == null)
                return;

            this.ActivityFinished.BuildAndFireEventArgs(this);
        }
        #endregion

        #region Helper Methods
        private List<IFluentActivity> GetActivities()
        {
            var list = this.Store.GetAll<IFluentActivity>();
            list = list.OrderBy(x => x.Id).ToList();
            return list;
        }

        private void CheckConditions()
        {
            lock (this._stateLock)
            {
                var activities = this.GetActivities();

                foreach (var each in activities)
                {
                    bool exit = false;
                    switch (each.Status)
                    {
                        case FluentItemStatusEnum.Pending:
                            //if the condition is triggered, get the data
                            if (each.ConditionalData.CheckCondition.Evaluate().GetValueOrDefault())
                            {
                                //hey! we're triggered, run the current activity
                                try
                                {
                                    each.Data = each.ConditionalData.GetData();
                                    if (each is IFluentAction)
                                    {
                                        IFluentAction fluentAction = each as IFluentAction;
                                        fluentAction.Action(each.Data);
                                        each.MarkComplete();
                                    }
                                    else if (each is IFluentFunction)
                                    {
                                        IFluentFunction fluentFunction = each as IFluentFunction;
                                        fluentFunction.Result = fluentFunction.Function(each.Data);
                                        each.MarkComplete();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    each.MarkError(ex);
                                    exit = true;
                                }
                                //save it
                                this.Store.SaveItem(each);
                            }
                            else
                            {
                                exit = true;
                            }
                            break;
                        case FluentItemStatusEnum.Error:
                            exit = true;
                            break;
                        case FluentItemStatusEnum.Complete:
                            continue;
                    }

                    if (exit)
                        break;
                }

                //check to see if it's complete (all are complete) 
                if (this.IsActivityChainComplete() || this.IsActivityChainError())
                {
                    //if we have any error, stop the background and exit
                    this.Background.IsEnabled = false;

                    this.OnActivityFinished(); //raise the finished event
                }
            }
        }
        private bool IsActivityChainComplete()
        {
            bool returnValue = true;

            var activities = this.GetActivities();

            foreach (var each in activities)
            {
                if (each.Status != FluentItemStatusEnum.Complete)
                {
                    returnValue = false;
                    break;
                }
            }
            return returnValue;
        }
        private bool IsActivityChainError()
        {
            bool returnValue = false;

            var activities = this.GetActivities();

            foreach (var each in activities)
            {
                if (each.Status == FluentItemStatusEnum.Error)
                {
                    returnValue = true;
                    break;
                }
            }
            return returnValue;
        }
        #endregion

        #region Overrides
        protected override void DisposeManaged()
        {
            if (this.Background != null)
            {
                this.Background.IsEnabled = false;
                this.Background.Dispose();
            }
        }
        #endregion
    }

    public class FluentAction : FluentActivityBase, IFluentAction
    {
        #region Ctor
        public FluentAction(Action<object> action)
            : base()
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
        }
        #endregion

        #region Properties
        public Action<object> Action { get; private set; }
        #endregion

        #region Overrides

        #endregion
    }

    public class FluentFunction : FluentActivityBase, IFluentFunction
    {
        #region Ctor
        public FluentFunction()
            : base()
        {

        }
        #endregion

        #region Properties
        object Result { get; set; }
        Func<object, object> Function { get; }
        #endregion
    }
}
