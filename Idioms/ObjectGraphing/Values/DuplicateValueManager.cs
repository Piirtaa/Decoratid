using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;

namespace Decoratid.Idioms.ObjectGraphing.Values
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
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            //we can handle this if it's  a duplicate reference of something already in the node store
            var matches = uow.NodeStore.SearchOf<GraphNode>(LogicOfTo<GraphNode,bool>.New((x) =>
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
            var matches = uow.NodeStore.SearchOf<GraphNode>(LogicOfTo<GraphNode, bool>.New((x) =>
            {
                if (x.NodeValue == null)
                    return false;

                return object.ReferenceEquals(x.NodeValue, obj);
            }));

            return LengthEncoder.LengthEncode(matches[0].Id);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            //get the node that is already hydrated, with the same provided path
            var matches = uow.NodeStore.SearchOf<GraphNode>(LogicOfTo<GraphNode,bool>.New((x) =>
            {
                if (x.NodeValue == null)
                    return false;

                return x.Id.Equals(nodeText);
            }));

            return LengthEncoder.LengthDecode(matches[0].Id);
        }
        #endregion
    }
}
