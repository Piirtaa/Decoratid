using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Extensions;

namespace Sandbox.SystemBus.Directives
{
    /// <summary>
    /// is a system message handler that wraps other handlers and parallelizes them
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ParallelHandlerDirective<TArg, TResult> : ISystemMessageHandlerDirective<TArg, TResult>
        where TArg : class
        where TResult : class
    {
        #region Ctor
        public ParallelHandlerDirective()
        {
        }
        #endregion


        #region ISysteMessageHandler        
        public List<SystemMessageResponse<TResult>> HandleMessage(List<ISystemMessageHandler<TArg, TResult>> handlers, TArg msg)
        {
            List<SystemMessageResponse<TResult>> returnValue = new List<SystemMessageResponse<TResult>>();

            List<Action> actions = new List<Action>();

            handlers.WithEach(handler =>
            {
                Action action = () =>
                {
                    var res = handler.HandleMessageWithHandler<TArg, TResult>(msg);

                    if (res != null)
                    {
                        returnValue.Add(res);
                    }
                };

                actions.Add(action);

            });

            Parallel.Invoke(actions.ToArray());

            return returnValue;
        }
        #endregion


    }
}
