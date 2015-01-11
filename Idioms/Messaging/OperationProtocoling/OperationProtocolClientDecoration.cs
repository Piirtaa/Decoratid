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
    public class OperationProtocolClientDecoration : DecoratedEndPointClientBase, IOperationProtocolClient
    {
        #region Ctor
        public OperationProtocolClientDecoration(IEndPointClient decorated, ValueManagerChainOfResponsibility valueManager = null)
            : base(decorated.StoreProtocoling(valueManager))
        {
            this.RequestStore = NaturalInMemoryStore.New();
        }
        #endregion

        #region Properties
        public IStore RequestStore { get; protected set; }
        public IStore ResponseStore { get; protected set; }
        #endregion

        #region Methods
        public void Perform()
        {
            this.ResponseStore = this.As<StoreProtocolClientDecoration>(true).Send(this.RequestStore);
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IEndPointClient> ApplyThisDecorationTo(IEndPointClient thing)
        {
            return new OperationProtocolClientDecoration(thing, this.As<StoreProtocolClientDecoration>(true).ValueManager);
        }
        #endregion

        #region Fluent Static
        public static OperationProtocolClientDecoration New(IEndPointClient decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            return new OperationProtocolClientDecoration(decorated, valueManager);
        }
        #endregion
    }

    public static class OperationProtocolClientDecorationExtensions
    {
        public static OperationProtocolClientDecoration OperationProtocoling(this IEndPointClient decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            var rv = new OperationProtocolClientDecoration(decorated, valueManager);
            return rv;
        }
    }
}
