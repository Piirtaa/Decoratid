using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.OperationProtocoling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
{
    public interface IOperationProtocolClient
    {
        /// <summary>
        /// the request store that is built up by adding operations
        /// </summary>
        IStore RequestStore { get; }
        /// <summary>
        /// the response store that is populated by Perform()
        /// </summary>
        IStore ResponseStore { get; }

        /// <summary>
        /// sends the request store and sets te response store
        /// </summary>
        void Perform();

    }


}
