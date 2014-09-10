using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.SystemBus.Directives
{
    public abstract class DecoratedHandlerDirectiveBase<TArg, TResult> : ISystemMessageHandlerDirective<TArg, TResult>
        where TArg : class
        where TResult : class
    {

        protected readonly ISystemMessageHandlerDirective<TArg, TResult> Decorated;

        public DecoratedHandlerDirectiveBase()
        {

        }

        public DecoratedHandlerDirectiveBase(ISystemMessageHandlerDirective<TArg, TResult> decorated)
        {
            this.Decorated = decorated;
        }
        
        public List<SystemMessageResponse<TResult>> HandleMessage(List<ISystemMessageHandler<TArg, TResult>> handlers, TArg msg)
        {
            return this.ActualHandleMessage(handlers, msg);
        }

        protected virtual List<SystemMessageResponse<TResult>> ActualHandleMessage(List<ISystemMessageHandler<TArg, TResult>> handlers, TArg msg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// runs the decorated handler 
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected List<SystemMessageResponse<TResult>> HandleWithDecorator(List<ISystemMessageHandler<TArg, TResult>> handlers, TArg msg)
        {
            if (this.Decorated == null)
                return null;

            return this.Decorated.HandleMessage(handlers, msg);
        }

    }
}
