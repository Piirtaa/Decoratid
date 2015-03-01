using CuttingEdge.Conditions;
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
using Decoratid.Idioms.TokenParsing.CommandLine.Lexing;
using Decoratid.Idioms.TokenParsing.HasValue;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
{
    /// <summary>
    /// a token that can evaluate to an object instance 
    /// </summary>
    public class SimpleThing : IToken<IToken<char>>, IHasValue
    {
        #region Declarations
        private static List<string> _validThingTokenizerIds = new List<string>(){
                CommandLineLexer.STORE,
                CommandLineLexer.NESS,
                CommandLineLexer.ARG,
                CommandLineLexer.THING};

        private object _value = null;
        #endregion

        #region Ctor
        public SimpleThing(IToken<char>[] tokens)
        {
            Condition.Requires(tokens).IsNotNull().HasLength(1);
            var tokenizerId = tokens[0].GetTokenizerId();
            Condition.Requires(_validThingTokenizerIds).Contains(tokenizerId, "token must be STORE, NESS, ARG or THING");
            this.TokenData = tokens;
            this.TokenizerId = tokenizerId;
        }
        #endregion

        #region Fluent Static
        public static SimpleThing New(IToken<char> token)
        {
            return new SimpleThing(token.AddToList().ToArray());
        }
        #endregion

        #region Properties
        public IToken<IToken<char>> PriorToken { get; set; }
        public IToken<char>[] TokenData { get; private set; }
        public string TokenizerId { get; private set; }
        #endregion

        #region IHasValue
        public object Value { get; set; }
        #endregion

    }
}
