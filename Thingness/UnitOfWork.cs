using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Thingness
{

    /// <summary>
    /// State holder for a function result
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class UnitOfWork<TArg, TRes>
    {
        #region Ctor
        public UnitOfWork(TArg arg)
        {
            this.Input = arg;
        }
        #endregion

        #region Properties
        /// <summary>
        /// can be nullable
        /// </summary>
        public TArg Input { get; private set; }
        /// <summary>
        /// the result of the function
        /// </summary>
        public TRes Result { get; private set; }
        /// <summary>
        /// the error of the function
        /// </summary>
        public Exception Error { get; private set; }
        #endregion

        #region Methods
        public void MarkResult(TRes res)
        {
            if (this.Error != null) { throw new InvalidOperationException("is immutable now"); }
            this.Result = res;
        }
        public void MarkError(Exception ex)
        {
            if (this.Result != null) { throw new InvalidOperationException("is immutable now"); }
            this.Error = ex;
        }
        #endregion
    }
}
