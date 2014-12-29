using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Storidioms;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.ObjectGraphing
{
    public static class NodeStoreUtil
    {
        /// <summary>
        /// given a path and a nodestore, tries to locate a node
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public static GraphNode GetNode(this IStoreOf<GraphNode> nodeStore, GraphPath path)
        {
            Condition.Requires(path).IsNotNull();
            Condition.Requires(nodeStore).IsNotNull();

            var node = nodeStore.Get<GraphNode>(path.Path);
            return node;
        }
        public static GraphNode GetRootNode(this IStoreOf<GraphNode> nodeStore)
        {
            return GetNode(nodeStore, GraphPath.New());
        }
        /// <summary>
        /// given a nodestore finds the current node's parent node
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public static GraphNode GetParentNode(this IStoreOf<GraphNode> nodeStore, GraphPath path)
        {
            Condition.Requires(nodeStore).IsNotNull();

            GraphNode rv = null;

            //if we're on a root node, there is no parent
            if (path.IsRoot)
                return null;

            var parentPath = path.ParentPath;
            var matches = nodeStore.SearchOf(LogicOfTo<GraphNode, bool>.New((x) => { return x.Id.Equals(parentPath); }));
            rv = matches.FirstOrDefault();
            return rv;
        }
        /// <summary>
        /// gets all children of the current node who have a parent path equal to the current node's path
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public static List<GraphNode> GetImmediateChildNodes(this IStoreOf<GraphNode> nodeStore, GraphPath path)
        {
            Condition.Requires(nodeStore).IsNotNull();
            var matches = nodeStore.SearchOf<GraphNode>(LogicOfTo<GraphNode, bool>.New((x) => { return x.Path.ParentPath.Equals(path); }));
            return matches;
        }
        /// <summary>
        /// gets all children of the current node (those with paths that begin with the current node's path)
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public static List<GraphNode> GetChildNodes(this IStoreOf<GraphNode> nodeStore, GraphPath path)
        {
            Condition.Requires(nodeStore).IsNotNull();
            var matches = nodeStore.SearchOf(LogicOfTo<GraphNode,bool>.New((x) => { return x.Path.Path.StartsWith(path.Path); }));
            return matches;
        }

        #region Query Methods
        /// <summary>
        /// if the path points to an IHasId node value, we grab the Id branches
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        //public static List<GraphNode> GetIdNodesFromNode(GraphPath path, IStoreOf<GraphNode> nodeStore)
        //{
        //    var node = GetNode(path, nodeStore);
        //    if (node == null)
        //        return null;

        //    //find the immediate child id node
        //    var children = node.GetImmediateChildNodes(nodeStore);

        //    if(children != null && children.Count > 0)
        //    {
        //        foreach(var each in children)
        //        {
        //            if (!(each.Path.CurrentSegment is GraphSegment))
        //            {
        //                continue;
        //            }
        //            var seg = (GraphSegment)each.Path.CurrentSegment;
        //            if(seg.SegmentName
        //        }
        //    }
        //}
     
        #endregion


        #region Static List/Store  Serialization/Hydration  
        /// <summary>
        /// converts Node Store to a list of nodes and serializes it
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public static string DehydrateNodeStore(this IStoreOf<GraphNode> nodeStore)
        {
            Condition.Requires(nodeStore).IsNotNull();
            var nodes = nodeStore.GetAll();
            var rv = DehydrateNodeList(nodes);
            return rv;
        }
        /// <summary>
        /// deserializes text to a Node Store.  The node values are unhydrated.
        /// </summary>
        /// <param name="storeText"></param>
        /// <returns></returns>
        public static IStoreOf<GraphNode> HydrateNodeStore(string storeText)
        {
            var nodeStore = NaturalInMemoryStore.New().IsOf<GraphNode>();
            var list = HydrateNodeList(storeText);
            nodeStore.SaveItems(list.ConvertListTo<IHasId, GraphNode>());
            return nodeStore;
        }
        /// <summary>
        /// serializes the list of nodes by dehydrating each node and adding to a delimited string
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static string DehydrateNodeList(List<GraphNode> nodes)
        {
            if (nodes == null)
                return null;

            List<string> lines = new List<string>();
            nodes.WithEach(node =>
            {
                lines.Add(node.GetValue());
            });
            var rv = LengthEncoder.LengthEncodeList(lines.ToArray());
            return rv;
        }
        /// <summary>
        /// deserializes into a list of nodes
        /// </summary>
        /// <param name="storeText"></param>
        /// <returns></returns>
        public static List<GraphNode> HydrateNodeList(string storeText)
        {
            if (string.IsNullOrEmpty(storeText))
                return null;

            var lines = LengthEncoder.LengthDecodeList(storeText);

            var list = new List<GraphNode>();
            for (int i = 0; i < lines.Count; i = i + 1)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                string line = lines[i];
                GraphNode node = line;

                list.Add(node);
            }
            return list;
        }

        #endregion
    }
}
