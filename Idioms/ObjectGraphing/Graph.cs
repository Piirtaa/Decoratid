using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Counting;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Idioms.Stringing;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Decoratid.Extensions;

namespace Decoratid.Idioms.ObjectGraphing
{
    /// <summary>
    /// a graph of objects converted into a storeOf GraphNode, and the ValueManager Chain of Responsibility that does
    /// the actual serialization of each node's value.  
    /// </summary>
    public interface IGraph
    {
        IStoreOf<GraphNode> NodeStore { get; }
        ValueManagerChainOfResponsibility ChainOfResponsibility { get; }
    }

    /// <summary>
    /// converts an object graph into an IGraph format
    /// </summary>
    public class Graph : IGraph, IStringable
    {
        #region Ctor
        private Graph(ValueManagerChainOfResponsibility chainOfResponsibility)
        {
        }
        #endregion

        #region Properties
        public IStoreOf<GraphNode> NodeStore { get; private set; }
        public ValueManagerChainOfResponsibility ChainOfResponsibility { get; set; }
        private Counter Counter { get; set; }
        private Func<object, GraphPath, bool> SkipFilter { get; set; }
        #endregion

        #region Calculated Properties
        public GraphNode RootNode
        {
            get
            {
                var root = this.NodeStore.GetRootNode();
                return root;
            }
        }
        #endregion

        #region Graphing Methods
        /// <summary>
        /// given the object to graph, builds a graph
        /// </summary>
        /// <param name="obj"></param>
        private void BuildGraph(object obj, Func<object, GraphPath, bool> skipFilter = null )
        {
            /*
             *  We walk each object in the graph and convert that into a GraphNode (ie. GraphPath + ManagedValue + Sequence) 
             * 
             */

            Condition.Requires(obj).IsNotNull();
            
            this.NodeStore = NaturalInMemoryStore.New().IsOf<GraphNode>();
            this.Counter = new Counter();
            this.SkipFilter = skipFilter;

            var rootPath = GraphPath.New();

            //build the node and recurse, maybe
            BuildNode(obj, rootPath);
        }

        /// <summary>
        /// the recursive call
        /// </summary>
        /// <param name="nodeValue"></param>
        /// <param name="nodePath"></param>
        /// <param name="nodeStore"></param>
        /// <param name="traversalIndex"></param>
        private void BuildNode(object nodeValue, GraphPath nodePath)
        {
            //skip if indicated
            if (this.SkipFilter != null && this.SkipFilter(nodeValue, nodePath))
                return;

            this.Counter.Increment();

            //get the manager for this node value
            var factory = this.ChainOfResponsibility.FindHandlingValueManager(nodeValue, this);
            Condition.Requires(factory).IsNotNull();

            //using the manager, serialize node value, and build the node
            var val = factory.DehydrateValue(nodeValue, this);
            
            var node = GraphNode.New(nodePath, nodeValue, this.Counter.Current, factory.Id, val);

            //save the node
            this.NodeStore.SaveItem(node);

            //only recurse if we're on a compound value
            if (!(factory is CompoundValueManager))
                return;

            //if the node is IEnumerable, recurse here
            if (nodeValue is IEnumerable && (nodeValue is string) == false)
            {
                IEnumerable objEnumerable = nodeValue as IEnumerable;

                EnumeratedSegmentType segType = EnumeratedSegmentType.None;
                if (nodeValue is IDictionary)
                {
                    segType = EnumeratedSegmentType.IDictionary;
                }
                else if (nodeValue is Stack)
                {
                    segType = EnumeratedSegmentType.Stack;
                }
                else if (nodeValue is Queue)
                {
                    segType = EnumeratedSegmentType.Queue;
                }
                else if (nodeValue is IList)
                {
                    segType = EnumeratedSegmentType.IList;
                }
                int index = 0;
                foreach (var each in objEnumerable)
                {
                    //build the path
                    var path = GraphPath.New(nodePath);
                    path.AddSegment(EnumeratedItemSegment.New(index, segType));

                    //build the node and recurse
                    BuildNode(each, path);
                    index++;
                }
            }
            else
            {
                //recurse the fields           
                var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(nodeValue.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (FieldInfo field in fields)
                {
                    //get field value
                    var obj = field.GetValue(nodeValue);

                    var path = GraphPath.New(nodePath);
                    path.AddSegment(GraphSegment.New(field.DeclaringType, field.Name));

                    //build the node and recurse
                    BuildNode(obj, path);

                }
            }

        }
        #endregion

        #region Reconstitution Methods
        /// <summary>
        /// given a Node Store, hydrates the value of each node, and wires the entire graph back together
        /// </summary>
        /// <param name="nodeStore"></param>
        /// <returns>the root node value</returns>
        private object ReconstituteFromNodeStore(IStoreOf<GraphNode> nodeStore)
        {
            Condition.Requires(nodeStore).IsNotNull();
            this.NodeStore = nodeStore;

            //iterate over all of the non-duplicate values and Reconste these nodes
            var nodes = NodeStore.GetAll();
            nodes.WithEach(node =>
            {
                if (node.ValueManagerId != DuplicateValueManager.ID)
                {
                    var mgr = this.ChainOfResponsibility.GetValueManagerById(node.ValueManagerId);
                    var obj = mgr.HydrateValue(node.Context, this);
                    node.NodeValue = obj;
                }
            });
            //wire the circ refs from the bottom up 
            nodes = nodes.OrderByDescending((x) => { return x.TraversalIndex; }).ToList();
            foreach (var node in nodes)
            {
                if (node.ValueManagerId == DuplicateValueManager.ID)
                {
                    var mgr = this.ChainOfResponsibility.GetValueManagerById(node.ValueManagerId);
                    var obj = mgr.HydrateValue(node, this);
                    node.NodeValue = obj;
                }
            }

            Dictionary<string, string> hasProcessed = new Dictionary<string, string>();

            //moving from bottom, wire children to parents
            foreach (var node in nodes)
            {
                //get the parent
                var parent = nodeStore.GetParentNode(node.Path);

                if (parent != null && hasProcessed.ContainsKey(parent.Id) == false)
                {
                    hasProcessed.Add(parent.Id, null);
                    WireParentToChildren(parent, nodeStore);
                }
            }

            //return root node
            var root = nodeStore.GetRootNode();
            return root.NodeValue;
        }
        /// <summary>
        /// given a parent node and a store of nodes, identifies the children of the parent and wires
        /// their values to the parent node's value
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeStore"></param>
        private void WireParentToChildren(GraphNode parentNode, IStoreOf<GraphNode> nodeStore)
        {
            Condition.Requires(parentNode).IsNotNull();
            Condition.Requires(nodeStore).IsNotNull();

            List<string> skipFilter = new List<string>() { NullValueManager.ID, DelegateValueManager.ID, PrimitiveValueManager.ID };
            if (skipFilter.Contains(parentNode.ValueManagerId))
                return;

            var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(parentNode.NodeValue.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var children = nodeStore.GetImmediateChildNodes(parentNode.Path).OrderBy((x) => { return x.Path.EnumeratedSegmentIndex; }).ToList();

            foreach (var each in children)
            {
                if (each.Path.IsEnumeratedSegment)
                {
                    //add the value to the parent
                    switch (each.Path.EnumeratedSegmentType)
                    {
                        case EnumeratedSegmentType.None:
                            break;
                        case EnumeratedSegmentType.IList:
                            IList list = parentNode.NodeValue as IList;
                            list.Add(each.NodeValue);
                            break;
                        case EnumeratedSegmentType.Queue:
                            Queue queue = parentNode.NodeValue as Queue;
                            queue.Enqueue(each.NodeValue);
                            break;
                        case EnumeratedSegmentType.Stack:
                            Stack stack = parentNode.NodeValue as Stack;
                            stack.Push(each.NodeValue);
                            break;
                        case EnumeratedSegmentType.IDictionary:
                            IDictionary dict = parentNode.NodeValue as IDictionary;
                            DictionaryEntry de = (DictionaryEntry)each.NodeValue;
                            dict.Add(de.Key, de.Value);
                            break;
                    }
                }
                else
                {
                    //get the matching field
                    GraphSegment gSeg = each.Path.CurrentSegment as GraphSegment;
                    var matches = fields.Filter((x) => { return x.DeclaringType == gSeg.DeclaringType && x.Name.Equals(gSeg.SegmentName); });
                    Condition.Requires(matches).IsNotNull().HasLength(1);

                    var fi = matches.First();
                    fi.SetValue(parentNode.NodeValue, each.NodeValue);
                }
            }
        }
        #endregion

        #region IStringable

        public string GetValue()
        {
            var storeText = this.NodeStore.DehydrateNodeStore();
            var managerText = this.ChainOfResponsibility.GetValue();
            string rv = LengthEncoder.LengthEncodeList(managerText, storeText);
            return rv;
        }

        public void Parse(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            var arr = LengthEncoder.LengthDecodeList(text);
            Condition.Requires(arr).HasLength(2);
            var storeText = arr[1];
            var managerText = arr[0];

            var set = ValueManagerChainOfResponsibility.New();
            set.Parse(managerText);
            this.ChainOfResponsibility = set;
            var store = NodeStoreUtil.HydrateNodeStore(storeText);
            this.ReconstituteFromNodeStore(store);
        }
        #endregion

        #region Static Fluent
        public static Graph NewDefault()
        {
            Graph graph = new Graph(ValueManagerChainOfResponsibility.NewDefault());
            return graph;
        }
        public static Graph New(ValueManagerChainOfResponsibility managerSet)
        {
            Graph graph = new Graph(managerSet);
            return graph;
        }
        /// <summary>
        /// converts the provided object into a graph
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="valueManagers">if null, the Default managers are used</param>
        /// <returns></returns>
        public static Graph Build(object obj, ValueManagerChainOfResponsibility managerSet, Func<object, GraphPath, bool> skipFilter = null)
        {
            Graph graph = new Graph(managerSet);
            graph.BuildGraph(obj, skipFilter);
            return graph;
        }

        /// <summary>
        /// hydrates the graph from a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="valueManagers">if null, the Default managers are used</param>
        /// <returns></returns>
        public static Graph Parse(string text, ValueManagerChainOfResponsibility managerSet)
        {
            Graph graph = new Graph(managerSet);
            graph.Parse(text);
            return graph;
        }
        #endregion


    }
}
