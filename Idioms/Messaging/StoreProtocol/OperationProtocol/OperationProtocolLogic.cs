using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations.StoreOf;
using Decoratid.Tasks;
using Decoratid.Tasks.Decorations;
using Decoratid.Idioms.Dependencies;
using Decoratid.Core.Logical;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Common;
using Decoratid.Tasks.Core;
using Decoratid.Thingness;
using Decoratid.Core.Storing;
using System.Runtime.Serialization;
using Decoratid.Core.Storing.Decorations;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Messaging.StoreProtocol.OperationProtocol
{

    /// <summary>
    /// an implementation of IStoreProtocolHandler that implements the Operation Protocol.  
    /// Contains a store of IOperations that could potentially be invoked if they are
    /// requested.
    /// </summary>
    /// <remarks>
    /// The Operation Protocol sits on top of the Store Protocol.
    /// The general idea is to conform requests 
    /// </remarks>
    [Serializable]
    public class OperationProtocolLogic : IStoreProtocolHandler, ISerializable
    {
        #region Ctor
        public OperationProtocolLogic() { }
        protected OperationProtocolLogic(IStoreOf<IOperation> operations)
        {
            this.Operations = operations;
        }
        #endregion

        #region ISerializable
        protected OperationProtocolLogic(SerializationInfo info, StreamingContext context)
        {
          //  StoreSerializationSurrogate.SetObjectDataHelper(this, info, context, null, null);
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           // StoreSerializationSurrogate.GetObjectDataHelper(this, info, context, null);
        }
        #endregion

        #region IHasId
        public string Id { get { return "OPERATION PROTOCOL HANDLER"; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public IStoreOf<IOperation> Operations { get; set; }
        #endregion

        #region Methods
        public void Handle(StoreProtocolUnitOfWork uow)
        {
            //create a job with a never evict policy (eg. the tasks in the job don't expire)
            JobTask job = JobTask.New("job", LogicOfTo<IHasId,ICondition>.New( (x) => { return AlwaysFalseCondition.New(); }) )as JobTask;

            //find which operations are requested
            List<IOperation> ops = new List<IOperation>();
            var allOps = this.Operations.GetAll();

            allOps.WithEach(op =>
            {
                //get the request if one exists
                var request = uow.RequestStore.Get<OperationProtocolRequestItem>(op.Id + "_Request");
                if (request != null)
                {
                    //get the operation logic, cloned.  we don't want multiple threads on the same logic, especially if the logic has state
                    var operationLogic = op.PerformLogic.Clone();

                    //the operation is a Operation<,>, just stored in the store as IOperation
                    //so we set the context

                    var genType = typeof(ValueOf<>).MakeGenericType(request.Data.GetType());
                    var valueOf = Activator.CreateInstance(genType, request.Data);

                    IHasContext hasContext = operationLogic as IHasContext;
                    hasContext.Context = valueOf;

                    //add a task of "Evaluate the Operation"
                    ITask task = StrategizedTask.New(op.Id, operationLogic, null);
 
                    //if the task has a dependency, set it     
                    if (op.Dependency.Prerequisites != null && op.Dependency.Prerequisites.Count > 0)
                    {
                        job.AddTask(task.DecorateWithDependency(op.Dependency.Prerequisites.ToArray()));
                    }
                    else
                    {
                        job.AddTask(task);
                    }
                }
            });

            //perform all the tasks.  
            job.RunToCompletion();

            //when the job is done we need to respond by writing response values to each function
            var tasks = job.TaskStore.GetAll<StrategizedTask>();
            tasks.WithEach(task =>
            {
                var op = ops.Find(x => x.Id.Equals(task.Id));
                var storedItem = new OperationProtocolResponseItem(op.Id, task);
                uow.ResponseStore.SaveItem(storedItem);
            });

        }
        #endregion

        #region Static Fluent Methods
        /// <summary>
        /// creates an instance of OperationProtocolLogic with the provided operations
        /// </summary>
        /// <param name="operations"></param>
        /// <returns></returns>
        public static OperationProtocolLogic New(params IOperation[] operations)
        {
            var store = InMemoryStore.New().DecorateWithIsOf<IOperation>();
            store.SaveItems(operations.ToList().ConvertListTo<IHasId, IOperation>());

            return new OperationProtocolLogic(store);
        }
        #endregion
    }
}
