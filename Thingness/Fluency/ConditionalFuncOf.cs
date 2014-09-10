using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Sandbox.Thingness
{
    /// <summary>
    /// class that performs a function if conditional data is available
    /// </summary>
    /// <typeparam name="Tdata"></typeparam>
    /// <typeparam name="Tout"></typeparam>
    public class ConditionalFuncOf<Tdata, Tout>
    {
        #region Ctor
        public ConditionalFuncOf(ConditionalFactoryOf<Tdata> condData, Func<Tdata, Tout> function)
        {
            Condition.Requires(condData).IsNotNull();
            this.ConditionalData = condData;
            this.Function = function;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The condition, upon which we must call the strategy to populate the output
        /// </summary>
        public ConditionalFactoryOf<Tdata> ConditionalData { get; private set; }
        public Func<Tdata, Tout> Function { get; private set; }
        public Tout FuncResult { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// tests the condition.  if true, performs
        /// </summary>
        /// <returns></returns>
        public bool Perform()
        {
            Condition.Requires(this.ConditionalData).IsNotNull();

            bool returnValue = this.ConditionalData.GetData();

            if (returnValue && Function != null)
            {
                this.FuncResult = this.Function(this.ConditionalData.Data);
            }
           
            return returnValue;
        }
        #endregion
    }
}
