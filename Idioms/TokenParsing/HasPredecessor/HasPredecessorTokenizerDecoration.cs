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
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.HasSelfDirection;
using Decoratid.Idioms.TokenParsing.HasTokenizerId;
using Decoratid.Idioms.TokenParsing.HasId;

namespace Decoratid.Idioms.TokenParsing.HasPredecessor
{
    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    public interface IHasPredecessorTokenizer<T> : IHasHandleConditionTokenizer<T>
    {
        string[] PriorTokenizerIds { get; }
    }

    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    [Serializable]
    public class HasPredecessorTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasPredecessorTokenizer<T>
    {
        #region Ctor
        public HasPredecessorTokenizerDecoration(IForwardMovingTokenizer<T> decorated, params string[] priorTokenizerIds)
            : base(decorated.HasSelfDirection())
        {
            this.PriorTokenizerIds = priorTokenizerIds;
        }
        #endregion

        #region Fluent Static
        public static HasPredecessorTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, params string[] priorTokenizerIds)
        {
            return new HasPredecessorTokenizerDecoration<T>(decorated, priorTokenizerIds);
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
        public IConditionOf<ForwardMovingTokenizingOperation<T>> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingOperation<T>>.New((x) =>
                {
                    var substring = x.Source.GetSegment(x.CurrentPosition);

                    //can't check without predecessor
                    //unless a null tokenizer id exists, in which case return true
                    if (x.CurrentToken == null)
                    {
                        if (this.PriorTokenizerIds == null)
                            return true;

                        if (this.PriorTokenizerIds.Contains(null))
                            return true;

                        return false;
                    }

                    //if we have no predecessors to validate (other than the null predecessor we handle above), then we skip out
                    if (this.PriorTokenizerIds == null)
                        return false;

                    //can't check if not decorated
                    if (!(x.CurrentToken is IFaceted))
                        return false;

                    var isa = IsA.New(x.CurrentToken as IFaceted);
                    //not decorated with tokenizerId, which is what we're comparing against
                    if (!isa.Is<IHasTokenizerId<T>>())
                        return false;

                    var tokenizer = isa.As<IHasTokenizerId<T>>();

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

        private string GetPriorTokenizerId(T[] source, int currentPosition, object state, IToken<T> currentToken)
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
            if (!isa.Is<IHasTokenizerId<T>>())
                return rv;

            var tokenizer = isa.As<IHasTokenizerId<T>>();

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
        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            IToken<T> newTokenOUT = null;

            var rv = base.Parse(source, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);

            //decorate token with prior tokenizer id
            var priorTokenizerId = this.GetPriorTokenizerId(source, currentPosition, state, currentToken);
            newTokenOUT = newTokenOUT.HasPriorTokenizerId(priorTokenizerId);

            newToken = newTokenOUT;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new HasPredecessorTokenizerDecoration<T>(thing, this.PriorTokenizerIds);
        }
        #endregion
    }

    public static class HasPriorTokenizerIdTokenizerDecorationExtensions
    {
        public static HasPredecessorTokenizerDecoration<T> HasPredecessorTokenizerIds<T>(this IForwardMovingTokenizer<T> decorated,
            params string[] priorTokenizerIds)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasPredecessorTokenizerDecoration<T>(decorated, priorTokenizerIds);
        }

        public static HasPredecessorTokenizerDecoration<T> HasPredecessorTokenizers<T>(this IForwardMovingTokenizer<T> decorated,
    params IHasStringIdTokenizer<T>[] priorTokenizers)
        {
            Condition.Requires(decorated).IsNotNull();
            List<string> ids = new List<string>();
            foreach (var each in priorTokenizers)
                ids.Add(each.Id);

            var rv = new HasPredecessorTokenizerDecoration<T>(decorated, ids.ToArray());

            //wire up the 

            return rv;
        }
    }
}


