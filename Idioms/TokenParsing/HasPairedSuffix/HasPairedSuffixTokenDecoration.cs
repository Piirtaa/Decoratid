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

namespace Decoratid.Idioms.TokenParsing.HasPairedSuffix
{
    public interface IHasSuffixToken<T> : IToken<T>
    {
        T[] Suffix { get; }
        bool IsInclusive { get; }
    }

    /// <summary>
    /// decorates with suffix
    /// </summary>
    [Serializable]
    public class HasSuffixTokenDecoration<T> : TokenDecorationBase<T>, IHasSuffixToken<T>
    {
        #region Ctor
        public HasSuffixTokenDecoration(IToken<T> decorated, T[] suffix, bool isInclusive)
            : base(decorated)
        {
            this.Suffix = suffix;
            this.IsInclusive = isInclusive;
        }
        #endregion

        #region Fluent Static
        public static HasSuffixTokenDecoration<T> New(IToken<T> decorated, T[] suffix, bool isInclusive)
        {
            return new HasSuffixTokenDecoration<T>(decorated, suffix, isInclusive);
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
        public T[] Suffix { get; private set; }
        public bool IsInclusive { get; private set; }
        public override T[] TokenData
        {
            get
            {
                //remove the suffix
                var rv = base.TokenData;
                rv = rv.GetSegment(0, rv.Length - this.Suffix.Length);
                return rv;
            }
        }
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasSuffixTokenDecoration<T>(thing, this.Suffix, this.IsInclusive);
        }
        #endregion
    }

    public static class HasSuffixTokenDecorationExtensions
    {
        public static HasSuffixTokenDecoration<T> HasSuffix<T>(this IToken<T> thing, T[] suffix, bool isInclusive)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasSuffixTokenDecoration<T>(thing, suffix, isInclusive);
        }

        public static T[] GetSuffix<T>(this IToken<T> token)
        {
            Condition.Requires(token).IsNotNull();
            var tokenizer = token.GetFace<IHasSuffixToken<T>>();
            return tokenizer.With(x => x.Suffix);
        }
    }
}
