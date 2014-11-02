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
    /// an operation's Error is stored with this container
    /// </summary>
    [Serializable]
    public class OperationError : IHasId<string>
    {
        #region Ctor
        public OperationError(string operationName, Exception error)
        {
            this.Id = operationName;
            this.Error = error;
        }
        #endregion

        #region Properties
        public Exception Error { get; set; }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Static Methods
        public static OperationError New(string operationName, Exception error)
        {
            OperationError item = new OperationError(operationName, error);
            return item;
        }
        #endregion
    }
}
