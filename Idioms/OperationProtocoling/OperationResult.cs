using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;
using Decoratid.Core.Identifying;

namespace Decoratid.Idioms.OperationProtocoling
{
    /// <summary>
    /// an operation's Response is stored with this container
    /// </summary>
    [Serializable]
    public class OperationResult : IHasId<string>
    {
        #region Ctor
        public OperationResult(string operationName, object data)
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
        public static OperationResult New(string operationName, object data)
        {
            OperationResult item = new OperationResult(operationName, data);
            return item;
        }
        #endregion
    }
}
