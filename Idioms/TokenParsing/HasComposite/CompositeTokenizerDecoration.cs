using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.HasId;
using Decoratid.Idioms.TokenParsing.HasValidation;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.TokenParsing.HasRouting;

namespace Decoratid.Idioms.TokenParsing.HasComposite
{
    /// <summary>
    /// is a tokenizer that returns composite token, and internally using a Router to parse.
    /// the composite token contains the results of a tokenize operation
    /// </summary>
    public interface ICompositeTokenizer<T> : IValidatingTokenizer<T>
    {
        IRoutingTokenizer<T> Router { get; }

    }


    /// <summary>
    /// recursively tokenizes the contents as a composite token
    /// </summary>
    [Serializable]
    public class CompositeTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, ICompositeTokenizer<T>
    {
        #region Ctor
        public CompositeTokenizerDecoration(IForwardMovingTokenizer<T> decorated, IRoutingTokenizer<T> router = null)
            : base(decorated)
        {
            if (router == null)
            {
                this.Router = NaturallyNotImplementedForwardMovingTokenizer<T>.New().MakeRouter();
            }
            else
            {
                this.Router = router;
            }
        }
        #endregion

        #region Fluent Static
        public static CompositeTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, IRoutingTokenizer<T> router = null)
        {
            return new CompositeTokenizerDecoration<T>(decorated, router);
        }
        #endregion

        #region ISerializable
        protected CompositeTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public IRoutingTokenizer<T> Router { get; private set; }

        public bool CanHandle(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
            //if the router can handle at least one of the items, we're good to parse
            return this.Router.CanHandle(source, currentPosition, state, currentToken);
        }
        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            //get the substring from current position
            var subData = source.GetSegment(currentPosition);

            //tokenize as far as we can
            int subNewPos = 0;
            var tokens = subData.ForwardMovingTokenize(null, this.Router, out subNewPos);

            if (subNewPos == 0)
            {
                newPosition = currentPosition;
                newToken = null;
                newParser = null;
                return false;
            }

            //build the composite
            var segData = subData.GetSegment(subNewPos);
            newToken = NaturalToken<T>.New(segData).HasComposite(tokens.ToArray());
            newParser = null;
            newPosition = currentPosition + subNewPos;

            return true;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            var rv = new CompositeTokenizerDecoration<T>(thing, this.Router);
            return rv;
        }
        #endregion
    }

    public static class CompositeTokenizerDecorationExtensions
    {
        public static CompositeTokenizerDecoration<T> MakeComposite<T>(this IForwardMovingTokenizer<T> decorated, IRoutingTokenizer<T> router = null)
        {
            Condition.Requires(decorated).IsNotNull();
            return new CompositeTokenizerDecoration<T>(decorated, router);
        }
        public static CompositeTokenizerDecoration<T> MakeCompositeOf<T>(this IForwardMovingTokenizer<T> decorated, params IForwardMovingTokenizer<T>[] composites)
        {
            Condition.Requires(decorated).IsNotNull();

            var rv = new CompositeTokenizerDecoration<T>(decorated);
            composites.WithEach(x =>
            {
                var tokenizer = x.As<IHasStringIdTokenizer<T>>(false);
                if (tokenizer != null)
                    rv.Router.AddTokenizer(tokenizer);
            });
            return rv;
        }
    }
}


