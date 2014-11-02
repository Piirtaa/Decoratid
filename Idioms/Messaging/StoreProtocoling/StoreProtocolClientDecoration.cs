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

namespace Decoratid.Idioms.Messaging.StoreProtocoling
{
    /// <summary>
    /// decorates with store protocol client
    /// </summary>
    public class StoreProtocolClientDecoration : DecoratedEndPointClientBase, IStoreProtocolClient
    {
        #region Ctor
        public StoreProtocolClientDecoration(IEndPointClient decorated, ValueManagerChainOfResponsibility valueManager = null)
            : base(decorated)
        {
            if (valueManager == null)
            {
                this.ValueManager = ValueManagerChainOfResponsibility.NewDefault();
            }
            else
            {
                this.ValueManager = valueManager;
            }
        }
        #endregion

        #region Properties
        public ValueManagerChainOfResponsibility ValueManager { get; private set; }
        #endregion

        #region IStoreProtocolClient
        public IStore Send(IStore request)
        {
            //encode the store to send over the wire
            var requestText = StoreSerializer.SerializeStore(request, this.ValueManager);

            //send it
            var respData = this.Decorated.Send(requestText);

            //decode the response store
            var responseStore = StoreSerializer.DeserializeStore(respData, this.ValueManager);

            return responseStore;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IEndPointClient> ApplyThisDecorationTo(IEndPointClient thing)
        {
            return new StoreProtocolClientDecoration(thing, this.ValueManager);
        }
        #endregion

        #region Fluent Static
        public static StoreProtocolClientDecoration New(IEndPointClient decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            return new StoreProtocolClientDecoration(decorated, valueManager);
        }
        #endregion
    }

    public static class StoreProtocolClientDecorationExtensions
    {
        public static StoreProtocolClientDecoration StoreProtocoling(this IEndPointClient decorated, ValueManagerChainOfResponsibility valueManager = null)
        {
            var rv = new StoreProtocolClientDecoration(decorated, valueManager);
            return rv;
        }
    }
}
