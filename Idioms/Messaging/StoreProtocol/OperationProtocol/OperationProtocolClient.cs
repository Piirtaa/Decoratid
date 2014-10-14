using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing;

namespace Decoratid.Messaging.StoreProtocol.OperationProtocol
{
    /// <summary>
    /// prepares and sends the operation protocol requests
    /// </summary>
    public class OperationProtocolClient
    {
        #region Ctor
        public OperationProtocolClient(StoreProtocolClient storeProtocolClient)
        {
            Condition.Requires(storeProtocolClient).IsNotNull();
            this.StoreProtocolClient = storeProtocolClient;
            this.RequestStore = InMemoryStore.New();
        }
        #endregion

        #region Properties
        public StoreProtocolClient StoreProtocolClient { get; protected set; }
        public IStore RequestStore { get; protected set; }
        public IStore ResponseStore { get; protected set; }
        #endregion

        #region Methods
        /// <summary>
        /// adds request data to the store
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="operationName"></param>
        /// <param name="arg"></param>
        public void AppendOperation<TArg>(string operationName, TArg arg)
        {
            this.RequestStore.SaveItem(OperationProtocolRequestItem.New(operationName, arg));
        }
        public IStore Perform()
        {
            this.ResponseStore = this.StoreProtocolClient.Send(this.RequestStore);
            return this.ResponseStore;
        }
        public OperationProtocolResponseItem GetOperationResult(string operationName)
        {
            return this.ResponseStore.Get<OperationProtocolResponseItem>(operationName + OperationProtocolConstants.Response_Suffix);
        }
        #endregion

        #region Static Methods
        public static OperationProtocolClient New(StoreProtocolClient storeProtocolClient)
        {
            return new OperationProtocolClient(storeProtocolClient);
        }
        #endregion
    }
}
