using Decoratid.Core.Conditional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.ConditionalWaiting
{
    /// <summary>
    /// will delay performing operation until wait condition is met
    /// </summary>
    public interface IHasWaitCondition
    {
        ICondition WaitCondition { get; }
        ICondition StopWaitingCondition { get; }

    }

    /// <summary>
    /// defines something that waits
    /// </summary>
    public interface IConditionalWaiter : IHasWaitCondition
    {
        /// <summary>
        /// waits until the wait condition is true, returning true.  if no stop waiting condition is provided, will wait forever.
        /// if stop waiting condition is true, will return false.  
        /// </summary>
        bool WaitAround();
    }

    /// <summary>
    /// Has A interface for IConditionalWaiter
    /// </summary>
    public interface IHasConditionalWaiter : IConditionalWaiter
    {
        IConditionalWaiter Waiter { get; set; }
    }
}
