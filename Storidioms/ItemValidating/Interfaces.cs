using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.ItemValidating
{
    /// <summary>
    /// defines condition for validity of an item (eg. in StoreOf, does the item meet the "is one Of" condition)
    /// </summary>
    public interface IItemValidator
    {
        /// <summary>
        /// The condition that items are evaluated against
        /// </summary>
        /// <remarks>
        /// this member is functionally equivalent to the method signature: bool ValidateItem(IHasId item)
        /// using an ICondition rather than a method opens up the decoration facilities
        /// </remarks>
        IConditionOf<IHasId> IsValidCondition { get; }
    }

    /// <summary>
    /// a validator that delegates to another, aggregated validator
    /// </summary>
    public interface IHasItemValidator : IItemValidator
    {
        IItemValidator Validator { get; }
    }
}
