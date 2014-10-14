using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.SystemBus.Directives
{
    /// <summary>
    /// with a list of handlers, executes the first - if this doesn't work execute the second - if this doesn't work, execute the third,
    /// and so forth.  N-level failover.
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class FailOverHandlerDirective<TArg, TResult> : ISystemMessageHandlerDirective<TArg, TResult>
        where TArg : class
        where TResult : class
    {
        #region Ctor
        public FailOverHandlerDirective()
        {
        }
        
        #endregion

        #region Properties
        /// <summary>
        /// if custom sorting is required use this strategy
        /// </summary>
        public Func<List<ISystemMessageHandler<TArg, TResult>>, List<ISystemMessageHandler<TArg, TResult>>> SortFunction { get; set; }
        #endregion

        #region ISysteMessageHandler
        public List<SystemMessageResponse<TResult>> HandleMessage(List<ISystemMessageHandler<TArg, TResult>> handlers, TArg msg)
        {
            List<SystemMessageResponse<TResult>> returnValue = new List<SystemMessageResponse<TResult>>();

            List<ISystemMessageHandler<TArg, TResult>> sortedHandlers = new List<ISystemMessageHandler<TArg, TResult>>();
            sortedHandlers.AddRange(handlers);

            //sort
            if (this.SortFunction != null)
            {
                sortedHandlers = this.SortFunction(handlers);
            }

            foreach (var handler in sortedHandlers)
            {
                var resp = handler.HandleMessageWithHandler(msg);
                if (resp.Error == null)
                {
                    returnValue.Add(resp);
                    break;
                }
            }

            return returnValue;
        }
        #endregion


    }
}
