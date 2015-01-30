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

namespace Decoratid.Idioms.TokenParsing.DotParenthesisComma
{
    public enum DPCTokenType
    {
        Operation,
        Item
    }
    /// <summary>
    /// a token that tracks where it was parsed from
    /// </summary>
    public interface IDPCToken : IToken
    {
        DPCTokenType TokenType { get; }
    }

    [Serializable]
    public class DPCDecoration : TokenDecorationBase, IDPCToken
    {
        #region Ctor
        public DPCDecoration(IToken decorated, DPCTokenType tokenType)
            : base(decorated)
        {
            this.TokenType = tokenType;
        }
        #endregion

        #region Fluent Static
        public static DPCDecoration New(IToken decorated, DPCTokenType tokenType)
        {
            return new DPCDecoration(decorated, tokenType);
        }
        #endregion

        #region ISerializable
        protected DPCDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public DPCTokenType TokenType { get; private set; }
        public override IDecorationOf<IToken> ApplyThisDecorationTo(IToken thing)
        {
            return new DPCDecoration(thing, this.TokenType);
        }
        #endregion
    }

    public static class DPCDecorationExtensions
    {
        public static DPCDecoration HasDPCTokenType(this IToken thing, DPCTokenType tokenType)
        {
            Condition.Requires(thing).IsNotNull();
            return new DPCDecoration(thing, tokenType);
        }

        
    }
}
