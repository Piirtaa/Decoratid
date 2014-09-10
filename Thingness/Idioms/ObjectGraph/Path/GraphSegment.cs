using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.TypeLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.ObjectGraph.Path
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public sealed class GraphSegment : IGraphSegment
    {
        public const string PREFIX = "(";

        #region Ctor
        private GraphSegment(Type declaringType, string segmentName)
        {
            this.DeclaringType = declaringType;
            this.SegmentName = segmentName;
        }
        #endregion

        #region Key Properties
        /// <summary>
        /// the type that declares the field
        /// </summary>
        public Type DeclaringType { get; private set; }
        /// <summary>
        /// the name of the field that holds the node(s).  
        /// </summary>
        public string SegmentName { get; private set; }
        #endregion

        #region IGraphPath
        public string Path
        {
            get
            {
                return string.Format("({0}){1}", this.DeclaringType.Name, this.SegmentName);
            }
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator GraphSegment(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            Condition.Requires(text).StartsWith(PREFIX);

            string declaringTypeName = text;
            string segmentName = null;
            string[] split = new string[] { "(", ")" };

            var arr = text.Split(split, StringSplitOptions.RemoveEmptyEntries);
            declaringTypeName = arr[0];
            segmentName = arr[1];

            var types = TypeLocator.Instance.Locate((x) => { return x.Name == declaringTypeName; });
            Condition.Requires(types).IsNotNull().IsNotEmpty();
            Condition.Requires(segmentName).IsNotNullOrEmpty();

            GraphSegment rv = new GraphSegment(types.FirstOrDefault(), segmentName);
            return rv;
        }
        public static implicit operator string(GraphSegment obj)
        {
            if (obj == null)
                return null;

            return obj.Path;
        }
        #endregion

        #region Fluent Static
        public static GraphSegment New(Type declaringType, string segmentName)
        {
            return new GraphSegment(declaringType, segmentName);
        }
        #endregion
    }
}
