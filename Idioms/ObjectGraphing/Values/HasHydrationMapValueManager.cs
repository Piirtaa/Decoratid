using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Collections.Generic;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// has a hydration map. 
    /// </summary>
    public interface IHasHydrationMap
    {
        IHydrationMap GetHydrationMap();
    }

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
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return obj is IHasHydrationMap;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            var typeName = obj.GetType().AssemblyQualifiedName;
            IHasHydrationMap hasMap = obj as IHasHydrationMap;
            var map = hasMap.GetHydrationMap();
            Condition.Requires(map).IsNotNull();
            var data = map.DehydrateValue(obj, uow);
            return LengthEncoder.LengthEncodeList(typeName, data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = LengthEncoder.LengthDecodeList(nodeText);
            Condition.Requires(list).HasLength(2);

            Type cType = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(list[0]);
            Condition.Requires(cType).IsNotNull();
            var obj = ReflectionUtil.CreateUninitializedObject(cType);
            IHasHydrationMap hasMap = obj as IHasHydrationMap;
            var map = hasMap.GetHydrationMap();
            map.HydrateValue(obj, list[1], uow);
            return obj;
        }
        #endregion

    }
}
