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

namespace Decoratid.Idioms.TokenParsing.HasPrefix
{
    public interface IHasPrefixToken<T> : IToken<T>
    {
        T[] Prefix { get; }
    }

    /// <summary>
    /// decorates with prefix
    /// </summary>
    [Serializable]
    public class HasPrefixTokenDecoration<T> : TokenDecorationBase<T>, IHasPrefixToken<T>
    {
        #region Ctor
        public HasPrefixTokenDecoration(IToken<T> decorated, T[] prefix)
            : base(decorated)
        {
            this.Prefix = prefix;
        }
        #endregion

        #region Fluent Static
        public static HasPrefixTokenDecoration<T> New(IToken<T> decorated, T[] prefix)
        {
            return new HasPrefixTokenDecoration<T>(decorated, prefix);
        }
        #endregion

        #region ISerializable
        protected HasPrefixTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public T[] Prefix { get; private set; }
        public override T[] TokenData
        {
            get
            {
                //remove the prefix
                var rv = base.TokenData;
                rv = rv.GetSegment(this.Prefix.Length);
                return  rv;
            }
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasPrefixTokenDecoration<T>(thing, this.Prefix);
        }
        #endregion
    }

    public static class HasPrefixTokenDecorationExtensions
    {
        public static HasPrefixTokenDecoration<T> HasPrefix<T>(this IToken<T> thing, T[] prefix)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasPrefixTokenDecoration<T>(thing, prefix);
        }
        public static T[] GetPrefix<T>(this IToken<T> token)
        {
            Condition.Requires(token).IsNotNull();
            var tokenizer = token.GetFace<IHasPrefixToken<T>>();
            return tokenizer.With(x => x.Prefix);
        }
    }
}
