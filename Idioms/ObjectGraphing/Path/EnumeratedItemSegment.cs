using CuttingEdge.Conditions;
using Decoratid.TypeLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.ObjectGraph.Path
{
    /// <summary>
    /// describes the type of collection the segment is part of
    /// </summary>
    public enum EnumeratedSegmentType
    {
        None,
        IList,
        IDictionary,
        Queue,
        Stack
    }

    /// <summary>
    /// describes segment from the parent node to one of its enumerable items
    /// </summary>
    /// <remarks>
    /// dehydrates to [{index},{IList,IDictionary,Queue,Stack}]
    /// </remarks>
    public sealed class EnumeratedItemSegment : IGraphSegment
    {
        public const string PREFIX = "[";

        #region Ctor
        private EnumeratedItemSegment(int index, EnumeratedSegmentType segType)
        {
            Condition.Requires(index).IsGreaterThan(-1);
            this.Index = index;
            this.SegmentType = segType;
        }
        #endregion

        #region Properties
        public int Index { get; private set; }
        public EnumeratedSegmentType SegmentType {get; private set;}
        #endregion

        #region IGraphPath
        public string Path
        {
            get
            {
                return string.Format("[{0},{1}]", this.Index, this.SegmentType);
            }
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator EnumeratedItemSegment(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            Condition.Requires(text).StartsWith("[");
            Condition.Requires(text).EndsWith("]");

            string scrubText = text.Substring(1,text.Length - 2);
            var arr = scrubText.Split(',');
            Condition.Requires(arr).HasLength(2);
            int index =  int.Parse(arr[0]);
            EnumeratedSegmentType segType = (EnumeratedSegmentType)Enum.Parse(typeof(EnumeratedSegmentType), arr[1]);
            EnumeratedItemSegment rv = new EnumeratedItemSegment(index, segType);
            return rv;
        }
        public static implicit operator string(EnumeratedItemSegment obj)
        {
            if (obj == null)
                return null;

            return obj.Path;
        }
        #endregion

        #region Fluent Static
        public static EnumeratedItemSegment New(int index, EnumeratedSegmentType segType)
        {
            return new EnumeratedItemSegment(index, segType);
        }
        #endregion
    }
}
