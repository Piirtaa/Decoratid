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

namespace Decoratid.Idioms.OperationProtocoling
{
    /*
     *  Operations are ofTo logic with known arg/return, that have dependencies similar to tasks, and get their args from a request store,
     *  and set their return values to a store.  The location of the args/return values/errors conforms to an Id convention - thus the 
     *  "Operation Protocol".
     * 
     */
 
    /// <summary>
    /// defines an operation which is LogicOfTo + IHasId expressed as a taskable.   implicit in this definition is the
    /// OperationProtocol which pulls Arguments from a store, and saves results/errors to a store.
    /// </summary>
    public interface IOperation : IHasId<string>
    {
        Type ArgumentType { get; }
        Type ResultType { get; }

        ILogic OperationLogic { get; }
       
        /// <summary>
        /// Get the task(appropriately decorated) that will execute the OperationLogic
        /// </summary>
        /// <param name="requestStore"></param>
        /// <param name="responseStore"></param>
        /// <returns></returns>
        ITask GetTask(IStore requestStore, IStore responseStore);
    }
}
