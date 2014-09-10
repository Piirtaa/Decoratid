using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Sandbox.Thingness
{
    /// <summary>
    /// class that performs an action if conditional data is avail  
    /// </summary>
    public class ConditionalActionOf<T>
    {
        #region Ctor
        public ConditionalActionOf(ConditionalFactoryOf<T> condData, Action<T> action)
        {
            Condition.Requires(condData).IsNotNull();
            this.ConditionalData = condData;
            this.Action = action;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The condition, upon which we must call the strategy to populate the output
        /// </summary>
        public ConditionalFactoryOf<T> ConditionalData { get; private set; }
        public Action<T> Action { get; private set; }
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

            if (returnValue && Action != null)
            {
                this.Action(this.ConditionalData.Data);
            }

            return returnValue;
        }
        #endregion
    }
}
