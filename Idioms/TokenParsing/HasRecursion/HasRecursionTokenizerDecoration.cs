﻿using CuttingEdge.Conditions;
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
using Decoratid.Idioms.TokenParsing.HasLength;
using Decoratid.Core.Logical;

namespace Decoratid.Idioms.TokenParsing.HasRecursion
{
    /// <summary>
    ///composite tokenizers essentially encapsulate the task of tokenizing with the state provided, using a 
    ///router as the initial logic.  takes the results and sticks them in a child token.  will tokenize as
    ///far as the routing allows for.  
    /// </summary>
    public interface ICompositeTokenizer<T> : IValidatingTokenizer<T>,
        IHasLengthStrategyTokenizerDecoration<T>
    {
        IRoutingTokenizer<T> Router { get; }

        /// <summary>
        /// produces the state to be used in a composite parsing run.  uses the current cursor as context.
        /// </summary>
        LogicOfTo<ForwardMovingTokenizingCursor<T>, object> StateStrategy { get; }
    }


    /// <summary>
    /// recursively tokenizes the contents as a composite token
    /// </summary>
    /// <remarks>
    /// creates a token that has child tokens 
    /// has a router that contains child token defs
    /// has length strategy that will default to perform the routed walk to see how long things are
    /// has 
    /// </remarks>
    [Serializable]
    public class CompositeTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, 
        ICompositeTokenizer<T>
    {
        #region Ctor
        public CompositeTokenizerDecoration(IForwardMovingTokenizer<T> decorated, 
            IRoutingTokenizer<T> router = null,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, int> lengthStrategy = null,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, object> stateStrategy = null)
            : base(decorated)
        {
            if (router == null)
            {
                this.Router = NaturallyNotImplementedForwardMovingTokenizer<T>.New().HasId("composite router").MakeRouter();
            }
            else
            {
                this.Router = router;
            }
            if (lengthStrategy == null)
            {
                //do a router parse and see how far we go
                this.LengthStrategy = LogicOfTo<ForwardMovingTokenizingCursor<T>, int>.New(cursor =>
                {
                    int newPos;
                    var vals = this.routerParse(cursor.Source, cursor.CurrentPosition, cursor.State, cursor.CurrentToken, out newPos);
                    return newPos;
                });
            }
            else
            {
                this.LengthStrategy = lengthStrategy;
            }
            if (stateStrategy == null)
            {
                this.StateStrategy = LogicOfTo<ForwardMovingTokenizingCursor<T>, object>.New(cursor =>
                {
                    return null;
                });
            }
            else
            {
                this.StateStrategy = stateStrategy;
            }
        }
        #endregion

        #region Fluent Static
        public static CompositeTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, IRoutingTokenizer<T> router = null,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, int> lengthStrategy = null,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, object> stateStrategy = null)
        {
            return new CompositeTokenizerDecoration<T>(decorated, router, lengthStrategy, stateStrategy);
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
        /// <summary>
        /// using the router, and a calculated initial state, performs a tokenize 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawData"></param>
        /// <param name="currentPosition"></param>
        /// <param name="state"></param>
        /// <param name="currentToken"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        private List<IToken<T>> routerParse<T>(T[] rawData,int currentPosition, object state, IToken<T> currentToken, out int newPosition)
        {
            var res = this.StateStrategy.Perform(ForwardMovingTokenizingCursor<T>.New(rawData, currentPosition, state, currentToken)) as LogicOfTo<ForwardMovingTokenizingCursor<T>, object>;
            object routerInitialState = res.Result;

            //get the substring from current position
            var subData = rawData.GetSegment(currentPosition);

            int newPos;
            var rv = subData.ForwardMovingTokenize(routerInitialState, this.Router as IForwardMovingTokenizer<T>, out newPos);

            //calc length to validate
            int length = 0;
            rv.WithEach(x =>
            {
                length += x.TokenData.Length;
            });
            Condition.Requires(length).IsEqualTo(newPos);

            newPosition = currentPosition + newPos;
            return rv;
        }

        #region Implementation
        public LogicOfTo<ForwardMovingTokenizingCursor<T>, object> StateStrategy { get; private set; }
        public LogicOfTo<ForwardMovingTokenizingCursor<T>, int> LengthStrategy { get; private set; }
        public IRoutingTokenizer<T> Router { get; private set; }
        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition { get; set; }
        public bool CanHandle(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
            int newPosition;
            this.routerParse(source, currentPosition, state, currentToken, out newPosition);
            return newPosition > 0;
        }

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            int newPos;
            var tokens = this.routerParse(source, currentPosition, state, currentToken, out newPos);

            if (newPos == 0)
            {
                newPosition = currentPosition;
                newToken = null;
                newParser = null;
                return false;
            }

            //build the composite
            var segData = source.GetSegment(currentPosition, newPos);
            newToken = NaturalToken<T>.New(segData).HasComposite(tokens.ToArray());
            newParser = null;
            newPosition = currentPosition + newPos;

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
        public static CompositeTokenizerDecoration<T> MakeComposite<T>(this IForwardMovingTokenizer<T> decorated, 
            IRoutingTokenizer<T> router = null,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, int> lengthStrategy = null,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, object> stateStrategy = null)
        {
            Condition.Requires(decorated).IsNotNull();
            return new CompositeTokenizerDecoration<T>(decorated, router, lengthStrategy, stateStrategy);
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


