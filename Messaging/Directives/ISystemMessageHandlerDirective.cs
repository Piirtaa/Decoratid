using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.SystemBus.Directives
{
    /// <summary>
    /// interface defining directives - instructions on how to process messages with the provided handlers.
    /// </summary>
    /// <remarks>
    /// It uses the handlers themselves to process messages, but will alter which handlers to use (ie. filter them)
    /// , and execute the handlers in a specific order (ie. sequence them, and execute them),
    /// and finally to roll up the results (ie. touch the results).
    /// 
    /// This is useful in doing a step by step process with well known handlers (eg. alert Actor A, if not available, then Actor B, etc).
    /// </remarks>
    public interface ISystemMessageHandlerDirective<TArg, TResult>
    {
        /// <summary>
        /// handles all the messages, and returns the results
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handlers">use the results of FilterHandlers</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        List<SystemMessageResponse<TResult>> HandleMessage(List<ISystemMessageHandler<TArg, TResult>> handlers, TArg msg);

    }
}
