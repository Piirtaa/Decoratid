using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Storidioms.ItemValidating;
using Decoratid.Storidioms.StoreOf;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// adds a commit validator to a store
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public static ValidatingDecoration ItemValidating(this IStore decorated, IItemValidator validator)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ValidatingDecoration(decorated, validator);
        }
    }



}
