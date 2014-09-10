using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Storidiom.Utils;
using ServiceStack.Text;
using Storidiom.Extensions;
using Storidiom.Serialization;
using Storidiom.Thingness.Idioms.Logics;
using Storidiom.Thingness.Idioms.ObjectGraph;

namespace Storidiom.Thingness.Idioms.Store
{
    /// <summary>
    /// Serializes an IHasId with Grapher.  Is coupled to Grapher in the serialization concerns also in the handling of Graphing reserved
    /// characters.
    /// </summary>
    public class SerializedIHasId : ContextualAsId<string,string>, IHydrateable
    {
        private static GrapherSerializationUtil _serializer = new GrapherSerializationUtil();

        #region Ctor
        private SerializedIHasId() { }
        private SerializedIHasId(IHasId obj)
        {
            Condition.Requires(obj).IsNotNull();

            var id = obj.GetStoredObjectId();
            this.Id = _serializer.Serialize(id);
            this.Context = _serializer.Serialize(obj);
        }
        #endregion

        #region IHydrateable
        /// <summary>
        /// serializes node to string
        /// </summary>
        /// <returns></returns>
        public string Dehydrate()
        {
            //use a reserved delimiter that will not be found in the graph itself (to avoid parsing exploits)
            string rv = string.Join(GraphingDelimiters.DELIM_LEVEL4, this.Id, this.Context);
            return rv;
        }
        /// <summary>
        /// hydrates node from string
        /// </summary>
        /// <param name="text"></param>
        public void Hydrate(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            var split = new string[] { GraphingDelimiters.DELIM_LEVEL4 };

            var arr = text.Split(split, StringSplitOptions.None);
            Condition.Requires(arr).IsNotEmpty();
            Condition.Requires(arr).HasLength(4);
            this.Id = arr[0];
            this.Context = arr[1];
            
            this.ValidateIsHydrated();
        }

        /// <summary>
        /// checks that the node's dehydrated parts are all non-null
        /// </summary>
        private void ValidateIsHydrated()
        {
            Condition.Requires(this.Context).IsNotNullOrEmpty();
            Condition.Requires(this.Id).IsNotNullOrEmpty();
        }
        #endregion

        #region Methods
        public IHasId GetStoredItem()
        {
            var rv = _serializer.Deserialize(null, this.Context);
            return rv as IHasId;
        }
        #endregion

        #region Static Builder
        public static string NewId(StoredObjectId id)
        {
            var val = _serializer.Serialize(id);
            return val;
        }
        public static SerializedIHasId New(IHasId obj)
        {
            return new SerializedIHasId(obj);
        }
        public static SerializedIHasId Parse(string data)
        {
            var item = new SerializedIHasId();
            item.Hydrate(data);
            return item;
        }
        public static string Dehydrate(IHasId obj)
        {
            var item = new SerializedIHasId(obj);
            var rv = item.Dehydrate();
            return rv;
        }
        #endregion

        #region Static List Methods
        public static string DehydrateList(List<IHasId> list)
        {
            StringBuilder sb = new StringBuilder();
            list.WithEach(x =>
            {
                sb.AppendLine(Dehydrate(x));
            });
            return sb.ToString();
        }
        public static List<IHasId> RehydrateList(string text)
        {
            List<IHasId> rv = new List<IHasId>();
            string[] split = new string[]{GraphingDelimiters.CRLF};
            var list = text.Split(split, StringSplitOptions.RemoveEmptyEntries);
            list.WithEach(x =>
            {
                var item = Parse(x);
                var sItem = item.GetStoredItem();
                rv.Add(sItem);
            });
            return rv;
        }
        #endregion
    }
}
