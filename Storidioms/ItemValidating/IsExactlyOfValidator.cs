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

namespace Decoratid.Storidioms.ItemValidating
{
    /// <summary>
    /// validates that an IHasId is exactly of the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IsExactlyOfValidator : IItemValidator
    {
        public IsExactlyOfValidator(Type type)
        {
            Condition.Requires(type).IsNotNull();
            this.OfType = type;

            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;

                return type.Equals(x.GetType());
            });
        }
        public Type OfType { get; private set; }
        [DataMember]
        public IConditionOf<IHasId> IsValidCondition { get; private set; }

        #region Static Methods
        public static IsExactlyOfValidator New(Type ofType)
        {
            return new IsExactlyOfValidator(ofType);
        }
        public static IsExactlyOfValidator New<T>()
        {
            return new IsExactlyOfValidator(typeof(T));
        }
        #endregion
    }

}
