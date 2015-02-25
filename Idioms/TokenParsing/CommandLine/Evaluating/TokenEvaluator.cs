using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Storing;
using Decoratid.Idioms.TokenParsing.CommandLine.Lexing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.TokenParsing.HasConstantValue;
using Decoratid.Idioms.TokenParsing.HasId;
using Decoratid.Idioms.TokenParsing.HasPredecessor;
using Decoratid.Idioms.TokenParsing.HasPrefix;
using Decoratid.Idioms.TokenParsing.HasRouting;
using Decoratid.Idioms.TokenParsing.HasValidation;
using Decoratid.Idioms.TokenParsing.HasStartEnd;
using Decoratid.Idioms.TokenParsing.HasSuccessor;
using Decoratid.Idioms.TokenParsing.HasSuffix;
using Decoratid.Idioms.TokenParsing.HasTokenizerId;
using Decoratid.Idioms.TokenParsing.HasImplementation;
using Decoratid.Idioms.TokenParsing.HasValue;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Evaluating
{
    /// <summary>
    /// decorates tokens with evaluated data
    /// </summary>
    public class TokenEvaluator
    {
        #region Ctor
        public TokenEvaluator(IStoreOf<NamedNaturalInMemoryStore> storeOfStores = null)
        {
            if (storeOfStores == null)
            {
                this.StoreOfStores = NaturalInMemoryStore.New().IsOf<NamedNaturalInMemoryStore>();
            }
            else
            {
                this.StoreOfStores = storeOfStores;
            }
        }
        #endregion

        #region Fluent Static
        public static TokenEvaluator New(IStoreOf<NamedNaturalInMemoryStore> storeOfStores = null)
        {
            return new TokenEvaluator(storeOfStores);
        }
        #endregion

        #region Properties
        public IStoreOf<NamedNaturalInMemoryStore> StoreOfStores { get; private set; }
        #endregion

        #region Methods
        public IToken<char> TouchToken(IToken<char> token)
        {
            Condition.Requires(token).IsNotNull();

            IToken<char> rv = token;
            if (!(token is IFaceted))
                return rv;

            var tokenizerId = token.GetTokenizerId();
            
            switch (tokenizerId)
            {
                case CommandLineLexer.ARG:
                    break;
                case CommandLineLexer.CLOSEPAREN:
                    break;
                case CommandLineLexer.COMMA:
                    break;
                case CommandLineLexer.NESS:
                    break;
                case CommandLineLexer.OP:
                    break;
                case CommandLineLexer.OPENPAREN:
                    break;
                case CommandLineLexer.STORE:
                    string storeName = new string(token.TokenData);
                    var store = this.StoreOfStores.Get<NamedNaturalInMemoryStore>(storeName);
                    rv = token.HasValue(store);
                    break;
            }

            return rv;
        }
        #endregion
    }
}
