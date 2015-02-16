using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// decorates with a string id
    /// </summary>
    public interface IHasStringIdTokenizer : IForwardMovingTokenizer, IHasId<String>
    {

    }

    /// <summary>
    /// a decorates with a string id
    /// </summary>
    [Serializable]
    public class HasStringIdTokenizerDecoration : ForwardMovingTokenizerDecorationBase, IHasStringIdTokenizer
    {
        #region Ctor
        public HasStringIdTokenizerDecoration(IForwardMovingTokenizer decorated, string id)
            : base(decorated)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
        }
        #endregion

        #region Fluent Static
        public static HasStringIdTokenizerDecoration New(IForwardMovingTokenizer decorated, string id)
        {
            return new HasStringIdTokenizerDecoration(decorated, id);
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
        public override bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            IToken newTokenOUT = null;

            var rv = base.Parse(text, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);

            //decorate token with tokenizer id
            newTokenOUT = newTokenOUT.HasTokenizerId(this.Id);

            newToken = newTokenOUT;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new HasStringIdTokenizerDecoration(thing, this.Id);
        }
        #endregion
    }

    public static class HasStringIdTokenizerDecorationExtensions
    {
        public static HasStringIdTokenizerDecoration HasId(this IForwardMovingTokenizer decorated, string id)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasStringIdTokenizerDecoration(decorated, id);
        }
    }
}


