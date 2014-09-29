using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Storidioms.Eventing;
using Decoratid.Storidioms;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// gets the factory layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static EventingDecoration GetEventingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<EventingDecoration>(true);
        }

        /// <summary>
        /// adds events
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static EventingDecoration DecorateWithEvents(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new EventingDecoration(decorated);
        }
    }



}
