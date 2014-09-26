using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing.Decorations.Factoried;

using Decoratid.Idioms.Storing.Decorations;
using Decoratid.Core.Logical;

namespace Decoratid.Idioms.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Idioms.Storing namespace.
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
