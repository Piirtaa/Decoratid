﻿using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// a token that tracks where it was parsed from
    /// </summary>
    public interface IStartEndPositionalToken : IToken
    {
        int StartPos { get; }
        int EndPos { get; }
    }
    /// <summary>
    /// decorates with positional tracking
    /// </summary>
    [Serializable]
    public class StartEndPositionTokenDecoration : TokenDecorationBase, IStartEndPositionalToken
    {
        #region Ctor
        public StartEndPositionTokenDecoration(IToken decorated, int startPos, int endPos)
            : base(decorated)
        {
            Condition.Requires(startPos).IsGreaterOrEqual(0).IsLessThan(endPos);
            this.StartPos = startPos;
            this.EndPos = endPos;
        }
        #endregion

        #region Fluent Static
        public static StartEndPositionTokenDecoration New(IToken decorated, int startPos, int endPos)
        {
            return new StartEndPositionTokenDecoration(decorated, startPos, endPos);
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
        public override IDecorationOf<IToken> ApplyThisDecorationTo(IToken thing)
        {
            return new StartEndPositionTokenDecoration(thing, this.StartPos, this.EndPos);
        }
        #endregion
    }

    public static class StartEndPositionDecorationExtensions
    {
        public static StartEndPositionTokenDecoration HasStartEndPositions(this IToken thing, int startPos, int endPos)
        {
            Condition.Requires(thing).IsNotNull();
            return new StartEndPositionTokenDecoration(thing, startPos, endPos);
        }
    }
}