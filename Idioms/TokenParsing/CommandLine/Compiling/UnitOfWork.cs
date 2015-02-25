using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
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

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
{
    /// <summary>
    /// the compilation process will create a bunch of unit of works as it moves forward thru the tokens
    /// </summary>
    public class UnitOfWork : IToken<IToken<char>>, IHasValue
    {
        #region Ctor
        public UnitOfWork()
        {
            this.Args = null;
        }
        #endregion

        #region Fluent Static
        public static UnitOfWork New()
        {
            return new UnitOfWork();
        }
        #endregion

        #region Token Properties
        public IHasValue Operand { get; set; }
        public IToken<char> OperationToken { get; set; }
        public List<IHasValue> Args { get; private set; }
        #endregion

        #region Placeholders
        public object Result { get; set; }
        public Func<object, List<object>, object> Function { get; set; }
        #endregion

        #region IHasValue
        public object Value
        {
            get
            {
                if (this.Result == null)
                {
                    var args = this.EvaluateArgs();

                    this.Result = this.Function(this.Operand, args);
                }
                return this.Result;
            }
        }
        #endregion


        #region Helpers
        private List<object> EvaluateArgs()
        {
            List<object> rv = new List<object>();

            foreach (IHasValue each in this.Args)
            {
                var val = each.Value;
                rv.Add(val);
            }
            return rv;
        }
        #endregion

    }
}
