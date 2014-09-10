using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Store.Broker
{

    ///// <summary>
    ///// a broker of stores
    ///// </summary>
    //public interface IStoreBroker
    //{
    //    string BrokerName { get; }
    //    List<string> GetStoreNames();
    //    IStore GetStore(string name);
    //}

    /// <summary>
    /// Defines a connection string parser that will build stores
    /// </summary>
    public interface IStoreConnectionBuilder : IHasId<string>
    {
        bool CanHandle(StoreConnection conn);
        IStore GetStore(StoreConnection conn);
    }
}
