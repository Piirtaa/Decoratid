using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Thingness.A4orm
{
    /// <summary>
    /// in the graph of objects we are flattening, each object is a node.  if that object has an id (eg. implements IHasId)
    /// then we use that to identify the object. if that object does not, we autogenerate an id.  In this way we give every node
    /// an id.
    /// </summary>
    public class ObjectGraphNode
    {
        #region Construction/Destruction
        public ObjectGraphNode(object obj)
        {
            this.Self = obj;

            //if we don't have an IHasId object instance, we autogenerate
            if (obj != null && obj is IHasId)
            {
                IHasId id = obj as IHasId;
                this.Id = id.Id;
                this.IsAutoId = false;
            }
            else
            {
                this.Id = Guid.NewGuid().ToString();
                this.IsAutoId = true;
            }
        }
        #endregion

        #region Properties
        public object Self { get; private set; }
        public IdRef Id { get; private set; }
        public bool IsAutoId { get; private set; }
        #endregion
    }

    /// <summary>
    /// Uniquely defines a relationship between 2 nodes.  In an object graph a property is a child node of the parent object - this
    /// relationship is defined here.  
    /// </summary>
    public class ObjectGraphRelationship : IHasId
    {
        #region Ctor
        public ObjectGraphRelationship(IdRef parent, IdRef child, string propertyName, int? index)
        {
            this.Parent = parent;
            this.Child = child;
            this.PropertyName = propertyName;
            this.Index = index;
        }
        #endregion

        #region Properties
        public IdRef Parent { get; private set; }
        public IdRef Child { get; private set; }
        public string PropertyName { get; private set; }
        public int? Index { get; private set; }
        public bool IsTopmostNode
        {
            get
            {
                return string.IsNullOrEmpty(this.PropertyName);
            }
        }
        #endregion

        #region Derived Properties
        public string Id
        {
            get
            {
                return string.Format("{0}:{1}:{2}:{3}", this.Parent, this.Child, this.PropertyName, this.Index.GetValueOrDefault(-1));
            }
        }
        #endregion
    }

    public class ObjectGraph 
    {


    }

    public class GraphWalker
    {
        #region Static Methods

        public static void GetNodes(object topmostObj, Func<object, bool> includeNodeFilter,
            Func<object, List<string>> getExcludePropertiesStrategy,
            Func<object, List<string>, Dictionary<string, object>> getNodeDictionaryStrategy,
            Func<object, string> idStrategy,
            out Dictionary<string, GraphWalkNode> nodes, out Dictionary<string, GraphWalkRelationship> rels)
        {
            if (topmostObj == null) { throw new ArgumentNullException("topmostObj"); }
            if (includeNodeFilter == null) { throw new ArgumentNullException("includeNodeFilter"); }
            if (idStrategy == null) { throw new ArgumentNullException("idStrategy"); }

            Dictionary<string, GraphWalkNode> nodeBag = new Dictionary<string, GraphWalkNode>();
            Dictionary<string, GraphWalkRelationship> relsBag = new Dictionary<string, GraphWalkRelationship>();
            WalkNode(topmostObj, topmostObj, string.Empty, false,
                includeNodeFilter, getExcludePropertiesStrategy, getNodeDictionaryStrategy, idStrategy, nodeBag, relsBag);

            nodes = nodeBag;
            rels = relsBag;
        }

        private static void WalkNode(object parentObj, object nodeObj, string nodePropertyName, bool isPropertyACollection,
            Func<object, bool> includeNodeFilter,
            Func<object, List<string>> getExcludePropertiesStrategy,
            Func<object, List<string>, Dictionary<string, object>> getNodeDictionaryStrategy,
            Func<object, string> idStrategy,
            Dictionary<string, GraphWalkNode> nodeBag, Dictionary<string, GraphWalkRelationship> relBag)
        {
            //skip if the node is null (there is nothing to walk)
            if (nodeObj == null) { return; }

            //skip if the node is a value type or a string
            Type nodeObjType = nodeObj.GetType();
            if (nodeObjType.IsValueType || nodeObjType == typeof(string)) { return; }

            //if the node doesn't pass the filter skip it
            if (includeNodeFilter != null && includeNodeFilter(nodeObj) == false)
            {
                return;
            }

            //get the node's id
            string currentId = idStrategy(nodeObj);

            //if the node has no id, skip it
            if (string.IsNullOrEmpty(currentId))
            {
                return;
            }

            //create the node and register it if it hasn't already been registered
            if (!nodeBag.ContainsKey(currentId))
            {
                GraphWalkNode currentNode = new GraphWalkNode(nodeObj, currentId);
                nodeBag.Add(currentId, currentNode);
            }

            //get the parent's id
            string parentId = idStrategy(parentObj);

            //build the rel
            GraphWalkRelationship rel = new GraphWalkRelationship(parentId, currentId, nodePropertyName, isPropertyACollection);

            //if the rel has already been processed, or is currently being processed, skip it
            if (relBag.ContainsKey(rel.Id))
            {
                return;
            }

            //register the rel
            relBag.Add(rel.Id, rel);

            //determine which properties to walk
            List<string> excludeProperties = new List<string>();
            if (getExcludePropertiesStrategy != null)
            {
                excludeProperties = getExcludePropertiesStrategy(nodeObj);
            }

            //get the property values
            Dictionary<string, object> propVals = null;
            if (getNodeDictionaryStrategy != null)
            {
                propVals = getNodeDictionaryStrategy(nodeObj, excludeProperties);
            }
            else
            {
                //if no strategy is defined to get a dictionary of the node's properties, we use the plumber
                propVals = PlumberManager.Instance.GetObjectAsDictionary(nodeObj, excludeProperties);
            }

            if (propVals != null && propVals.Count > 0)
            {
                //recurse
                foreach (KeyValuePair<string, object> each in propVals)
                {
                    try
                    {
                        //if the property is null, go to the next one
                        if (each.Value == null)
                        {
                            continue;
                        }

                        //if it's IEnumerable, iterate thru
                        if (each.Value is IEnumerable)
                        {
                            List<object> propValList = EnumeratorUtil.GetList(each.Value);

                            if (propValList != null && propValList.Count > 0)
                            {
                                for (int i = 0; i < propValList.Count; i++)
                                {
                                    object propValListItem = propValList[i];

                                    //walk the node
                                    WalkNode(nodeObj, propValListItem, each.Key, true,
                                        includeNodeFilter, getExcludePropertiesStrategy, getNodeDictionaryStrategy
                                    , idStrategy, nodeBag, relBag);
                                }
                            }
                        }
                        else
                        {
                            //walk the node
                            WalkNode(nodeObj, each.Value, each.Key, false,
                                includeNodeFilter, getExcludePropertiesStrategy, getNodeDictionaryStrategy, idStrategy, nodeBag, relBag);
                        }
                    }
                    catch
                    {
                        continue;
                    }

                }
            }
        }
        #endregion
    }
}
