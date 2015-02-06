using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.CommandLine
{
    public class DPCOperation
    {
        public DPCOperation()
        {
            this.ArgTokens = new List<IDPCToken>();
        }
        public IDPCToken OperationToken { get; set; }
        public List<IDPCToken> ArgTokens { get; private set; }


    }

    public static class DPCOperationExtensions
    {
        public static List<DPCOperation> TokenizeToDPCOperations(this string text)
        {
            var rv = new List<DPCOperation>();

            var tokens = text.Tokenize(new ToDotParser());

            DPCOperation currentOperation = null;
                    
            for (int i = 0; i < tokens.Count; i++)
            {
                var each = tokens[i];

                if (!(each is IDPCToken))
                    continue;

                DPCDecoration eachToken = each as DPCDecoration;
                if (eachToken.TokenType == DPCTokenType.Operation)
                {
                    //create a new operation
                    currentOperation = new DPCOperation();
                    currentOperation.OperationToken = eachToken;
                    rv.Add(currentOperation); //add a ref upon creation
                }
                else if (eachToken.TokenType == DPCTokenType.Item)
                {
                    currentOperation.ArgTokens.Add(eachToken);
                }
            }

            return rv;
        }
    }
}
