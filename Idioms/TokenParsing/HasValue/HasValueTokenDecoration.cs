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


namespace Decoratid.Idioms.TokenParsing.HasValue
{
    public interface IHasValue
    {
        object Value { get; }
    }
    /// <summary>
    /// decorates a token with a value
    /// </summary>
    public interface IHasValueToken<T> : IToken<T>, IHasValue
    {
    }

    /// <summary>
    /// decorates with a value
    /// </summary>
    [Serializable]
    public class HasValueTokenDecoration<T> : TokenDecorationBase<T>, IHasValueToken<T>
    {
        #region Ctor
        public HasValueTokenDecoration(IToken<T> decorated, object value)
            : base(decorated)
        {
            this.Value = value;
        }
        #endregion

        #region Fluent Static
        public static HasValueTokenDecoration<T> New(IToken<T> decorated, object value)
        {
            return new HasValueTokenDecoration<T>(decorated, value);
        }
        #endregion

        #region ISerializable
        protected HasValueTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public object Value { get; private set; }

        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasValueTokenDecoration<T>(thing, this.Value);
        }
        #endregion
    }

    public static class HasValueTokenDecorationExtensions
    {
        public static HasValueTokenDecoration<T> HasValue<T>(this IToken<T> thing, object value)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasValueTokenDecoration<T>(thing, value);
        }
    }
}
