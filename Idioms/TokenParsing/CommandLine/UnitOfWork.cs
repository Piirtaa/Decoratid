using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.CommandLine
{
    /// <summary>
    /// common interface shared by arg tokens (eg. primitives) and UnitOfWork
    /// </summary>
    public interface ICanEval
    {
        object Evaluate();
    }

    /// <summary>
    /// the compilation process will create a bunch of unit of works as it moves forward thru the tokens
    /// </summary>
    public class UnitOfWork : ICanEval
    {
        #region Ctor
        public UnitOfWork()
        {
            this.ArgTokens = new List<ICanEval>();
        }
        #endregion

        #region Fluent Static
        public static UnitOfWork New()
        {
            return new UnitOfWork();
        }
        #endregion

        #region Token Properties
        public IToken OperandToken { get; set; }
        public IToken OperationToken { get; set; }
        public List<ICanEval> ArgTokens { get; private set; }
        #endregion

        #region Placeholders
        public object Operand { get; set; }
        public object Result { get; set; }
        public Func<object, List<object>, object> Function { get; set; }
        #endregion

        #region ICanEval
        public object Evaluate()
        {
            if (this.Result == null)
            {
                var args = this.GetArgs();

                this.Result = this.Function(this.Operand, args); 
            }
            return this.Result;
        }
        private List<object> GetArgs()
        {
            List<object> rv = new List<object>();

            foreach (ICanEval each in this.ArgTokens)
            {
                var val = each.Evaluate();
                rv.Add(val);
            }
            return rv;
        }
        #endregion
    }
}
