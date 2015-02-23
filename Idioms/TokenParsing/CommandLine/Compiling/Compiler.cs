using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.CommandLine.Lexing;
using Decoratid.Idioms.TokenParsing.CommandLine.Evaluating;
using Decoratid.Core.Logical;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
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
        private UnitOfWork 
        public ILogic Compile(List<IStringToken> tokens)
        {
            UnitOfWork rv = UnitOfWork.New();


            var operandTokenizers = new List<string>(){
                CommandLineLexer.STORE,
                CommandLineLexer.NESS,
                CommandLineLexer.OP,
                CommandLineLexer.ARG,
                CommandLineLexer.THING};

            //@store.search(#ness.IsThing("x","y"))

            //iterate once only thru the tokens to do a compile
            //-thus all preprocessing steps need to be done at each iteration (Eg. evaluation)
            //we can decouple "Evaluation", and should prob think about that some more

            UnitOfWork uow = rv;

            for (int i = 0; i < tokens.Count; i++)
            {
                IStringToken each = tokens[i];

                //scrub
                IStringToken token = this.TokenScrubber.TouchToken(each);

                /*
                    Iterating thru the tokens:
                 * 
                 *      Is the CurrentToken an operand?
                 *          -yes
                 *              -do we have 
                 * 
                 * 
                 *          -no
                 *          
                 * 
                 
                    
                */ 

                if (token.IsOperandToken())
                {

                }

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
