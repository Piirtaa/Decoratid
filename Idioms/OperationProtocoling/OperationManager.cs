using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Idioms.Tasking;
using Decoratid.Storidioms.StoreOf;
using System;
using Decoratid.Extensions;
using System.Collections.Generic;

namespace Decoratid.Idioms.OperationProtocoling
{
    /// <summary>
    /// maintains a set of operations, and knows how to evaluate the operations against an argument store
    /// </summary>
    public class OperationManager 
    {
        #region Ctor
        public OperationManager()
        {
            this.Operations = NaturalInMemoryStore.New().IsOf<IOperation>();
        }
        #endregion

        #region Properties
        private IStoreOf<IOperation> Operations { get; set; }
        #endregion

        #region Methods
        public void AddOperation(IOperation operation)
        {
            Condition.Requires(operation).IsNotNull();
            this.Operations.SaveItem(operation);
        }
        #endregion

        #region Methods
        public void HandleOperations(IStore requestStore, IStore responseStore)
        {
            var job = Job.NewWithNeverExpireDefault("job " + DateTime.UtcNow.ToString());

            var ops = this.Operations.GetAll();
            ops.WithEach(op =>
            {
                if (this.IsRequested(requestStore, op.Id))
                {
                    var task = op.GetTask(requestStore, responseStore);
                    job.AddTask(task);
                }
            });

            job.RunToCompletion();
        }
        public List<OperationError> GetErrors(IStore responseStore)
        {
            var rv = responseStore.GetAll<OperationError>();
            return rv;
        }
        public List<OperationResult> GetResults(IStore responseStore)
        {
            var rv = responseStore.GetAll<OperationResult>();
            return rv;
        }
        
        private bool IsRequested(IStore requestStore, string operationName)
        {
            Condition.Requires(requestStore).IsNotNull();
            var req = requestStore.Get<OperationArg>(operationName);
            return req != null;
        }
        #endregion


        #region Fluent Static
        public static OperationManager New()
        {
            return new OperationManager();
        }
        #endregion
    }
}
