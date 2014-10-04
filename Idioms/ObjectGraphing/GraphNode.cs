using CuttingEdge.Conditions;
using Decoratid.Core.Contextual;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Storidioms.StoreOf;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing
{
    /// <summary>
    /// GraphPath + ManagedValue + Sequence
    /// </summary>
    public class GraphNode : ContextualId<string, string>, IStringable
    {
        #region Ctor
        private GraphNode() :base() { }
        private GraphNode(GraphPath path, object nodeValue, int traversalIndex, string valueMgrId, string serializedNodeValue)
            : base()
        {
            Condition.Requires(path).IsNotNull();
            Condition.Requires(traversalIndex).IsGreaterThan(-1);

            //validate the serialized value doesn't have reserved characters
            this.TraversalIndex = traversalIndex;
            this.ValueManagerId = valueMgrId;
            this.Id = path.Path;
            this.Context = serializedNodeValue;
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

        #region IStringable
        public string GetValue()
        {
            var rv = LengthEncoder.LengthEncodeList(this.TraversalIndex.ToString(), this.ValueManagerId, this.Id, this.Context);
            return rv;
        }
        public void Parse(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            var arr = LengthEncoder.LengthDecodeList(text);
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

        #region Static Fluent Methods
        public static GraphNode New(GraphPath path, object nodeValue, int traversalIndex, string valueMgrId, string serializedContext)
        {
            return new GraphNode(path, nodeValue, traversalIndex, valueMgrId, serializedContext);
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator GraphNode(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            var rv = new GraphNode();
            rv.Parse(text);
            return rv;
        }
        public static implicit operator string(GraphNode obj)
        {
            if (obj == null)
                return null;

            var rv= obj.GetValue();
            return rv;
        }
        #endregion
    }
}
