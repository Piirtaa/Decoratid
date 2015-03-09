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
using Decoratid.Idioms.TokenParsing.HasComment;

namespace Decoratid.Idioms.TokenParsing.HasRouting
{

    //define behaviour of the logic for each bit
    using TokenizerItem = IsA<IHasId<string>>;
    using System.Diagnostics;

    /// <summary>
    /// this decoration delegates the actual tokenizing process to the appropriate tokenizer. Typically instances of this
    /// are used as the bootstrapping tokenizer
    /// </summary>
    public interface IRoutingTokenizer<T> : IValidatingTokenizer<T>
    {
        /// <summary>
        /// the store of IValidatingTokenizer (wrapped as an IsA to handle cakes)
        /// </summary>
        IStoreOf<TokenizerItem> TokenizerStore { get; }
        IRoutingTokenizer<T> AddTokenizer(params IHasStringIdTokenizer<T>[] t);
        /// <summary>
        /// finds first tokenizer that validates (eg. says it can handle) the current cursor position
        /// </summary>
        /// <param name="source"></param>
        /// <param name="currentPosition"></param>
        /// <param name="state"></param>
        /// <param name="currentToken"></param>
        /// <returns></returns>
        TokenizerItem GetTokenizer(T[] source, int currentPosition, object state, IToken<T> currentToken);
        /// <summary>
        /// ignores the nextRouter provided by the handling tokenizer and uses own routing 
        /// </summary>
        bool OverridesTokenizerRouting { get; }
        /// <summary>
        /// if no validating tokenizer can be found for a given cursor, it will capture the unrecognized area
        /// as a token commented as UNRECOGNIZED
        /// </summary>
        bool TokenizeUnrecognized { get; }

    }

    /// <summary>
    /// this decoration delegates the actual tokenizing process to the appropriate tokenizer
    /// </summary>
    [Serializable]
    public class RoutingTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IRoutingTokenizer<T>
    {
        #region Ctor
        public RoutingTokenizerDecoration(IForwardMovingTokenizer<T> decorated, 
            bool overridesTokenizerRouting = true,
            bool tokenizeUnrecognized = true)
            : base(decorated)
        {
            this.TokenizerStore = NaturalInMemoryStore.New().IsOf<TokenizerItem>();
            this.OverridesTokenizerRouting = overridesTokenizerRouting;
            this.TokenizeUnrecognized = tokenizeUnrecognized;
        }
        #endregion

        #region Fluent Static
        public static RoutingTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, bool overridesTokenizerRouting = true,
            bool tokenizeUnrecognized = true)
        {
            return new RoutingTokenizerDecoration<T>(decorated, overridesTokenizerRouting, tokenizeUnrecognized);
        }
        #endregion

        #region ISerializable
        protected RoutingTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// ignores the nextRouter provided by the handling tokenizer and uses own routing 
        /// </summary>
        public bool OverridesTokenizerRouting { get; private set; }
        /// <summary>
        /// if no validating tokenizer can be found for a given cursor, it will capture the unrecognized area
        /// as a token commented as UNRECOGNIZED
        /// </summary>
        public bool TokenizeUnrecognized { get; private set; }
        public IStoreOf<TokenizerItem> TokenizerStore { get; private set; }
        public IRoutingTokenizer<T> AddTokenizer(params IHasStringIdTokenizer<T>[] t)
        {
            t.WithEach(x =>
            {

                TokenizerItem item = new TokenizerItem(x as IFaceted);

                this.TokenizerStore.SaveItem(item);

            });
            return this;
        }
        public TokenizerItem GetTokenizer(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
           // Debug.WriteLine("Router getting tokenizer @ {0} of {1}", currentPosition, string.Join("", source));

            //if we're passed the end of the source, return null
            if (source.Length <= currentPosition)
                return null;

            List<TokenizerItem> tokenizers = TokenizerStore.GetAll();

            //iterate thru all the tokenizers and find ones that know if they can handle stuff
            foreach (var each in tokenizers)
            {
                var sd = each.GetFace<IValidatingTokenizer<T>>();

                if (sd != null)
                    if (sd.CanHandle(source, currentPosition, state, currentToken))
                    {
                       // Debug.WriteLine("Router gets {0}", each.Id);
                        return each;
                    }
            }
            //Debug.WriteLine("Router cannot get tokenizer");
            return null;
        }

        public virtual bool CanHandle(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
            var tokenizer = GetTokenizer(source, currentPosition, state, currentToken);
            return tokenizer != null;
        }
        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            //Debug.WriteLine("Parsing @ {0} of {1}", currentPosition, string.Join("", source));

            //get the new tokenizer
            var tokenizer = GetTokenizer(source, currentPosition, state, currentToken);
            
            int newPositionOUT = 0;
            IToken<T> newTokenOUT = null;
            IForwardMovingTokenizer<T> newParserOUT = null;

            bool rv = tokenizer != null;

            if (rv)
            {
                //Debug.WriteLine("located tokenizer delegated to");
                IForwardMovingTokenizer<T> alg = tokenizer.As<IForwardMovingTokenizer<T>>().GetOuterDecorator() as IForwardMovingTokenizer<T>;
                var cake2 = alg.GetAllDecorations();
                rv = alg.Parse(source, currentPosition, state, currentToken, out newPositionOUT, out newTokenOUT, out newParserOUT);
            }

            //loop back into router to handle the next token
            if (OverridesTokenizerRouting)
                newParserOUT = this;
 
            //if we classify unrecognized, then we do that here
            if (!rv && this.TokenizeUnrecognized)
            {
                //Debug.WriteLine("classifying unrecognized");

                rv = true;
                newPositionOUT = GetNextRecognizedPosition(source, currentPosition, state, currentToken);

                //get string between old and new positions
                var tokenText = source.GetSegment(currentPosition, newPositionOUT - currentPosition);

                //Debug.WriteLine("unrecognized runs to {0}, producing {1}", newPositionOUT, string.Join("", tokenText));

                //returns a suffixed natural token
                newTokenOUT = NaturalToken<T>.New(tokenText).HasComment("UNRECOGNIZED");

                newParserOUT = this;
            }

            newParser = newParserOUT;
            newToken = newTokenOUT;
            newPosition = newPositionOUT;

            //if (!rv)
            //    Debug.WriteLine("tokenize fail.");

            //if (rv)
            //    Debug.WriteLine("tokenize successful. new position = {0}. token = {1}", newPositionOUT, string.Join("", newToken.TokenData));

            //Debug.WriteLine("Parsing Complete @ {0} of {1}", currentPosition, string.Join("", source));

            return rv;
        }
        private int GetNextRecognizedPosition(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
            bool isRecognized = false;
            int pos = currentPosition;
            int maxPos = source.Length - 1;
            while (!isRecognized && pos <= maxPos)
            {
                isRecognized = this.CanHandle(source, pos, state, currentToken);
                if (!isRecognized)
                    pos++;
            }
            return pos;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            var rv = new RoutingTokenizerDecoration<T>(thing, this.OverridesTokenizerRouting, this.TokenizeUnrecognized);

            //move the rules over
            var rules = this.TokenizerStore.GetAll();
            rules.WithEach(x =>
            {
                rv.TokenizerStore.SaveItem(x);
            });

            return rv;
        }
        #endregion
    }

    public static class RoutingTokenizerDecorationExtensions
    {
        public static RoutingTokenizerDecoration<T> MakeRouter<T>(this IForwardMovingTokenizer<T> decorated ,
            bool overridesTokenizerRouting = true,
            bool tokenizeUnrecognized = true)
        {
            Condition.Requires(decorated).IsNotNull();
            return new RoutingTokenizerDecoration<T>(decorated);
        }
    }
}


