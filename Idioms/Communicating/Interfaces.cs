using Decoratid.Idioms.Serviceable;
using System;

namespace Decoratid.Idioms.Communicating
{
    ///// <summary>
    ///// builds endpoint hosts(eg. wcf, servicestack, tcp, ice, etc)
    ///// </summary>
    //public interface IEndPointHostBuilder : IHasId<string>
    //{
    //    IEndPointHost BuildEndPointHost(EndPoint endpoint, IEndPointLogic service);
    //    IEndPointClient BuildClient(EndPoint endpoint);
    //}

    /// <summary>
    /// defines the logic that is being performed at the endpoint
    /// </summary>
    public interface IEndPointLogic 
    {
        string HandleRequest(string request);
    }

    /// <summary>
    /// a service host(ie. it initializes, starts and stops) on an endpoint that contains logic for request/response handling.
    /// </summary>
    public interface IEndPointHost : IService, IDisposable
    {
        /// <summary>
        /// the server endpoint
        /// </summary>
        EndPoint EndPoint { get; }
        IEndPointLogic Logic { get; }
    }

    public interface IEndPointClient : IDisposable
    {
        /// <summary>
        /// the server endpoint
        /// </summary>
        EndPoint EndPoint { get; }
        string Send(string request);
    }
}
