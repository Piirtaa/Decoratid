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
    /// <summary>
    /// decorates a token with a value factory
    /// </summary>
    public interface IHasValueFactoryToken<T> : IToken<T>, IHasValue
    {
        Func<IToken<T>, object> ValueFactory { get; }
    }

    /// <summary>
    /// decorates with a value
    /// </summary>
    [Serializable]
    public class HasValueFactoryTokenDecoration<T> : TokenDecorationBase<T>, IHasValueFactoryToken<T>
    {
        #region Ctor
        public HasValueFactoryTokenDecoration(IToken<T> decorated, Func<IToken<T>, object> valueFactory)
            : base(decorated)
        {
            this.ValueFactory = valueFactory;
        }
        #endregion

        #region Fluent Static
        public static HasValueFactoryTokenDecoration<T> New(IToken<T> decorated, Func<IToken<T>, object> valueFactory)
        {
            return new HasValueFactoryTokenDecoration<T>(decorated, valueFactory);
        }
        #endregion

        #region ISerializable
        protected HasValueFactoryTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public Func<IToken<T>, object> ValueFactory { get; private set; }
        public object Value
        {
            get { return this.ValueFactory(this);} 
        }
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasValueFactoryTokenDecoration<T>(thing, this.ValueFactory);
        }
        #endregion


    }

    public static class HasValueFactoryTokenDecorationExtensions
    {
        public static HasValueFactoryTokenDecoration<T> HasValueFactory<T>(this IToken<T> thing, Func<IToken<T>, object> valueFactory)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasValueFactoryTokenDecoration<T>(thing, valueFactory);
        }
    }
}
