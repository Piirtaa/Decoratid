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

namespace Decoratid.Idioms.TokenParsing.HasSuffix
{
    public interface IHasSuffixToken<T> : IToken<T>
    {
        T[] Suffix { get; }
    }

    /// <summary>
    /// decorates with suffix
    /// </summary>
    [Serializable]
    public class HasSuffixTokenDecoration<T> : TokenDecorationBase<T>, IHasSuffixToken<T>
    {
        #region Ctor
        public HasSuffixTokenDecoration(IToken<T> decorated, T[] suffix)
            : base(decorated)
        {
            this.Suffix = suffix;
        }
        #endregion

        #region Fluent Static
        public static HasSuffixTokenDecoration<T> New(IToken<T> decorated, T[] suffix)
        {
            return new HasSuffixTokenDecoration<T>(decorated, suffix);
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
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasSuffixTokenDecoration<T>(thing, this.Suffix);
        }
        #endregion
    }

    public static class HasSuffixTokenDecorationExtensions
    {
        public static HasSuffixTokenDecoration<T> HasSuffix<T>(this IToken<T> thing, T[] suffix)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasSuffixTokenDecoration<T>(thing, suffix);
        }
    }
}
