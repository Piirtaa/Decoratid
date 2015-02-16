using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Decorating;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    public interface IHasPredecessorTokenizer : IHasHandleConditionTokenizer
    {
        string[] PriorTokenizerIds { get; }
    }

    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    [Serializable]
    public class HasPredecessorTokenizerDecoration : ForwardMovingTokenizerDecorationBase, IHasPredecessorTokenizer
    {
        #region Ctor
        public HasPredecessorTokenizerDecoration(IForwardMovingTokenizer decorated, params string[] priorTokenizerIds)
            : base(decorated.HasSelfDirection())
        {
            Condition.Requires(priorTokenizerIds).IsNotEmpty();
            this.PriorTokenizerIds = priorTokenizerIds;
        }
        #endregion

        #region Fluent Static
        public static HasPredecessorTokenizerDecoration New(IForwardMovingTokenizer decorated, params string[] priorTokenizerIds)
        {
            return new HasPredecessorTokenizerDecoration(decorated, priorTokenizerIds);
        }
        #endregion

        #region ISerializable
        protected HasPredecessorTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public string[] PriorTokenizerIds { get; private set; }
        public IConditionOf<ForwardMovingTokenizingOperation> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingOperation>.New((x) =>
                {
                    var substring = x.Text.Substring(x.CurrentPosition);

                    //can't check without predecessor
                    if (x.CurrentToken == null)
                        return false;
                    //can't check if not decorated
                    if (!(x.CurrentToken is IFaceted))
                        return false;

                    var isa = IsA.New(x.CurrentToken as IFaceted);
                    //not decorated with tokenizerId, which is what we're comparing against
                    if (!isa.Is<IHasTokenizerId>())
                        return false;

                    var tokenizer = isa.As<IHasTokenizerId>();

                    var rv = false;
                    foreach (var each in this.PriorTokenizerIds)
                    {
                        //finally check if the last token has the correct tokenizerid
                        if (tokenizer.TokenizerId.Equals(each))
                        {
                            rv = true;
                            break;
                        }
                    }

                    if (!rv)
                        return false;

                    return true;
                });
                return cond;
            }
        }

        private string GetPriorTokenizerId(string text, int currentPosition, object state, IToken currentToken)
        {
            string rv = null;

            //can't check without predecessor
            if (currentToken == null)
                return rv;
            //can't check if not decorated
            if (!(currentToken is IFaceted))
                return rv;

            var isa = IsA.New(currentToken as IFaceted);
            //not decorated with tokenizerId, which is what we're comparing against
            if (!isa.Is<IHasTokenizerId>())
                return rv;

            var tokenizer = isa.As<IHasTokenizerId>();

            foreach (var each in this.PriorTokenizerIds)
            {
                //finally check if the last token has the correct tokenizerid
                if (tokenizer.TokenizerId.Equals(each))
                {
                    rv = each;
                    break;
                }
            }

            return rv;
        }
        public override bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            IToken newTokenOUT = null;

            var rv = base.Parse(text, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);

            //decorate token with prior tokenizer id
            var priorTokenizerId = this.GetPriorTokenizerId(text, currentPosition, state, currentToken);
            newTokenOUT = newTokenOUT.HasPriorTokenizerId(priorTokenizerId);

            newToken = newTokenOUT;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new HasPredecessorTokenizerDecoration(thing, this.PriorTokenizerIds);
        }
        #endregion
    }

    public static class HasPriorTokenizerIdTokenizerDecorationExtensions
    {
        public static HasPredecessorTokenizerDecoration HasPredecessor(this IForwardMovingTokenizer decorated,
            params string[] priorTokenizerIds)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasPredecessorTokenizerDecoration(decorated, priorTokenizerIds);
        }

        public static HasPredecessorTokenizerDecoration HasPredecessor(this IForwardMovingTokenizer decorated,
    params IHasStringIdTokenizer[] priorTokenizers)
        {
            Condition.Requires(decorated).IsNotNull();
            List<string> ids = new List<string>();
            foreach (var each in priorTokenizers)
                ids.Add(each.Id);

            var rv = new HasPredecessorTokenizerDecoration(decorated, ids.ToArray());

            //wire up the 

            return rv;
        }
    }
}


