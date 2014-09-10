using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Stringable;
using Decoratid.Thingness.Idioms.Stringable.Decorations;
using Decoratid.Thingness.Idioms.Hydrateable;

namespace Decoratid.Thingness.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// Handles Duplicates.  Each call to this does a graph scan, so it scales badly on large graphs
    /// </summary>
    public sealed class DuplicateValueManager : INodeValueManager
    {
        public const string ID = "Duplicate";

        #region Ctor
        public DuplicateValueManager()
        {
        }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            //we can handle this if it's  a duplicate reference of something already in the node store
            var matches = uow.NodeStore.Search<GraphNode>(SearchFilterOf<GraphNode>.NewOf((x) =>
            {
                if (x.NodeValue == null)
                    return false;

                return object.ReferenceEquals(x.NodeValue, obj);
            }));

            if (matches != null && matches.Count > 0)
            {
                return true;
            }
            return false;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            //we can handle this if it's  a duplicate reference of something already in the node store
            var matches = uow.NodeStore.Search<GraphNode>(SearchFilterOf<GraphNode>.NewOf((x) =>
            {
                if (x.NodeValue == null)
                    return false;

                return object.ReferenceEquals(x.NodeValue, obj);
            }));

            return TextDecorator.LengthEncode(matches[0].Id);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            //get the node that is already hydrated, with the same provided path
            var matches = uow.NodeStore.Search<GraphNode>(SearchFilterOf<GraphNode>.NewOf((x) =>
            {
                if (x.NodeValue == null)
                    return false;

                return x.Id.Equals(nodeText);
            }));

            return TextDecorator.LengthDecode(matches[0].Id);
        }
        #endregion
    }
}
