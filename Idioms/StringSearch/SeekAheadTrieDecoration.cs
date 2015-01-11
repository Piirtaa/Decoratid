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
    /// has seekahead algorithm - evaluates the trie at the given index and filters matches out by advancing the cursor 
    /// (ie. seeking ahead) until it can't search no more.  
    /// </summary>
    public interface ISeekAheadTrie : ITrie
    {
        /// <summary>
        /// for the provided index position, examines the trie to see if it can handle the character, and moves through
        /// the trie graph until it no longer matches.  returns list to account for situation where matches share a common suffix.
        /// </summary>
        List<StringSearchMatch> FindMatchesAtPosition(int idx, string text, out int graspLengthOUT);
    }

    [Serializable]
    public class SeekAheadTrieDecoration : StringSearcherDecorationBase, ISeekAheadTrie
    {
        #region Ctor
        public SeekAheadTrieDecoration(ITrie decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected SeekAheadTrieDecoration(SerializationInfo info, StreamingContext context)
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
            List<StringSearchMatch> rv = new List<StringSearchMatch>();
            graspLengthOUT = 0;

            //basic cursor position validation
            int maxIndex = text.Length - 1;

            if (maxIndex < idx)
                return rv;

            graspLengthOUT = 1;//we're at least processing 1 character
            var currentIdx = idx;
            var currentNode = this.Root;
            var currentChar = text[currentIdx];

            //do the first test in the loop
            currentNode = currentNode[currentChar];
            if (currentNode == null)
                return rv;

            var queue = new Queue<Tuple<int, char, ITrieNode>>();

            //define function to increment position, and update currentIdx, currentChar, and queue it up
            //implicit is the currentNode position has ALREADY been incremented at this point
            Func<bool> enqueue = () =>
            {
                //get the next char
                currentIdx++;
                //validate position
                if (maxIndex < currentIdx)
                    return false;

                currentChar = text[currentIdx];
                queue.Enqueue(new Tuple<int, char, ITrieNode>(currentIdx, currentChar, currentNode));
                return true;
            };

            enqueue();

            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                currentNode = tuple.Item3;
                currentChar = tuple.Item2;
                currentIdx = tuple.Item1;

                //if we're matching, return it
                if (currentNode.HasWord)
                {
                    var match = StringSearchMatch.New(currentIdx - 1 - currentNode.Id.Length, currentNode.Id, currentNode.Value);
                    rv.Add(match);

                    //update grasp length
                    var matchGraspLength = match.Word.Length;
                    if (graspLengthOUT < matchGraspLength)
                        graspLengthOUT = matchGraspLength;
                }

                //does the trie even handle (aka lift, bro) this particular character?
                currentNode = currentNode[currentChar];
                if (currentNode != null)
                {
                    enqueue();
                }
            }

            return rv;
        }

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
            return new SeekAheadTrieDecoration(thing as ITrie);
        }
        #endregion
    }

    public static class SeekAheadTrieDecorationExtensions
    {
        public static SeekAheadTrieDecoration UseSeekAhead(this ITrie decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new SeekAheadTrieDecoration(decorated);
        }

    }
}
