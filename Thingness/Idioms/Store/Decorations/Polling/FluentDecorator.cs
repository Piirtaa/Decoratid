using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store.Decorations.Polling;
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
        /// gets the first (exact type) PollDecoration 
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static PollDecoration GetPollingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<PollDecoration>(true);
        }
        /// <summary>
        /// adds a background action 
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="backgroundAction"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <returns></returns>
        public static PollDecoration DecorateWithPolling(this IStore decorated, LogicOf<IStore> backgroundAction, double backgroundIntervalMSecs = 30000)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PollDecoration(decorated, backgroundIntervalMSecs, backgroundAction);
        }

    }



}
