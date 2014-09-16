using CuttingEdge.Conditions;
using Decoratid.Reflection;
using Decoratid.Idioms.ObjectGraph.Path;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Storing.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using System.Collections;
using Decoratid.Idioms.Storing.Core;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph
{
    /// <summary>
    /// GraphPath + ManagedValue + Sequence
    /// </summary>
    public class GraphNode : ContextualAsId<string, string>, IReconstable
    {
        #region Ctor
        private GraphNode() { }
        private GraphNode(GraphPath path, object nodeValue, int traversalIndex, string valueMgrId, string serializedNodeValue)
            : base()
        {
            Condition.Requires(path).IsNotNull();
            Condition.Requires(traversalIndex).IsGreaterThan(-1);
            //validate the serialized value doesn't have reserved characters
            this.TraversalIndex = traversalIndex;
            this.ValueManagerId = valueMgrId;
            this.Context = serializedNodeValue;
            this.Id = path.Path;

            //set placeholder
            this.NodeValue = nodeValue;
        }
        #endregion

        #region Properties
        public int TraversalIndex { get; private set; }
        public string ValueManagerId { get; private set; }
        #endregion

        #region Placeholder - not involved in hydration
        /// <summary>
        /// placeholder for node value
        /// </summary>
        public object NodeValue { get; set; }
        #endregion

        #region Calculated Properties
        /// <summary>
        /// casts id to path 
        /// </summary>
        public GraphPath Path
        {
            get
            {
                return this.Id;
            }
        }
        #endregion

        #region IReconstable
        /// <summary>
        /// serializes node to string
        /// </summary>
        /// <returns></returns>
        public string Dehydrate()
        {
            var rv = TextDecorator.LengthEncodeList( this.TraversalIndex.ToString(), this.ValueManagerId, this.Id, this.Context);
            return rv;
        }
        /// <summary>
        /// hydrates node from string
        /// </summary>
        /// <param name="text"></param>
        public void Hydrate(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            var arr = TextDecorator.LengthDecodeList(text);
            Condition.Requires(arr).IsNotEmpty();
            Condition.Requires(arr).HasLength(4);
            this.TraversalIndex = int.Parse(arr[0]);
            this.ValueManagerId = arr[1];
            this.Id = arr[2];
            this.Context = arr[3];

            this.ValidateIsHydrated();
        }

        /// <summary>
        /// checks that the node's dehydrated parts are all non-null
        /// </summary>
        private void ValidateIsHydrated()
        {
            Condition.Requires(this.Context).IsNotNullOrEmpty();
            Condition.Requires(this.Id).IsNotNullOrEmpty();
            Condition.Requires(this.TraversalIndex).IsGreaterThan(-1);
            Condition.Requires(this.ValueManagerId).IsNotNullOrEmpty();
        }
        #endregion

        #region Query Methods
        /// <summary>
        /// given a nodestore finds the current node's parent node
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public GraphNode GetParentNode(IStoreOf<GraphNode> nodeStore)
        {
            GraphNode rv = null;

            //if we're on a root node, there is no parent
            if (this.Path.CurrentSegment is RootSegment)
                return null;

            var parentPath = this.Path.ParentPath;
            var matches = nodeStore.Search(SearchFilterOf<GraphNode>.NewOf((x) => { return x.Id.Equals(parentPath); }));
            rv = matches.FirstOrDefault();
            return rv;
        }
        /// <summary>
        /// gets all children of the current node who have a parent path equal to the current node's path
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public List<GraphNode> GetImmediateChildNodes(IStoreOf<GraphNode> nodeStore)
        {
            var path = this.Path.Path;
            var matches = nodeStore.Search(SearchFilterOf<GraphNode>.NewOf((x) => { return x.Path.ParentPath.Equals(path); }));
            return matches;
        }
        /// <summary>
        /// gets all children of the current node (those with paths that begin with the current node's path)
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public List<GraphNode> GetChildNodes(IStoreOf<GraphNode> nodeStore)
        {
            var path = this.Path;
            var matches = nodeStore.Search(SearchFilterOf<GraphNode>.NewOf((x) => { return x.Path.Path.StartsWith(path.Path); }));
            return matches;
        }
        #endregion

        #region Static Query Methods
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
        /// <summary>
        /// given a path and a nodestore, tries to locate a node
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public static GraphNode GetNode(GraphPath path, IStoreOf<GraphNode> nodeStore)
        {
            Condition.Requires(path).IsNotNull();
            Condition.Requires(nodeStore).IsNotNull();

            var node = nodeStore.Get<GraphNode>(path.Path);
            return node;
        }
        public static GraphNode GetRootNode(IStoreOf<GraphNode> nodeStore)
        {
            return GetNode(GraphPath.New(), nodeStore);
        }
        #endregion

        #region Static Fluent Methods
        public static GraphNode New(GraphPath path, object nodeValue, int traversalIndex, string valueMgrId, string serializedContext)
        {
            return new GraphNode(path, nodeValue, traversalIndex, valueMgrId, serializedContext);
        }
        /// <summary>
        /// returns a hydrated GraphNode from its dehydrated state.  Does NOT reconstitute the Node value 
        /// </summary>
        /// <param name="nodeText"></param>
        /// <returns></returns>
        public static GraphNode Parse(string nodeText)
        {
            var rv = new GraphNode();
            rv.Hydrate(nodeText);
            return rv;
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator GraphNode(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            return GraphNode.Parse(text);
        }
        public static implicit operator string(GraphNode obj)
        {
            if (obj == null)
                return null;

            return obj.Dehydrate();
        }
        #endregion

        #region Static List/Store  Serialization/Hydration
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
                lines.Add(node.Dehydrate());
            });
            var rv = TextDecorator.LengthEncodeList(lines.ToArray());
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

            var lines = TextDecorator.LengthDecodeList(storeText);

            var list = new List<GraphNode>();
            for (int i = 0; i < lines.Count; i = i + 1)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                string line = lines[i];
                var node = GraphNode.Parse(line);

                list.Add(node);
            }
            return list;
        }
        /// <summary>
        /// converts Node Store to a list of nodes and serializes it
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns></returns>
        public static string DehydrateNodeStore(IStoreOf<GraphNode> nodeStore)
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
            var nodeStore = NaturalInMemoryStore.New().DecorateWithIsOf<GraphNode>();
            var list = HydrateNodeList(storeText);
            nodeStore.SaveItems(list.ConvertListTo<IHasId,GraphNode>());
            return nodeStore;
        }
        #endregion
    }
}
