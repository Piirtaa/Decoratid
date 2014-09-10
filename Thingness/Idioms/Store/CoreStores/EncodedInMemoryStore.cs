//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
//using CuttingEdge.Conditions;
//using Decoratid.Crypto;
//using Decoratid.Extensions;
//using Decoratid.Thingness.Idioms.Logics;
//using Decoratid.Thingness.Idioms.ValuesOf;
//using Decoratid.Thingness.Idioms.ObjectGraph.Values;

//namespace Decoratid.Thingness.Idioms.Store.CoreStores
//{
//    /// <summary>
//    /// an in-memory store that saves entries as SerializedStoredItem.  Thus this is a very expensive store to use, as 
//    /// each item is serialized and deserialized.
//    /// </summary>
//    [Serializable]
//    public class EncodedInMemoryStore : IStore, IGetAllableStore
//    {
//        #region Declarations
//        private readonly object _stateLock = new object();
//        #endregion

//        #region Ctor
//        public EncodedInMemoryStore(ValueManagerSet managerSet = null, LogicOfTo<string, string> encodeWriteDataStrategy = null, LogicOfTo<string, string> decodeReadDataStrategy = null)
//        {
//            this.Dictionary = new Dictionary<string, string>();
//            this.EncodeWriteDataStrategy = encodeWriteDataStrategy;
//            this.DecodeReadDataStrategy = decodeReadDataStrategy;
//        }
//        #endregion

//        #region Properties
//        protected Dictionary<string, string> Dictionary { get; set; }
//        /// <summary>
//        /// if we want to encode the data we are writing, we provide a strategy here
//        /// </summary>
//        public LogicOfTo<string, string> EncodeWriteDataStrategy { get; set; }
//        /// <summary>
//        /// if we want to decode an encoded stream we are reading, we provide a strategy here
//        /// </summary>
//        public LogicOfTo<string, string> DecodeReadDataStrategy { get; set; }
//        #endregion

//        #region IStore
//        public virtual IHasId Get(IStoredObjectId soId)
//        {
//            IHasId obj = null;

//            SerializedIHasId item;
//            var key = StoredObjectId.New(soId.ObjectType, soId.ObjectId);
//            var keyString = SerializedIHasId.NewId(key);

//            if (this.Dictionary.TryGetValue(keyString, out item))
//            {
//                //decode the string first
//                item.Context = this.DecodeReadData(item.Context);

//                obj = item.GetStoredItem();
//            }

//            return obj;
//        }
//        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
//        {
//            Condition.Requires(filter).IsNotNull();

//            List<T> returnValue = new List<T>();
//            Type filterType = typeof(T);

//            //lock and retrieve the values
//            List<IHasId> vals = this.GetAll();

//            vals.WithEach(x =>
//            {
//                //if the item is the wrong type, skip it
//                var type = x.GetType();
//                if (filterType.IsAssignableFrom(type))
//                {
//                    if (filter.Filter((T)x))
//                    {
//                        returnValue.Add((T)x);
//                    }
//                }
//            });

//            return returnValue;
//        }
//        public virtual void Commit(ICommitBag bag)
//        {
//            Condition.Requires(bag).IsNotNull();

//            //lock the store
//            lock (this._stateLock)
//            {
//                bag.ItemsToDelete.WithEach(x =>
//                {
//                    var keyString = SerializedIHasId.NewId(x);
//                    this.Dictionary.Remove(keyString);
//                });
//                bag.ItemsToSave.WithEach(x =>
//                {
//                    //build a serializedstoreditem, encode the data, and then save - keyed by the soid of the storeditem
//                    var item = SerializedIHasId.New(x);
//                    item.Context = this.EncodeWriteData(item.Context);
//                    this.Dictionary[item.Id] = item;
//                });
//            }
//        }
//        #endregion

//        #region IGetAllable
//        public virtual List<IHasId> GetAll()
//        {
//            List<IHasId> returnValue = new List<IHasId>();
//            this.Dictionary.Values.WithEach(item =>
//            {
//                item.Context = this.DecodeReadData(item.Context);
//                var obj = item.GetStoredItem();
//                returnValue.Add(obj);
//            });
//            return returnValue;
//        }
//        #endregion

//        #region Helpers
//        protected string DecodeReadData(string text)
//        {
//            if (this.DecodeReadDataStrategy == null)
//                return text;

//            return this.DecodeReadDataStrategy.CloneAndPerform(text.ValueOf());
//        }
//        protected string EncodeWriteData(string text)
//        {
//            if (this.EncodeWriteDataStrategy == null)
//                return text;

//            return this.EncodeWriteDataStrategy.CloneAndPerform(text.ValueOf());
//        }
//        #endregion

//        #region Static Fluent Methods
//        public static EncodedInMemoryStore New(LogicOfTo<string, string> encodeWriteDataStrategy = null, LogicOfTo<string, string> decodeReadDataStrategy = null)
//        {
//            return new EncodedInMemoryStore(encodeWriteDataStrategy, decodeReadDataStrategy);
//        }
//        #endregion
//    }
//}
