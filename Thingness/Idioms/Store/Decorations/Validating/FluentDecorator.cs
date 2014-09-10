using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store.Decorations.Validating;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Thingness.Idioms.Store namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// adds a commit validator to a store
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public static ValidatingDecoration DecorateWithValidation(this IStore decorated, IItemValidator validator)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ValidatingDecoration(decorated, validator);
        }
    }



}
