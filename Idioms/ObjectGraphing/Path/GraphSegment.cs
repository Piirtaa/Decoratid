using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Path
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public sealed class GraphSegment : IGraphSegment
    {
        public const string PREFIX = "(";
        public const string SUFFIX = ")";
        public const string DELIM = ",";

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
                var list = StringableList.New().Delimit(PREFIX, DELIM, SUFFIX);
                list.Add(this.DeclaringType.Name);
                list.Add(this.SegmentName);
                return list.GetValue();
            }
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator GraphSegment(string text)
        {
            var list = StringableList.New().Delimit(PREFIX, DELIM, SUFFIX);
            list.Parse(text);

            string declaringTypeName = list[0];
            string segmentName = list[1];

            var types = TheTypeLocator.Instance.Locator.Locate((x) => { return x.Name == declaringTypeName; });
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
