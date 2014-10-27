using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.Encrypting;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Storidioms;
using System;

namespace Decoratid.Idioms.Messaging.StoreProtocoling
{


    public class StoreProtocolClient : IStoreProtocolClient
    {
        #region Ctor
        public StoreProtocolClient(IEndPointClient client, ValueManagerChainOfResponsibility valueManager = null)
        {
            Condition.Requires(client).IsNotNull();
            this.Client = client;

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
        public IEndPointClient Client { get; protected set; }
        private ValueManagerChainOfResponsibility ValueManager { get; set; }
        #endregion

        #region IStoreProtocolClient
        public IStore Send(IStore request)
        {
            //encode the store to send over the wire
            var requestText = StoreSerializer.SerializeStore(request, this.ValueManager);
            
            //send it
            var respData = this.Client.Send(requestText);

            //decode the response store
            var responseStore = StoreSerializer.DeserializeStore(respData, this.ValueManager);
 
            return responseStore;
        }
        #endregion

        #region Static Methods
        public static StoreProtocolClient New(IEndPointClient client, ValueManagerChainOfResponsibility valueManager = null)
        {
            return new StoreProtocolClient(client, valueManager);
        }
        #endregion
    }
}
