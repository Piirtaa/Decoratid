using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Store.Decorations.Named
{
    /// <summary>
    /// Decorates a store with IHasId
    /// </summary>
    public interface IHasIdStore : IStoreDecoration, IHasId<SandboxId>
    {

    }
}
