using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.Encrypting;
using Decoratid.Storidioms;
using System;

namespace Decoratid.Messaging.StoreProtocol
{
    /// <summary>
    /// defines the client 
    /// </summary>
    public interface IStoreProtocolClient 
    {
        IStore Send(IStore request);
    }

    public class StoreProtocolClient : IStoreProtocolClient
    {
        #region Ctor
        public StoreProtocolClient(IEndPointClient client)
        {
            Condition.Requires(client).IsNotNull();
            this.Client = client;
        }
        #endregion

        #region Properties
        public IEndPointClient Client { get; protected set; }
        #endregion

        #region IStoreProtocolClient
        public IStore Send(IStore request)
        {
            //encode the store to send over the wire
            var requestText = StoreSerializer.SerializeStore(request, null);
            
            //send it
            var respData = this.Client.Send(requestText);

            //decode the response store
            var responseStore = StoreSerializer.DeserializeStore(respData, null, this.DecodingStrategy);
 
            return responseStore;
        }
        #endregion

        #region Static Methods
        public static StoreProtocolClient New(Func<string, string> encodingStrategy,
            Func<string, string> decodingStrategy, IEndPointClient client)
        {
            return new StoreProtocolClient(encodingStrategy, decodingStrategy, client);
        }
        public static StoreProtocolClient NewEncrypted(SymmetricCipherPair cp, IEndPointClient client)
        {
            return new StoreProtocolClient(cp.GetSymmetricEncodingStrategy(), cp.GetSymmetricDecodingStrategy(), client);
        }
        public static StoreProtocolClient NewPlaintext(IEndPointClient client)
        {
            return new StoreProtocolClient(null, null, client);
        }
        #endregion
    }
}
