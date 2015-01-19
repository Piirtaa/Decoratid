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
    /// modifies behaviour of byposition search to move the cursor forward on every single match
    /// </summary>
    public interface IPostMatchCursorMovingStringSearcher : IByPositionStringSearcher
    {
        Func<List<StringSearchMatch>, int, string, int> PostMatchCursorMoveLogic { get; }
    }

    /// <summary>
    /// modifies behaviour of byposition search to move the cursor forward on every single match
    /// </summary>
    [Serializable]
    public class PostMatchCursorMovingStringSearcherDecoration : StringSearcherDecorationBase, IPostMatchCursorMovingStringSearcher
    {
        #region Ctor
        public PostMatchCursorMovingStringSearcherDecoration(IByPositionStringSearcher decorated,
            Func<List<StringSearchMatch>, int, string, int> postMatchCursorMoveLogic)
            : base(decorated)
        {
            Condition.Requires(postMatchCursorMoveLogic).IsNotNull();
            this.PostMatchCursorMoveLogic = postMatchCursorMoveLogic;
        }
        #endregion

        #region ISerializable
        protected PostMatchCursorMovingStringSearcherDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public Func<List<StringSearchMatch>, int,string, int> PostMatchCursorMoveLogic { get; private set; }
        #endregion

        #region Overrides
        public List<StringSearchMatch> FindMatchesAtPosition(int idx, string text, out int graspLengthOUT)
        {
            int graspLength = 0;
            //do the positional match
            var rv = (this.Decorated as IByPositionStringSearcher).FindMatchesAtPosition(idx, text, out graspLength);
         
            //do the post match move
            graspLength = this.PostMatchCursorMoveLogic(rv, graspLength, text);//move the cursor ahead according to some silly strategy
            graspLengthOUT = graspLength;
            return rv;
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
                i = i + grasp;
            }
            return rv;
        }
        public override IDecorationOf<IStringSearcher> ApplyThisDecorationTo(IStringSearcher thing)
        {
            return new PostMatchCursorMovingStringSearcherDecoration(thing as IByPositionStringSearcher, this.PostMatchCursorMoveLogic);
        }
        #endregion
    }

    public static class PostMatchCursorMovingStringSearcherDecorationExtensions
    {
        /// <summary>
        /// applies non-overlapping search alg to IByPositionStringSearcher
        /// </summary>
        public static PostMatchCursorMovingStringSearcherDecoration PostMatchCursorMoving(this IByPositionStringSearcher decorated,
            Func<List<StringSearchMatch>, int, string, int> postMatchCursorMoveLogic)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PostMatchCursorMovingStringSearcherDecoration(decorated, postMatchCursorMoveLogic);
        }

    }
}
