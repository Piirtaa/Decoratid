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
    /// describes segment from the parent node to one of its enumerable items
    /// </summary>
    /// <remarks>
    /// dehydrates to #{index}
    /// </remarks>
    public sealed class EnumeratedItemSegment : IGraphSegment
    {
        public const string PREFIX = "#";


        #region Ctor
        private EnumeratedItemSegment(int index)
        {
            Condition.Requires(index).IsGreaterThan(-1);
            this.Index = index;
        }
        #endregion

        #region Properties
        public int Index { get; private set; }
        #endregion

        #region IGraphPath
        public string Path
        {
            get
            {
                return PREFIX + Index;
           }
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator EnumeratedItemSegment(string text)
        {
            Condition.Requires(text).StartsWith(PREFIX);
            return new EnumeratedItemSegment(text.Substring(1).ConvertToInt());
        }
        public static implicit operator string(EnumeratedItemSegment obj)
        {
            if (obj == null)
                return null;

            return obj.Path;
        }
        #endregion

        #region Fluent Static
        public static EnumeratedItemSegment New(int index)
        {
            return new EnumeratedItemSegment(index);
        }
        #endregion
    }
}
