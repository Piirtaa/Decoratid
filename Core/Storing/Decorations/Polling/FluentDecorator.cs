using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing.Decorations.Polling;
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
