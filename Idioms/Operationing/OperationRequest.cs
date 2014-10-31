using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;
using Decoratid.Core.Identifying;

namespace Decoratid.Idioms.Operationing
{
    /// <summary>
    /// an operation's request is stored with this container
    /// </summary>
    [Serializable]
    public class OperationRequest : IHasId<string>
    {
        #region Ctor
        public OperationRequest(string operationName, object data)
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
        public static OperationRequest New(string operationName, object data)
        {
            OperationRequest item = new OperationRequest(operationName, data);
            return item;
        }
        #endregion
    }
}
