using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Communication.Decorations.Throttle;

namespace Sandbox.Communication.Implementations
{
    public static class HostFactory
    {
        /// <summary>
        /// returns a newly instantiated host with the given throttling.  The host is in the Uninitialized state.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="logic"></param>
        /// <param name="maxConnections"></param>
        /// <returns></returns>
        public static IEndPointHost BuildThrottledHost(int port, IEndPointLogic logic, int maxConnections)
        {
            var builder = EndPointHostBuilder.Instance.GetBuilder();
            var ep = EndPointHelper.GetLocalEndPoint(port);
            var throttleLogic = ThrottleLogic(logic, maxConnections);
            var host = builder.BuildEndPointHost(ep, throttleLogic);

            return host;
        }
        /// <summary>
        /// returns a newly instantiated host with the given throttling.  The host is in the Uninitialized state.
        /// </summary>
        /// <param name="wireBuilderId"></param>
        /// <param name="port"></param>
        /// <param name="logic"></param>
        /// <param name="maxConnections"></param>
        /// <returns></returns>
        public static IEndPointHost BuildThrottledHost(string wireBuilderId, int port, IEndPointLogic logic, int maxConnections)
        {
            var builder = EndPointHostBuilder.Instance.GetBuilder(wireBuilderId);
            var ep = EndPointHelper.GetLocalEndPoint(port);
            var throttleLogic = ThrottleLogic(logic, maxConnections);
            var host = builder.BuildEndPointHost(ep, throttleLogic);

            return host;
        }

        /// <summary>
        /// adds throttling to the end point logic
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="maxConnections"></param>
        /// <returns></returns>
        private static IThrottlingEndPointLogic ThrottleLogic(IEndPointLogic logic, int maxConnections)
        {
            return new ThrottlingEndPointHostDecoration(logic, maxConnections);
        }
    }
}
