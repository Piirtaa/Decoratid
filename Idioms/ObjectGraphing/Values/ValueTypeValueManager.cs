using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using System;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    public sealed class ValueTypeValueManager : INodeValueManager
    {
        public const string ID = "Value";

        #region Ctor
        public ValueTypeValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            return obj.GetType().IsValueType;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var name = obj.GetType().AssemblyQualifiedName;
            var ser = new BinarySerializationUtil();
            var data = ser.Serialize(obj);

            return LengthEncoder.LengthEncodeList(name, data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = LengthEncoder.LengthDecodeList(nodeText);

            Condition.Requires(list).HasLength(2);
            var ser = new BinarySerializationUtil();
            var typeName = list.ElementAt(0);
            var serData = list.ElementAt(1);
            Type type = TypeFinder.FindAssemblyQualifiedType(typeName);
            var obj = ser.Deserialize(type, serData);
            return obj;
        }
        #endregion

    }
}
