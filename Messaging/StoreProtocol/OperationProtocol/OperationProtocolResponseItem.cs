using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Tasks.Core;

namespace Decoratid.Messaging.StoreProtocol.OperationProtocol
{
    /// <summary>
    /// a wrapper for data we're storing in the response store
    /// </summary>
    /// 
    [Serializable]
    public class OperationProtocolResponseItem : IHasId<string>
    {
        #region Ctor
        public OperationProtocolResponseItem(string operationName, StrategizedTask data)
        {
            this.Id = operationName + OperationProtocolConstants.Response_Suffix;
            this.Data = data;
        }
        #endregion

        #region Properties
        public StrategizedTask Data { get; set; }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion
    }
}
