using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Storing.Decorations.StoreOf;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// Handles system primitives
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
        public bool CanHandle(object obj, IGraph uow)
        {
            return PrimitivesUtil.IsSystemPrimitive(obj);
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var name = obj.GetType().Name;
            var val = obj.ToString();
            return TextDecorator.LengthEncodeList(name, val);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = TextDecorator.LengthDecodeList(nodeText);
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
