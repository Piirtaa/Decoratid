using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.CommandLine
{

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
        public IToken TouchToken(IToken token)
        {
            Condition.Requires(token).IsNotNull();

            IToken rv = token;
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
                    var store = this.StoreOfStores.Get<NamedNaturalInMemoryStore>(token.TokenString);
                    rv = token.HasValue(store);
                    break;
            }

            return rv;
        }
        #endregion
    }
}
