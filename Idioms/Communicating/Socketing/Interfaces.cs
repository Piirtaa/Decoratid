using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Communicating.Socketing
{
    /// <summary>
    /// adds an IP filtering strategy to the host
    /// </summary>
    public interface IIPFilteringEndPointHost : IEndPointHost
    {
        Func<System.Net.IPEndPoint, bool> ValidateClientEndPointStrategy { get; }
    }
    ///// <summary>
    ///// builder interface to create an IEndPointFilteringHost
    ///// </summary>
    //public interface IIPFilteredEndpointHostBuilder : IEndPointHostBuilder
    //{
    //    IIPFilteringEndPointHost BuildFilteredEndPointHost(EndPoint endpoint, IEndPointLogic service, Func<System.Net.IPEndPoint, bool> validateClientEndPointStrategy);
    //}
}
