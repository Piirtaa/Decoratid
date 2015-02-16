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

namespace Decoratid.Idioms.TokenParsing
{
    public interface IHasSuffixToken : IToken
    {
        string Suffix { get; }
    }

    /// <summary>
    /// decorates with suffix
    /// </summary>
    [Serializable]
    public class HasSuffixTokenDecoration : TokenDecorationBase, IHasSuffixToken
    {
        #region Ctor
        public HasSuffixTokenDecoration(IToken decorated, string suffix)
            : base(decorated)
        {
            this.Suffix = suffix;
        }
        #endregion

        #region Fluent Static
        public static HasSuffixTokenDecoration New(IToken decorated, string suffix)
        {
            return new HasSuffixTokenDecoration(decorated, suffix);
        }
        #endregion

        #region ISerializable
        protected HasSuffixTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public string Suffix { get; private set; }
        public override IDecorationOf<IToken> ApplyThisDecorationTo(IToken thing)
        {
            return new HasSuffixTokenDecoration(thing, this.Suffix);
        }
        #endregion
    }

    public static class HasSuffixTokenDecorationExtensions
    {
        public static HasSuffixTokenDecoration HasSuffix(this IToken thing, string suffix)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasSuffixTokenDecoration(thing, suffix);
        }
    }
}
