﻿using System;
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
using Decoratid.Core.Conditional.Of;
using Decoratid.Core;
using Decoratid.Idioms.TokenParsing.CommandLine.Lexing;
using CuttingEdge.Conditions;

using Decoratid.Idioms.Ness;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Core.Storing;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
{
    /// <summary>
    /// lexes the Level1 results into HasValue tokens (ie. UnitOfWork or SimpleThing)
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class TokenLexer : IForwardMovingTokenizer<IToken<char>>, IValidatingTokenizer<IToken<char>>
    {
        #region Declarations
        private static List<string> _validThingTokenizerIds = new List<string>(){
                CommandLineLexer.STORE,
                CommandLineLexer.NESS,
                CommandLineLexer.ARG,
                CommandLineLexer.THING};
        #endregion

        #region Ctor
        public TokenLexer(NessManager nessManager, IStoreOf<NamedNaturalInMemoryStore> storeOfStores = null)
        {
            if (storeOfStores == null)
            {
                this.StoreOfStores = NaturalInMemoryStore.New().IsOf<NamedNaturalInMemoryStore>();
            }
            else
            {
                this.StoreOfStores = storeOfStores;
            }
            this.NessManager = nessManager;
        }
        #endregion

        #region IValidatingTokenizer
        public bool CanHandle(IToken<char>[] source, int currentPosition, object state, IToken<IToken<char>> currentToken)
        {
            return true;
        }
        #endregion

        #region Properties
        public IStoreOf<NamedNaturalInMemoryStore> StoreOfStores { get; private set; }
        public NessManager NessManager { get; private set; }
        #endregion

        #region IForwardMovingTokenizer
        /*
         *  Source string:
                @store.search(#ness.IsThing("x","y"))
            First level Tokens:
         *      @store
         *      .search
         *      (
         *      #ness
         *      .IsThing
         *      (
         *      "x"
         *      ,
         *      "y"
         *      )
         *      )

         * 2nd level UoW Tokens:
         *      UoW0.Opr = @store
         *      UoW0.Op = .search
         *      UoW0.Arg0 = UoW1
         *     
         *      UoW1.Opr = #ness
         *      UoW1.Op = .IsThing
         *      UoW1.Arg0 = SimpleThing "x"
         *      UoW1.Arg1 = SimpleThing "y"

         * The process:
         *  
         *  iterating thru the level 1 tokens,
         *  -is the token a simple thing?  currentThing = build SimpleThing
         *  -is the token a thing?  currentThing = build UoW
         *      
        */
        public bool Parse(IToken<char>[] source, int currentPosition, object state, IToken<IToken<char>> currentToken, 
            out int newPosition, out IToken<IToken<char>> newToken, out IForwardMovingTokenizer<IToken<char>> newParser)
        {
            //create the UnitOfWork we're going to perform, regardless            
            UnitOfWork uow = new UnitOfWork();
            UnitOfWork rv = uow;

            //get context if we have it
            UnitOfWork contextUoW = currentToken as UnitOfWork;
            
            //do we have context?  this means we're setting args on the context UoW and returning that
            //otherwise we return the UoW we 

            //@store.search(#ness.IsThing("x","y"))

            
            //expect operand
            int pos = currentPosition;
            var token0 = source[pos];
            Condition.Requires(token0.IsOperandToken(), "operand expected");
            uow.OperandToken = token0;

            //expect operation
            pos++;
            var token1 = source[pos];
            Condition.Requires(token1).IsNotNull();
            Condition.Requires(token1.GetTokenizerId()).IsEqualTo(CommandLineLexer.OP);
            uow.OperationToken = token1;
          
            //, -> append

            //expect ( or . (another op)
            pos++;
            var token2 = source[pos];

            if (token2.GetTokenizerId() == CommandLineLexer.OP)
            {
                UnitOfWork uow2 = UnitOfWork.New();
                uow2.Operand = uow;
                uow2.OperationToken = token2;

                rv = uow2;

                //keep going until it's no longer an op
                //keep wrapping rv
            }

            //if it's a (
            //  -we recurse.  

            
            //if an op, we create a new unit of work



            
            return true;
        }

        //private IToken<IToken<char>> BuildToken(IToken<char>[] source, int currentPosition, object state, out int newPosition)
        //{
        //    //get current source token
        //    var sourceToken = source[currentPosition];
            
        //    //can we parse a simple thing out? (store, ness, arg, thing)
        //    var tokenizerId = sourceToken.GetTokenizerId();
        //    Condition.Requires(_validThingTokenizerIds).Contains(tokenizerId);
            
        //    //create the simple thing
        //    var operand = SimpleThing.New(sourceToken);
            
        //    //set the value
        //    switch(tokenizerId)
        //    {
        //        case CommandLineLexer.ARG:
        //        case CommandLineLexer.THING:
        //            //set the value of the operand to the arg's text.  this will be parsed on interpretation
        //            operand.Value = new string(sourceToken.TokenData);
        //            break;
        //        case CommandLineLexer.STORE:
        //            string storeName = new string(sourceToken.TokenData);
        //            var store = this.StoreOfStores.Get<NamedNaturalInMemoryStore>(storeName);
        //            operand.Value = store;
        //            break;
        //        case CommandLineLexer.NESS:
        //            string nessName = new string(sourceToken.TokenData);
        //            operand.Value = nessName;

        //            //what the ness is depends on the previous value.  so we build a function
        //            this.NessManager.GetNess(
        //            //operand.Value
        //            break;
        //    }
        //    return operand;
        //}
        #endregion
    }
}
