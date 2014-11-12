using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// uses binary serializer. handles anything.  
    /// </summary>
    public class BinarySerializingValueManager : INodeValueManager
    {
        public const string ID = "BinarySerializing";

        #region Ctor
        public BinarySerializingValueManager() { }
        #endregion

        #region IHasId
        public virtual string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public virtual bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return true;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var name = obj.GetType().AssemblyQualifiedName;
            var data = BinarySerializationUtil.Serialize(obj);

            return LengthEncoder.LengthEncodeList(name, data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = LengthEncoder.LengthDecodeList(nodeText);

            Condition.Requires(list).HasLength(2);
            var typeName = list.ElementAt(0);
            var serData = list.ElementAt(1);
            Type type = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(typeName);
            var obj = BinarySerializationUtil.Deserialize(type, serData);
            return obj;
        }
        #endregion

    }
}
