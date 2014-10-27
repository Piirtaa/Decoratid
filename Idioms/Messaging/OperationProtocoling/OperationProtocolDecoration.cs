using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Storidioms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Messaging.StoreProtocoling;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Extensions;
using Decoratid.Idioms.Tasking;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
{
    /// <summary>
    /// decorates the host by giving it an operation protocol implementation
    /// </summary>
    public class OperationProtocolDecoration : DecoratedEndPointHostBase
    {
        #region Ctor
        public OperationProtocolDecoration(IEndPointHost decorated, ValueManagerChainOfResponsibility valueManager = null)
            : base(decorated.StoreProtocoling(null, valueManager))
        {
            //replace the store protocol logic
            this.FindDecoratorOf<StoreProtocolDecoration>(true).HasStoreProtocolLogic(
                LogicOf<Tuple<IStore, IStore>>.New((uow) =>
                {
                    this.HandleOperations(uow.Item1, uow.Item2);
                }));

            this.Operations = NaturalInMemoryStore.New().IsOf<IOperation>();
        }
        #endregion

        #region IHasStoreProtocolLogic
        private IStoreOf<IOperation> Operations { get; set; }
        #endregion

        #region Methods
        public void AddOperation(IOperation operation)
        {
            Condition.Requires(operation).IsNotNull();
            this.Operations.SaveItem(operation);
        }
        #endregion

        #region Helper Methods
        private void HandleOperations(IStore requestStore, IStore responseStore)
        {
            //TODO: use a better id here, one that captures machine state
            //TODO: id decorations (AppendIP, AppendMachineName, etc).  use this to drive the default id stuff
            var job = Job.NewWithNeverExpireDefault("job " + DateTime.UtcNow.ToString());

            var ops = this.Operations.GetAll();
            ops.WithEach(op =>
            {
                if (op.IsRequested(requestStore))
                {
                    var task = op.GetPerformTask(requestStore, responseStore);
                    job.AddTask(task);
                }
            });

            job.RunToCompletion();
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IEndPointHost> ApplyThisDecorationTo(IEndPointHost thing)
        {
            return new OperationProtocolDecoration(thing, this.FindDecoratorOf<StoreProtocolDecoration>(true).ValueManager);
        }
        #endregion

        #region Fluent Static
        public static OperationProtocolDecoration New(IEndPointHost decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            return new OperationProtocolDecoration(decorated, valueManager);
        }
        #endregion
    }

    public static class OperationProtocolDecorationExtensions
    {
        public static OperationProtocolDecoration OperationProtocoling(this IEndPointHost decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            var rv = new OperationProtocolDecoration(decorated, valueManager);
            return rv;
        }
    }
}
