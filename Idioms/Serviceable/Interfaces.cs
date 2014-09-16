using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Serviceable
{
    /// <summary>
    /// a service's state transition triggers
    /// </summary>
    public enum ServiceTriggersEnum
    {
        Initialize,
        Start,
        Stop
    }
    /// <summary>
    /// a service's states
    /// </summary>
    public enum ServiceStateEnum
    {
        Uninitialized,
        Initialized,
        Started,
        Stopped
    }
    /// <summary>
    /// interface defining service behaviour
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Implements an initialization phase, post construction.  Must be explicitly called.
        /// </summary>
        /// <returns></returns>
        bool Initialize();
        bool Start();
        bool Stop();
        ServiceStateEnum CurrentState { get; }
    }
}
