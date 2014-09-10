using CuttingEdge.Conditions;
using Decoratid.Serialization;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Stringable;
using Decoratid.Thingness.Idioms.Stringable.Decorations;
using Decoratid.Extensions;
using Decoratid.Reflection;
using Decoratid.Thingness.Idioms.Hydrateable;

namespace Decoratid.Thingness.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// Handles instances of IHasHydrationMap. 
    /// </summary>
    public sealed class HasHydrationMapValueManager : INodeValueManager
    {
        public const string ID = "HasHydrationMap";

        #region Ctor
        public HasHydrationMapValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            return obj is IHasHydrationMap;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            var typeName = obj.GetType().AssemblyQualifiedName;
            IHasHydrationMap hasMap = obj as IHasHydrationMap;
            var map = hasMap.GetHydrationMap();
            Condition.Requires(map).IsNotNull();
            var data = map.DehydrateValue(obj, uow);
            return TextDecorator.LengthEncodeList(typeName, data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = TextDecorator.LengthDecodeList(nodeText);
            Condition.Requires(list).HasLength(2);

            Type cType = TypeFinder.FindAssemblyQualifiedType(list[0]);
            Condition.Requires(cType).IsNotNull();
            var obj = ReflectionUtil.CreateUninitializedObject(cType);
            IHasHydrationMap hasMap = obj as IHasHydrationMap;
            var map = hasMap.GetHydrationMap();
            map.HydrateValue(obj,list[1], uow);
            return obj;
        }
        #endregion

    }
}
