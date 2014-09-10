using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store.Decorations.Factoried;

using Decoratid.Thingness.Idioms.Store.Decorations;
using Decoratid.Thingness.Idioms.Logics;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Thingness.Idioms.Store namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// gets the factory layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static FactoryDecoration GetFactoryDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<FactoryDecoration>(true);
        }

        /// <summary>
        /// provides a factory on the get if an item doesn't exist
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static FactoryDecoration DecorateWithFactory(this IStore decorated, LogicOfTo<IStoredObjectId, IHasId> factory)
        {
            Condition.Requires(decorated).IsNotNull();
            return new FactoryDecoration(factory, decorated);
        }
    }
}
