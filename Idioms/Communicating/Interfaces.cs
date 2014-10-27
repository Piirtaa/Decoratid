using Decoratid.Core.Logical;
using Decoratid.Idioms.Serviceable;
using System;

namespace Decoratid.Idioms.Communicating
{
    /// <summary>
    /// something that has an ipaddress and a port (AKA an "endpoint", mkay)
    /// </summary>
    public interface IHasEndPoint
    {
        EndPoint EndPoint { get; }
    }

    public interface IHasEndPointLogic
    {
        LogicOfTo<string,string> Logic { get; set; }
    }

    /// <summary>
    /// a service host(ie. it initializes, starts and stops) on an endpoint that contains logic for request/response handling.
    /// </summary>
    public interface IEndPointHost : IService, IDisposable, IHasEndPoint, IHasEndPointLogic
    {
    }

    public interface IEndPointClient : IHasEndPoint, IDisposable
    {
        string Send(string request);
    }

    ///// <summary>
    ///// builds endpoint hosts(eg. wcf, servicestack, tcp, ice, etc)
    ///// </summary>
    //public interface IEndPointHostBuilder : IHasId<string>
    //{
    //    IEndPointHost BuildEndPointHost(EndPoint endpoint, IEndPointLogic service);
    //    IEndPointClient BuildClient(EndPoint endpoint);
    //}
}
