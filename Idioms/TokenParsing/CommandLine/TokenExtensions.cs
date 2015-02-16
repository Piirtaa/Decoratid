using CuttingEdge.Conditions;
using Decoratid.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TokenParsing.CommandLine
{
    public static class TokenExtensions
    {
        public static object GetTokenValue(this IToken token)
        {
            Condition.Requires(token).IsNotNull();

            var tokenizer = token.GetFace<IHasValueToken>();
            return tokenizer.With(x => x.Value);
        }
    }
}
