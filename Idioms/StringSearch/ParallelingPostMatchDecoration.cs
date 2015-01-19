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
    public interface IParallelingPostMatch : IPostMatchCursorMovingStringSearcher
    {
    }


    [Serializable]
    public class ParallelingPostMatchDecoration : StringSearcherDecorationBase, IParallelingPostMatch
    {
        #region Ctor
        public ParallelingPostMatchDecoration(IPostMatchCursorMovingStringSearcher decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ParallelingPostMatchDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public Func<List<StringSearchMatch>, int, string, int> PostMatchCursorMoveLogic { get { return (this.Decorated as IPostMatchCursorMovingStringSearcher).PostMatchCursorMoveLogic; } }
        #endregion

        #region Overrides
        public List<StringSearchMatch> FindMatchesAtPosition(int idx, string text, out int graspLengthOUT)
        {
            return (this.Decorated as IPostMatchCursorMovingStringSearcher).FindMatchesAtPosition(idx, text, out graspLengthOUT);
        }
        private class SegmentUoW
        {
            public SegmentUoW(SegmentUoW lastSeg, int startIndex, int stopIndex)
            {

            }
            public int StartIndex { get; }
            public int StopIndex { get; }
            public SegmentUoW Previous { get; }
            public SegmentUoW Next { get; }
        }
        public override List<StringSearchMatch> FindMatches(string text)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            //ok, so how to parallelize the cursor moving:  
            //slice up the work into batches/segments
            //

            for (int i = 0; i <= maxIndex; )
            {
                int grasp;
                var list = (this.Decorated as IByPositionStringSearcher).FindMatchesAtPosition(i, text, out grasp);
                rv.AddRange(list);
                i = i + grasp;
            }
            return rv;

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

        }
        public override IDecorationOf<IStringSearcher> ApplyThisDecorationTo(IStringSearcher thing)
        {
            return new ParallelingPostMatchDecoration(thing as IByPositionStringSearcher, this.PostMatchCursorMoveLogic);
        }
        #endregion
    }

    public static class ParallelingPostMatchDecorationExtensions
    {
        public static ParallelingPostMatchDecoration ParallelPostMatch(this IPostMatchCursorMovingStringSearcher decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ParallelingPostMatchDecoration(decorated);
        }
    }
}
