using CuttingEdge.Conditions;
using Decoratid.Serialization;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Storing.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using Decoratid.Reflection;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// Handles instances that are IReconstable by delegating their hydration to themselves
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
            return obj is IReconstable;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            return Hydrator.Dehydrate(obj as IReconstable);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            return Hydrator.Hydrate(nodeText);
        }
        #endregion

    }
}
