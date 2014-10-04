using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.Stringing;

namespace Decoratid.Idioms.ObjectGraphing.Path
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
        public const string SUFFIX = "]";
        public const string DELIM = ",";

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
                var list = StringableList.New().Delimit(PREFIX, DELIM, SUFFIX);
                list.Add(this.Index.ToString());
                list.Add(this.SegmentType.ToString());
                return list.GetValue();
           }
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator EnumeratedItemSegment(string text)
        {
            var list = StringableList.New().Delimit(PREFIX, DELIM, SUFFIX);
            list.Parse(text);

            int index =  int.Parse(list[0]);
            EnumeratedSegmentType segType = (EnumeratedSegmentType)Enum.Parse(typeof(EnumeratedSegmentType), list[1]);
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
