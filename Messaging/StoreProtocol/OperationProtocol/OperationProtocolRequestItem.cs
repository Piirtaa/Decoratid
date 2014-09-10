using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Store;

namespace Decoratid.Messaging.StoreProtocol.OperationProtocol
{
    /// <summary>
    /// a wrapper for data we're storing in the request store
    /// </summary>
    [Serializable]
    public class OperationProtocolRequestItem : IHasId<string>
    {
        #region Ctor
        public OperationProtocolRequestItem(string operationName, object data)
        {
            this.Id = operationName + OperationProtocolConstants.Request_Suffix;
            this.Data = data;
        }
        #endregion

        #region Properties
        public object Data { get; set; }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Static Methods
        public static OperationProtocolRequestItem New(string operationName, object data)
        {
            OperationProtocolRequestItem item = new OperationProtocolRequestItem(operationName, data);
            return item;
        }
        #endregion
    }
}
