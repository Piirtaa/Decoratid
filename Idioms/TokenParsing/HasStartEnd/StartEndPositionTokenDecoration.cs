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

namespace Decoratid.Idioms.TokenParsing.HasStartEnd
{
    /// <summary>
    /// a token that tracks where it was parsed from
    /// </summary>
    public interface IStartEndPositionalToken<T> : IToken<T>
    {
        int StartPos { get; }
        int EndPos { get; }
    }
    /// <summary>
    /// decorates with positional tracking
    /// </summary>
    [Serializable]
    public class StartEndPositionTokenDecoration<T> : TokenDecorationBase<T>, IStartEndPositionalToken<T>
    {
        #region Ctor
        public StartEndPositionTokenDecoration(IToken<T> decorated, int startPos, int endPos)
            : base(decorated)
        {
            Condition.Requires(startPos).IsGreaterOrEqual(0).IsLessThan(endPos);
            this.StartPos = startPos;
            this.EndPos = endPos;
        }
        #endregion

        #region Fluent Static
        public static StartEndPositionTokenDecoration<T> New(IToken<T> decorated, int startPos, int endPos)
        {
            return new StartEndPositionTokenDecoration<T>(decorated, startPos, endPos);
        }
        #endregion

        #region ISerializable
        protected StartEndPositionTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public int StartPos { get; private set; }
        public int EndPos { get; private set; }
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new StartEndPositionTokenDecoration<T>(thing, this.StartPos, this.EndPos);
        }
        #endregion
    }

    public static class StartEndPositionDecorationExtensions
    {
        public static StartEndPositionTokenDecoration<T> HasStartEnd<T>(this IToken<T> thing, int startPos, int endPos)
        {
            Condition.Requires(thing).IsNotNull();
            return new StartEndPositionTokenDecoration<T>(thing, startPos, endPos);
        }
    }
}
