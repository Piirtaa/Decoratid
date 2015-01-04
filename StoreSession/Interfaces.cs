using Decoratid.Core.Identifying;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.StoreSession
{
    public interface IThing
    {
        IHasId<string> IHasId { get; }
        
    }

    public interface ISession : IStoreOfUniqueId
    {

    }
}
