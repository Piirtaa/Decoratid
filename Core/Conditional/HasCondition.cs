using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Conditional
{
    /// <summary>
    /// concrete hasA class wrapper
    /// </summary>
    [Serializable]
    public sealed class HasCondition : IHasCondition
    {
        #region Ctor
        public HasCondition(ICondition condition = null)
        {
            this.Condition = condition;
        }
        #endregion

        #region IHasCondition
        public ICondition Condition { get; set; }
        #endregion

        #region Fluent Methods
        public HasCondition SetCondition(ICondition condition)
        {
            this.Condition = condition;
            return this;
        }
        #endregion

        #region Fluent Static
        public static HasCondition New(ICondition condition = null)
        {
            return new HasCondition(condition);
        }
        #endregion


    }
}
