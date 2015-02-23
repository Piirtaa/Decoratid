using CuttingEdge.Conditions;
using Decoratid.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.CommandLine.Evaluating;
using Decoratid.Idioms.TokenParsing.CommandLine.Lexing;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
{
    public static class TokenExtensions
    {
        private static List<string> _operandTokenizers = new List<string>(){
                CommandLineLexer.STORE,
                CommandLineLexer.NESS,
                CommandLineLexer.OP,
                CommandLineLexer.ARG,
                CommandLineLexer.THING};

        public static object GetTokenValue(this IStringToken token)
        {
            Condition.Requires(token).IsNotNull();

            var tokenizer = token.GetFace<IHasValueToken>();
            return tokenizer.With(x => x.Value);
        }

        /// <summary>
        /// is the token a store, ness, op, arg or thing
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsOperandToken(this IStringToken token)
        {
            Condition.Requires(token).IsNotNull();

            string tokenizerId = token.GetTokenizerId();
            var rv = (_operandTokenizers.Contains(tokenizerId));
            return rv;
        }
    }
}
