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
using Decoratid.Reflection;
using Decoratid.Thingness.Idioms.Hydrateable;

namespace Decoratid.Thingness.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// Handles instances that are IHydrateable by delegating their hydration to themselves
    /// </summary>
    public sealed class HydrateableValueManager : INodeValueManager
    {
        public const string ID = "Hydrateable";

        #region Ctor
        public HydrateableValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            return obj is IHydrateable;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            return HydrateableUtil.Dehydrate(obj as IHydrateable);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            return HydrateableUtil.Hydrate(nodeText);
        }
        #endregion

    }
}
