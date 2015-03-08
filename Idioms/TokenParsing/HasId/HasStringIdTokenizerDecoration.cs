using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.TokenParsing.HasTokenizerId;

namespace Decoratid.Idioms.TokenParsing.HasId
{
    /// <summary>
    /// decorates with a string id
    /// </summary>
    public interface IHasStringIdTokenizer<T> : IForwardMovingTokenizer<T>, IHasId<String>
    {

    }

    /// <summary>
    /// a decorates with a string id
    /// </summary>
    [Serializable]
    public class HasStringIdTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasStringIdTokenizer<T>
    {
        #region Ctor
        public HasStringIdTokenizerDecoration(IForwardMovingTokenizer<T> decorated, string id)
            : base(decorated)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
        }
        #endregion

        #region Fluent Static
        public static HasStringIdTokenizerDecoration<T> New<T>(IForwardMovingTokenizer<T> decorated, string id)
        {
            return new HasStringIdTokenizerDecoration<T>(decorated, id);
        }
        #endregion

        #region ISerializable
        protected HasStringIdTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasId
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Implementation
        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            IToken<T> newTokenOUT = null;

            var rv = this.Decorated.Parse(source, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);

            //decorate token with tokenizer id
            newTokenOUT = newTokenOUT.HasTokenizerId(this.Id);

            newToken = newTokenOUT;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new HasStringIdTokenizerDecoration<T>(thing, this.Id);
        }
        #endregion
    }

    public static class HasStringIdTokenizerDecorationExtensions
    {
        public static HasStringIdTokenizerDecoration<T> HasId<T>(this IForwardMovingTokenizer<T> decorated, string id)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasStringIdTokenizerDecoration<T>(decorated, id);
        }
    }
}


