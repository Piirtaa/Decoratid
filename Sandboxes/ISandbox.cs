using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Conditions;
using Sandbox.Thingness;
using Sandbox.Thingness.Dependencies;
using Sandbox.Thingness.Dependencies.Named;


namespace Sandbox.Sandboxes
{
    /// <summary>
    /// defines a sandbox, which is a service that lives for the life of the application.
    /// </summary>
    /// <remarks>
    /// Sandboxes are the top-level manageable services the app(eg. the SandboxManager singleton) hosts.
    /// </remarks>
    public interface ISandbox : IService
    {
        string Name { get; }
        string GetStatus();
    }


}
