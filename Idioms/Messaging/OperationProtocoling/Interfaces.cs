using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Depending;
using Decoratid.Idioms.Tasking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
{
    /// <summary>
    /// defines an the operation
    /// </summary>
    public interface IOperation : IHasId<string>
    {
        Type ResponseType { get; }
        Type RequestType { get; }

        /// <summary>
        /// the logic we execute.  Because it's a "protocol" we specify an implementation (eg. ILogic) here.
        /// </summary>
        ICloneableLogic PerformLogic { get; }

        bool IsRequested(IStore requestStore);
        ITask GetPerformTask(IStore requestStore, IStore responseStore);
    }
}
