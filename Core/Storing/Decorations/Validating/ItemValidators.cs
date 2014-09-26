using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;

namespace Decoratid.Idioms.Storing.Decorations.Validating
{
    /// <summary>
    /// defines condition for validity of an item (eg. in StoreOf, does the item meet the "is one Of" condition)
    /// </summary>
    public interface IItemValidator
    {
        IConditionOf<IHasId> IsValidCondition { get; }
    }

    /// <summary>
    /// validates that an IHasId is of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class IsOfValidator<T> : IItemValidator
    {
        public IsOfValidator()
        {
            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;

                return typeof(T).IsAssignableFrom(x.GetType());
            });
        }
        [DataMember]
        public IConditionOf<IHasId> IsValidCondition { get; private set; }
    }
    /// <summary>
    /// validates that an IHasId is exactly of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class IsExactlyOfValidator<T> : IItemValidator
    {
        public IsExactlyOfValidator()
        {
            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;

                return typeof(T).Equals(x.GetType());
            });
        }
        [DataMember]
        public IConditionOf<IHasId> IsValidCondition { get; private set; }
    }

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
