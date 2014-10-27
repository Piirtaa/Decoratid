using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;
using Decoratid.Core.Identifying;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
{
    /// <summary>
    /// an operation's Response is stored with this container
    /// </summary>
    [Serializable]
    public class OperationResponse : IHasId<string>
    {
        #region Ctor
        public OperationResponse(string operationName, object data)
        {
            this.Id = operationName;
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
        public static OperationResponse New(string operationName, object data)
        {
            OperationResponse item = new OperationResponse(operationName, data);
            return item;
        }
        #endregion
    }
}
