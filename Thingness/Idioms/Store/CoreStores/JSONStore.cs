using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Storidiom.Thingness;
using ServiceStack.Text;
using Storidiom.Extensions;
using System.IO;
using System.Runtime.Serialization;

namespace Storidiom.Thingness.Idioms.Store.CoreStores
{
    /// <summary>
    /// uses a JsonObject as a store
    /// </summary>
    /// 
    [Serializable]
    public class JSONStore : DisposableBase, IStore, IGetAllableStore//, ISerializable
    {
        #region Declarations
        protected readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public JSONStore()
        {
            this.JSONObject = new JsonObject();
        }
        public JSONStore(JsonObject obj)
        {
            Condition.Requires(obj).IsNotNull();
            this.JSONObject = obj;
        }
        #endregion

        //#region ISerializable
        //protected JSONStore(SerializationInfo info, StreamingContext context)
        //{
        //    this.JSONObject = (JsonObject)info.GetValue("_JSONObject", typeof(JsonObject));
        //}
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("_JSONObject", this.JSONObject, typeof(JsonObject));
        //}
        //#endregion

        #region Properties
        public JsonObject JSONObject { get; set; }
        #endregion

        #region IStore
        /// <summary>
        /// gets the property of type T with the name of the IHasId.Id value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iHasId"></param>
        /// <returns></returns>
        public virtual IHasId Get(IStoredObjectId soId)
        {
            if (soId == null) { return null; }

            //get the store item (our native format)
            StoredObjectId key = StoredObjectId.New(soId.ObjectType, soId.ObjectId);
            var item = this.JSONObject.Get<SerializedIHasId>(key.ToString());

            //return the nested object
            if (item == null)
                return null;

            return item.GetStoredItem();
        }
        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
            Condition.Requires(filter).IsNotNull();
            List<T> list = new List<T>();

            this.JSONObject.WithEach(x =>
            {
                try
                {
                    //get the store item
                    SerializedIHasId item = JsonSerializer.DeserializeFromString<SerializedIHasId>(x.Value);

                    //test the nested object
                    if (item != null)
                    {
                        T t = (T)item.GetStoredItem();

                        if (filter.Filter(t))
                            list.Add(t);
                    }
                }
                catch { }
            });
            return list;
        }
        public virtual void Commit(ICommitBag bag)
        {
            Condition.Requires(bag).IsNotNull();

            //lock the store
            lock (this._stateLock)
            {
                bag.ItemsToDelete.WithEach(x =>
                {
                    this.JSONObject.Remove(x.ToString());
                });
                bag.ItemsToSave.WithEach(x =>
                {
                    SerializedIHasId item = SerializedIHasId.New(x);
                    this.JSONObject[item.Id.ToString()] = JsonSerializer.SerializeToString<SerializedIHasId>(item);
                });
            }
        }
        public virtual List<IHasId> GetAll()
        {
            List<IHasId> list = new List<IHasId>();

            this.JSONObject.WithEach(x =>
            {
                try
                {
                    //get the store item
                    SerializedIHasId item = JsonSerializer.DeserializeFromString<SerializedIHasId>(x.Value);

                    //test the nested object
                    if (item != null)
                        list.Add(item.GetStoredItem());
                }
                catch { }
            });
            return list;
        }
        #endregion

        #region Static Fluent Methods
        public static JSONStore New()
        {
            return new JSONStore();
        }
        #endregion
    }
}
