using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Communication;
using Decoratid.Crypto;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness.Idioms.ValuesOf;

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
        public StoreProtocolClient(Func<string, string> encodingStrategy,
            Func<string, string> decodingStrategy, IEndPointClient client)
        {
            Condition.Requires(client).IsNotNull();
            this.EncodingStrategy = encodingStrategy;
            this.DecodingStrategy = decodingStrategy;
            this.Client = client;
        }
        #endregion

        #region Properties
        public Func<string, string> EncodingStrategy { get; protected set; }
        public Func<string, string> DecodingStrategy { get; protected set; }
        public IEndPointClient Client { get; protected set; }
        #endregion

        #region IStoreProtocolClient
        public IStore Send(IStore request)
        {
            //encode the store to send over the wire
            var requestText = StoreSerializer.SerializeStore(request, null, this.EncodingStrategy);
            
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
