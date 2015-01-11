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
    public interface INonOverlappingTrie : ISeekAheadTrie
    {
    }

    [Serializable]
    public class NonOverlappingTrieDecoration : StringSearcherDecorationBase, INonOverlappingTrie
    {
        #region Ctor
        public NonOverlappingTrieDecoration(ISeekAheadTrie decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected NonOverlappingTrieDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public ITrieNode Root { get { return (this.Decorated as ITrie).Root; } }
        public ITrieNode this[string path] { get { return (this.Decorated as ITrie)[path]; } set { (this.Decorated as ITrie)[path] = value; } }

        public List<StringSearchMatch> FindMatchesAtPosition(int idx, string text, out int graspLengthOUT)
        {
            return (this.Decorated as ISeekAheadTrie).FindMatchesAtPosition(idx, text, out graspLengthOUT);
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
                var list = (this.Decorated as ISeekAheadTrie).FindMatchesAtPosition(i, text, out grasp);
                rv.AddRange(list);
                i = i + grasp; //move ahead

            }
            return rv;
        }
        public override IDecorationOf<IStringSearcher> ApplyThisDecorationTo(IStringSearcher thing)
        {
            return new NonOverlappingTrieDecoration(thing as ISeekAheadTrie);
        }
        #endregion
    }

    public static class NonOverlappingTrieDecorationExtensions
    {
        public static NonOverlappingTrieDecoration NonOverlapping(this ISeekAheadTrie decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new NonOverlappingTrieDecoration(decorated);
        }

    }
}
