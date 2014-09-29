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

}
