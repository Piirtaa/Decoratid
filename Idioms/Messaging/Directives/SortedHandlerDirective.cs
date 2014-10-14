using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.SystemBus.Directives
{
    /// <summary>
    /// with a list of handlers, sorts and executes each in order
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SortedHandlerDirective<TArg, TResult> : DecoratedHandlerDirectiveBase<TArg, TResult>
        where TArg : class
        where TResult : class
    {
        #region Ctor
        public SortedHandlerDirective(ISystemMessageHandlerDirective<TArg, TResult> decorated) :base(decorated)
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
        protected override List<SystemMessageResponse<TResult>> ActualHandleMessage(List<ISystemMessageHandler<TArg, TResult>> handlers, TArg msg)
        {
            List<SystemMessageResponse<TResult>> returnValue = new List<SystemMessageResponse<TResult>>();

            List<ISystemMessageHandler<TArg, TResult>> sortedHandlers = new List<ISystemMessageHandler<TArg, TResult>>();
            sortedHandlers.AddRange(handlers);

            //sort
            if (this.SortFunction != null)
            {
                sortedHandlers = this.SortFunction(handlers);
            }

            //wrap it
            returnValue = this.Decorated.HandleMessage(sortedHandlers, msg);

            return returnValue;
        }
        #endregion


    }
}
