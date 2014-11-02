using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Communicating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Messaging.StoreProtocoling
{
    public static class Extensions
    {
        /// <summary>
        /// Fluently sets endpoint logic
        /// </summary>
        /// <param name="host"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        public static IEndPointHost HasLogic(this IEndPointHost host, LogicOfTo<string, string> logic)
        {
            Condition.Requires(host).IsNotNull();
            Condition.Requires(logic).IsNotNull();
            host.Logic = logic;
            return host;
        }
    }
}
