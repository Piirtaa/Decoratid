using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
{
    public class Evalable : ICanEval
    {
                #region Ctor
        public UnitOfWork()
        {
            this.Args = new List<ICanEval>();
        }
        #endregion

        #region Fluent Static
        public static UnitOfWork New()
        {
            return new UnitOfWork();
        }
        #endregion

        #region Token Properties
        public IStringToken OperandToken { get; set; }
        public IStringToken OperationToken { get; set; }
        public List<ICanEval> Args { get; private set; }
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
                var args = this.EvaluateArgs();

                this.Result = this.Function(this.Operand, args); 
            }
            return this.Result;
        }

        private List<object> EvaluateArgs()
        {
            List<object> rv = new List<object>();

            foreach (ICanEval each in this.Args)
            {
                var val = each.Evaluate();
                rv.Add(val);
            }
            return rv;
        }
        #endregion
    }
}
