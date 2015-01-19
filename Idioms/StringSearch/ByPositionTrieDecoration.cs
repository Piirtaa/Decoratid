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
    /// a trie that has a byposition search
    /// </summary>
    public interface IByPositionTrie : ITrie, IByPositionStringSearcher
    {
    }
    /// <summary>
    /// a trie decoration that has a byposition search
    /// </summary>
    [Serializable]
    public class ByPositionTrieDecoration : StringSearcherDecorationBase, IByPositionTrie
    {
        #region Ctor
        public ByPositionTrieDecoration(ITrie decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ByPositionTrieDecoration(SerializationInfo info, StreamingContext context)
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
        /// <summary>
        /// for the provided index position, examines the trie to see if it can handle the character, and moves through
        /// the trie graph until it no longer matches.  returns list to account for situation where matches share a common suffix.
        /// </summary>
        /// <param name="trie"></param>
        /// <param name="idx"></param>
        /// <param name="text"></param>
        /// <param name="graspLengthOUT">the number of characters processed</param>
        /// <returns></returns>
        public List<StringSearchMatch> FindMatchesAtPosition(int idx, string text, out int graspLengthOUT)
        {
            return TrieLogic.FindMatchesAtPosition(this, idx, text, out graspLengthOUT);
        }

        /// <summary>
        /// implements FindMatches using a FindMatchesAtPosition aggregation
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public override List<StringSearchMatch> FindMatches(string text)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            for (int i = 0; i <= maxIndex; i++)
            {
                int grasp;
                var list = FindMatchesAtPosition(i, text, out grasp);
                rv.AddRange(list);
            }
            return rv;
        }
        public override IDecorationOf<IStringSearcher> ApplyThisDecorationTo(IStringSearcher thing)
        {
            return new ByPositionTrieDecoration(thing as ITrie);
        }
        #endregion
    }

    public static class ByPositionTrieDecorationExtensions
    {
        /// <summary>
        /// gives the trie positional search capability, and modifies the default trie search
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ByPositionTrieDecoration HasPositionalSearch(this ITrie decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ByPositionTrieDecoration(decorated);
        }

    }
}
