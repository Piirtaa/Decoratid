using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TokenParsing.CommandLine
{
    public class Compiler
    {
        #region Ctor
        public Compiler(TokenEvaluator scrubber)
        {
            Condition.Requires(scrubber).IsNotNull();
            this.TokenScrubber = scrubber;
        }
        #endregion

        #region Static Fluent
        public static Compiler New(TokenEvaluator scrubber)
        {
            return new Compiler(scrubber);
        }
        #endregion

        #region Properties
        private TokenEvaluator TokenScrubber { get; set; }
        #endregion

        #region Methods
        public UnitOfWork Compile(List<IToken> tokens)
        {
            UnitOfWork rv = UnitOfWork.New();

            UnitOfWork uow = rv;
            var operandTokenizers = new List<string>(){
                CommandLineLexer.STORE,
                CommandLineLexer.NESS,
                CommandLineLexer.OP,
                CommandLineLexer.ARG};

            //@store.search(#ness.IsThing("x","y"))

            foreach (IToken each in tokens)
            {
                //scrub
                IToken token = this.TokenScrubber.TouchToken(each);

                string tokenizerId = token.GetTokenizerId();

                //if the token is an Operand (eg. store, op, ness or arg)
                //then the current uow must have an empty OperandToken
                if (operandTokenizers.Contains(tokenizerId))
                {
                    if (uow.OperandToken != null)
                        throw new CommandLineCompilationException("expected null operand");

                    uow.OperandToken = each;
                    continue;
                }

                //if the token is an Operation(eg. 
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
                        break;
                }
            }

            return rv;
        }
        #endregion
    }
}
