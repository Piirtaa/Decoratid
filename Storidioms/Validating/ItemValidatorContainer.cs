using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;

namespace Decoratid.Storidioms.Validating
{

    /// <summary>
    /// basically a blank validator where one supplies the validating condition
    /// </summary>
    [Serializable]
    public class ItemValidatorContainer : IItemValidator
    {
        #region Ctor
        public ItemValidatorContainer(IConditionOf<IHasId> condition)
        {
            Condition.Requires(condition).IsNotNull();
            this.IsValidCondition = condition;
        }
        #endregion

        #region Properties
        public IConditionOf<IHasId> IsValidCondition { get; private set; }
        #endregion

        #region Static Methods
        public static ItemValidatorContainer New(IConditionOf<IHasId> condition)
        {
            return new ItemValidatorContainer(condition);
        }
        #endregion
    }
}
