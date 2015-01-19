using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// uses non-overlapping search.  searches index by index, but will skip cursor ahead by the length of the last
    /// index match, thus avoiding overlapping matches (if we know the text we're searching for looks like this, then
    /// this decoration might be useful)
    /// </summary>
    public interface INonOverlappingStringSearcher : IByPositionStringSearcher
    {
    }

    /// <summary>
    /// applies non-overlapping search alg to IByPositionStringSearcher
    /// </summary>
    [Serializable]
    public class NonOverlappingStringSearcherDecoration : StringSearcherDecorationBase, INonOverlappingStringSearcher
    {
        #region Ctor
        public NonOverlappingStringSearcherDecoration(IByPositionStringSearcher decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected NonOverlappingStringSearcherDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public List<StringSearchMatch> FindMatchesAtPosition(int idx, string text, out int graspLengthOUT)
        {
            return (this.Decorated as IByPositionStringSearcher).FindMatchesAtPosition(idx, text, out graspLengthOUT);
        }

        public override List<StringSearchMatch> FindMatches(string text)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            for (int i = 0; i <= maxIndex; )
            {
                int grasp;
                var list = (this.Decorated as IByPositionStringSearcher).FindMatchesAtPosition(i, text, out grasp);
                rv.AddRange(list);
                i = i + grasp; //move ahead

            }
            return rv;
        }
        public override IDecorationOf<IStringSearcher> ApplyThisDecorationTo(IStringSearcher thing)
        {
            return new NonOverlappingStringSearcherDecoration(thing as IByPositionStringSearcher);
        }
        #endregion
    }

    public static class NonOverlappingTrieDecorationExtensions
    {
        /// <summary>
        /// applies non-overlapping search alg to IByPositionStringSearcher
        /// </summary>
        public static NonOverlappingStringSearcherDecoration NonOverlapping(this IByPositionStringSearcher decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new NonOverlappingStringSearcherDecoration(decorated);
        }

    }
}
