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
    /// validates that an IHasId has an id of the provided type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IdIsOfValidator : IItemValidator
    {
        public IdIsOfValidator(Type type)
        {
            Condition.Requires(type).IsNotNull();
            this.OfType = type;

            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;
                if (x.Id == null)
                    return false;

                return type.IsAssignableFrom(x.Id.GetType());
            });
        }

        public Type OfType { get; private set; }
        [DataMember]
        public IConditionOf<IHasId> IsValidCondition { get; private set; }

        #region Static Methods
        public static IdIsOfValidator New(Type ofType)
        {
            return new IdIsOfValidator(ofType);
        }
        public static IdIsOfValidator New<T>()
        {
            return new IdIsOfValidator(typeof(T));
        }
        #endregion
    }
  
}
