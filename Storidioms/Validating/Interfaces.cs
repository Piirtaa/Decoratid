using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.Validating
{
    /// <summary>
    /// defines condition for validity of an item (eg. in StoreOf, does the item meet the "is one Of" condition)
    /// </summary>
    public interface IItemValidator
    {
        IConditionOf<IHasId> IsValidCondition { get; }
    }
}
