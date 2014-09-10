using CuttingEdge.Conditions;
using Storidiom.Serialization;
using Storidiom.Thingness.Idioms.Store;
using Storidiom.Thingness.Idioms.Store.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storidiom.Extensions;
using Storidiom.Thingness.Idioms.Stringable;
using Storidiom.Thingness.Idioms.Stringable.Decorations;

namespace Storidiom.Thingness.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// serializes stores
    /// </summary>
    public sealed class StoreValueManager : INodeValueManager
    {
        public const string ID = "Store";

        #region Ctor
        public StoreValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            return obj.IsMarkedSerializable();
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            var ser = new BinarySerializationUtil();
            var data = ser.Serialize(obj);
            //prepend the assembly qualified type name
            data = obj.GetType().AssemblyQualifiedName + GraphingDelimiters.DELIM_LEVEL1 + data;
            
            return data;
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var ser = new BinarySerializationUtil();
            string[] split = new string[] { GraphingDelimiters.DELIM_LEVEL1 };
            var arr = nodeText.Split(split, StringSplitOptions.None);
            Condition.Requires(arr).IsNotNull();
            Condition.Requires(arr).HasLength(2);
            var typeName = arr[0];
            var serData = arr[1];

            Type type = Util.FindAssemblyQualifiedType(typeName);
            var obj = ser.Deserialize(type, serData);
            return obj;
        }
        #endregion

    }
}
