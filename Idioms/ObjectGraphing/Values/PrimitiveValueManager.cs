using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// Handles system primitives as defined in PrimitivesUtil
    /// </summary>
    public sealed class PrimitiveValueManager : INodeValueManager
    {
        public const string ID = "Primitive";

        #region Ctor
        public PrimitiveValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return PrimitivesUtil.IsSystemPrimitive(obj);
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var name = obj.GetType().Name;
            var val = obj.ToString();
            return LengthEncoder.LengthEncodeList(name, val);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = LengthEncoder.LengthDecodeList(nodeText);
            Condition.Requires(list).HasLength(2);

            var type = PrimitivesUtil.GetSystemPrimitiveTypeBySimpleName(list.ElementAt(0));
            var val = PrimitivesUtil.ConvertStringToSystemPrimitive(list.ElementAt(1), type);

            if (type == typeof(string))
            {
                var stringVal = val.ToString();
                return stringVal;
            }
            else
            {
                return val;
            }
        }
        #endregion
    }
}
