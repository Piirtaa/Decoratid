using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Messaging.StoreProtocoling
{
    /* 
     * The "store protocol" is just an exchange of stores (eg. request is a store and response is a store), serialized as strings. 

    */

    /// <summary>
    /// defines the restatement of the Communication process as an exchange of stores
    /// </summary>
    public interface IHasStoreProtocolLogic
    {
        LogicOf<Tuple<IStore, IStore>> StoreProtocolLogic { get; }
    }

    /// <summary>
    /// defines the client 
    /// </summary>
    public interface IStoreProtocolClient
    {
        IStore Send(IStore request);
    }


}
