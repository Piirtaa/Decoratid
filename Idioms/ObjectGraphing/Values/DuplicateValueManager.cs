using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Storing.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
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
