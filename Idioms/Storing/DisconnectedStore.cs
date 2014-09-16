using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Storidiom.Extensions;
using ServiceStack.Text;

namespace Storidiom.Thingness.Idioms.Store
{    
    /// <summary>
    /// defines a textual representation of a store that can be sent over a wire, for instance
    /// </summary>
    public interface IDisconnectedStore 
    {
        string StoreData { get; }
        IStore Store { get; }
    }

    /// <summary>
    /// defines a textual representation of a store that can be sent over a wire, for instance
    /// </summary>
    public struct DisconnectedStore : IDisconnectedStore
    {
        #region Declarations
        private readonly string _data;
        private readonly IStore _store;
        #endregion
        
        #region Ctor
        /// <summary>
        /// use this ctor when creating a disconnected store from an existing store
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeEncodingStrategy"></param>
        /// <param name="storeDecodingStrategy"></param>
        public DisconnectedStore(IStore store, Func<string, string> storeEncodingStrategy)
        {
            Condition.Requires(store).IsNotNull();
            this._data = StoreSerializer.SerializeAndEncodeStoreData(store, storeEncodingStrategy);
            this._store = store;
        }
        /// <summary>
        /// use this ctor when creating a disconnected store from existing encoded/disconnected data
        /// </summary>
        /// <param name="storeData"></param>
        /// <param name="storeDecodingStrategy"></param>
        public DisconnectedStore(string storeData, Func<string, string> storeDecodingStrategy)
        {
            Condition.Requires(storeData).IsNotNullOrEmpty();
            this._data = storeData;
            this._store = StoreSerializer.DecodeAndDeserializeStoreData(this._data, storeDecodingStrategy);
            
        }
        #endregion

        #region IDisconnectedStore
        public string StoreData { get { return this._data; } }
        public IStore Store{ get { return this._store; } }
        #endregion
    }
}
