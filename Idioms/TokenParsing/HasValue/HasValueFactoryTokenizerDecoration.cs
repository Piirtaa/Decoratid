using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core;
using Decoratid.Extensions;
using Decoratid.Core.Logical;

namespace Decoratid.Idioms.TokenParsing.HasValue
{
    public interface IHasValueFactoryTokenizer<T> : IForwardMovingTokenizer<T>
    {
        Func<IToken<T>, object> ValueFactory { get; set; }
    }

    [Serializable]
    public class HasValueFactoryTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasValueFactoryTokenizer<T>
    {
        #region Ctor
        public HasValueFactoryTokenizerDecoration(IForwardMovingTokenizer<T> decorated,
            Func<IToken<T>, object> valueFactory )
            : base(decorated)
        {
            Condition.Requires(valueFactory).IsNotNull();
            this.ValueFactory = valueFactory;
        }
        #endregion

        #region Fluent Static
        public static HasValueFactoryTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, Func<IToken<T>, object> valueFactory)
        {
            return new HasValueFactoryTokenizerDecoration<T>(decorated, valueFactory);
        }
        #endregion

        #region ISerializable
        protected HasValueFactoryTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public Func<IToken<T>, object> ValueFactory { get; set; }
     
        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            IToken<T> newTokenOUT = null;
            var rv = this.Decorated.Parse(source, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);
            
            //apply the has value token scrub
            newTokenOUT.HasValueFactory(this.ValueFactory);
            
            //return
            newToken = newTokenOUT;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new HasValueFactoryTokenizerDecoration<T>(thing, this.ValueFactory);
        }
        #endregion
    }

    public static class HasValueTokenizerDecorationExtensions
    {
        public static HasValueFactoryTokenizerDecoration<T> HasValueFactory<T>(this IForwardMovingTokenizer<T> decorated, Func<IToken<T>, object> valueFactory)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasValueFactoryTokenizerDecoration<T>(decorated, valueFactory);
        }

    }
}


