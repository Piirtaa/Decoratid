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
using Decoratid.Idioms.OperationProtocoling;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
{
    /// <summary>
    /// decorates the host by giving it an operation protocol implementation
    /// </summary>
    public class OperationProtocolHostDecoration : DecoratedEndPointHostBase
    {
        #region Ctor
        public OperationProtocolHostDecoration(IEndPointHost decorated, ValueManagerChainOfResponsibility valueManager = null)
            : base(decorated.StoreProtocoling(null, valueManager))
        {
            //replace the store protocol logic
            this.FindDecoratorOf<StoreProtocolHostDecoration>(true).HasStoreProtocolLogic(
                LogicOf<Tuple<IStore, IStore>>.New((uow) =>
                {
                    this.OperationManager.HandleOperations(uow.Item1, uow.Item2);
                }));

            this.OperationManager = OperationManager.New();
        }
        #endregion

        #region Properties
        private OperationManager OperationManager { get; set; }
        #endregion

        #region Methods
        public void AddOperation(IOperation operation)
        {
            this.OperationManager.AddOperation(operation);
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IEndPointHost> ApplyThisDecorationTo(IEndPointHost thing)
        {
            return new OperationProtocolHostDecoration(thing, this.FindDecoratorOf<StoreProtocolHostDecoration>(true).ValueManager);
        }
        #endregion

        #region Fluent Static
        public static OperationProtocolHostDecoration New(IEndPointHost decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            return new OperationProtocolHostDecoration(decorated, valueManager);
        }
        #endregion
    }

    public static class OperationProtocolDecorationExtensions
    {
        public static OperationProtocolHostDecoration OperationProtocoling(this IEndPointHost decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            var rv = new OperationProtocolHostDecoration(decorated, valueManager);
            return rv;
        }
    }
}
