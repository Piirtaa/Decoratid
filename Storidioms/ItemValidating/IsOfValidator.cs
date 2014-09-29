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
    /// validates that an IHasId is of the passed type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IsOfValidator : IItemValidator
    {
        public IsOfValidator(Type type)
        {
            Condition.Requires(type).IsNotNull();
            this.OfType = type;

            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;

                return type.IsAssignableFrom(x.GetType());
            });
        }

        public Type OfType { get; private set; }
        [DataMember]
        public IConditionOf<IHasId> IsValidCondition { get; private set; }

        #region Static Methods
        public static IsOfValidator New(Type ofType)
        {
            return new IsOfValidator(ofType);
        }
        public static IsOfValidator New<T>()
        {
            return new IsOfValidator(typeof(T));
        }
        #endregion
    }
  
}
