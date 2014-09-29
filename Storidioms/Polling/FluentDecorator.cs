using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Storidioms.Polling;
using Decoratid.Storidioms;
using Decoratid.Core.Logical;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
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
