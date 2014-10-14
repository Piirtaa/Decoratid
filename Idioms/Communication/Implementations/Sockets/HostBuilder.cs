using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Sandbox.Communication.Implementations.Sockets
{



    public class HostBuilder : IEndPointHostBuilder, IIPFilteredEndpointHostBuilder
    {
        #region Declarations
        public const string BUILDER_ID = "Simple";
        #endregion

        #region Ctor
        public HostBuilder()
        {
        }
        #endregion

        #region IEndpointWireBuilder
        public IEndPointHost BuildEndPointHost(EndPoint endpoint, IEndPointLogic logic)
        {
            return new Host(endpoint, logic, null);
        }

        public IIPFilteringEndPointHost BuildFilteredEndPointHost(EndPoint endpoint, IEndPointLogic logic, Func<System.Net.IPEndPoint, bool> validateClientEndPointStrategy)
        {
            return new Host(endpoint, logic, validateClientEndPointStrategy);
        }
        public IEndPointHost BuildEndPointHost(EndPoint endpoint, IEndPointLogic logic, Func<System.Net.IPEndPoint, bool> validateClientEndPointStrategy)
        {
            return new Host(endpoint, logic, validateClientEndPointStrategy);
        }

        public IEndPointClient BuildClient(EndPoint endpoint)
        {
            return new Client(endpoint);
        }
        #endregion

        #region IHasId
        public string Id { get { return BUILDER_ID; } }
        object Store.IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        /// <summary>
        /// builds a host on the Loopback address (with next avail port) that ONLY allows clients also
        /// coming from the Loopback.  This is great for local machine IPC
        /// </summary>
        /// <param name="logic"></param>
        /// <returns></returns>
        public IEndPointHost BuildNextAvailableLocalOnlyHost(IEndPointLogic logic)
        {
            //get the next available local ip 
            var ep = EndPointHelper.GetNextFreeLoopbackEndPoint();
            Condition.Requires(ep).IsNotNull();

            Func<System.Net.IPEndPoint, bool> validator = (x) =>
            {
                return x.Address.Equals(System.Net.IPAddress.Loopback);
            };

            return this.BuildEndPointHost(ep, logic, validator);
        }
        #endregion

    }
}
