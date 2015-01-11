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
    /// slices up each index to process as a separate job and parallelize the seekahead.
    /// by far the most performant trie decoration. 
    /// </summary>
    public interface IParallelingTrie : ISeekAheadTrie
    {
    }

    [Serializable]
    public class ParallelingTrieDecoration : TrieDecorationBase, IParallelingTrie
    {
        #region Ctor
        public ParallelingTrieDecoration(ISeekAheadTrie decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ParallelingTrieDecoration(SerializationInfo info, StreamingContext context)
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
            return (this.Decorated as ISeekAheadTrie).FindMatchesAtPosition(idx, text, out graspLengthOUT);
        }
        public override List<StringSearchMatch> FindMatches(string text)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            //var loop = Parallel.For(0, maxIndex, (x) =>
            //{
            //    int grasp;
            //    var list = (this.Decorated as ISeekAheadTrie).FindMatchesAtPosition(x, text, out grasp);
            //    rv.AddRange(list);
            //});

            Parallel.For(0, maxIndex, () => new List<StringSearchMatch>(),
                (x, loop, subList) =>
                {
                    int grasp;
                    var list = (this.Decorated as ISeekAheadTrie).FindMatchesAtPosition(x, text, out grasp);
                    subList.AddRange(list);
                    return subList;
                },
                (x) => { rv.AddRange(x); }
            );

            return rv;
        }
        public override IDecorationOf<ITrie> ApplyThisDecorationTo(ITrie thing)
        {
            return new ParallelingTrieDecoration(thing as ISeekAheadTrie);
        }
        #endregion
    }

    public static class ParallelingTrieDecorationExtensions
    {
        /// <summary>
        /// decorates a seekaheadtrie with some bitchin parallelization perf improvements, y'all
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ParallelingTrieDecoration Paralleling(this ISeekAheadTrie decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ParallelingTrieDecoration(decorated);
        }

    }
}
