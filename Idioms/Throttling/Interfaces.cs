using Decoratid.Core.Conditional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Throttling
{
    /// <summary>
    /// throttles the operation
    /// </summary>
    public interface IThrottle
    {
        int ConcurrencyLimit { get; }
        /// <summary>
        /// reset the count of current connections
        /// </summary>
        void Reset();
        void Perform(Action action);
    }

    public interface IHasThrottle : IThrottle
    {
        IThrottle Throttle { get; }
    }
}
