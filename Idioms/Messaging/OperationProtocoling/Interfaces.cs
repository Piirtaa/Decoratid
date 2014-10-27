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

        ICloneableLogic PerformLogic { get; }
       
        /// <summary>
        /// is the request present in the store?
        /// </summary>
        /// <param name="requestStore"></param>
        /// <returns></returns>
        bool IsRequested(IStore requestStore);
        /// <summary>
        /// Get the task(appropriately decorated) that will execute the PerformLogic
        /// </summary>
        /// <param name="requestStore"></param>
        /// <param name="responseStore"></param>
        /// <returns></returns>
        ITask GetPerformTask(IStore requestStore, IStore responseStore);
    }
}
