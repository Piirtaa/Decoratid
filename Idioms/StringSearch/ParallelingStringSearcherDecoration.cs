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
    /// slices up each index to process as a separate job and parallelize the IByPositionStringSearcher.
    /// by far the most performant trie decoration. 
    /// </summary>
    public interface IParallelingStringSearcher : IByPositionStringSearcher
    {
    }
    /// <summary>
    /// slices up each index to process as a separate job and parallelize the IByPositionStringSearcher.
    /// by far the most performant trie decoration. 
    /// </summary>
    [Serializable]
    public class ParallelingStringSearcherDecoration : StringSearcherDecorationBase, IByPositionStringSearcher
    {
        #region Ctor
        public ParallelingStringSearcherDecoration(IByPositionStringSearcher decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ParallelingStringSearcherDecoration(SerializationInfo info, StreamingContext context)
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
                    var list = (this.Decorated as IByPositionStringSearcher).FindMatchesAtPosition(x, text, out grasp);
                    subList.AddRange(list);
                    return subList;
                },
                (x) => { rv.AddRange(x); }
            );

            return rv;
        }
        public override IDecorationOf<IStringSearcher> ApplyThisDecorationTo(IStringSearcher thing)
        {
            return new ParallelingStringSearcherDecoration(thing as IByPositionStringSearcher);
        }
        #endregion
    }

    public static class ParallelingTrieDecorationExtensions
    {
        /// <summary>
        /// decorates a IByPositionStringSearcher with some bitchin parallelization perf improvements, y'all
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ParallelingStringSearcherDecoration Paralleling(this IByPositionStringSearcher decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ParallelingStringSearcherDecoration(decorated);
        }

    }
}
