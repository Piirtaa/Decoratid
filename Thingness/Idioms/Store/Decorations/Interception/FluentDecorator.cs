using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store.Decorations.Intercepting;
using Decoratid.Thingness.Idioms.Store.Decorations;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Thingness.Idioms.Store namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// gets the interception layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static InterceptingDecoration GetInterceptingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<InterceptingDecoration>(true);
        }
        /// <summary>
        /// adds interception to the store
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static InterceptingDecoration DecorateWithInterception(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new InterceptingDecoration(decorated);
        }
    }
}
