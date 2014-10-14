using Decoratid.Core.Logical;
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
    /// a service host(ie. it initializes, starts and stops) on an endpoint that contains logic for request/response handling.
    /// </summary>
    public interface IEndPointHost : IService, IDisposable
    {
        /// <summary>
        /// the server endpoint
        /// </summary>
        EndPoint EndPoint { get; }
        LogicOfTo<string,string> Logic { get; }
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
