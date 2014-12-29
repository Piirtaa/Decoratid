using Decoratid.Core.Identifying;
using Decoratid.Storidioms.Evicting;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Tasking
{
    
    /// <summary>
    /// the task states
    /// </summary>
    public enum DecoratidTaskStatusEnum { Pending, InProcess, Cancelled, Complete, Errored }

    /// <summary>
    /// the task state transitions
    /// </summary>
    public enum DecoratidTaskTransitionEnum { Perform, MarkComplete, MarkErrored, Cancel }

    /// <summary>
    /// a task.   follows a statemachine.
    /// </summary>
    /// <remarks>
    /// The state machine graph is:
    /// Pending->Cancelled via Cancel()
    /// Pending->InProcess via Perform()
    /// InProcess->Complete via MarkComplete()
    /// InProcess->Errored via MarkErrored()
    /// InProcess->Cancelled via Cancel()
    /// 
    /// -IHasId -> persistence
    /// </remarks>
    public interface ITask : IHasId<string>
    {
        
        //actual transition methods
        bool PerformTask();
        bool CancelTask();
        bool MarkTaskComplete();
        bool MarkTaskError(Exception ex);
        DecoratidTaskStatusEnum Status { get; }
        Exception Error { get; }

        /// <summary>
        /// reference to the store of tasks this task belongs to
        /// </summary>
        ITaskStore TaskStore { get; set; }
    }

    /// <summary>
    /// a task store is a store that is: storeof itask, unique id constrained, and evicting 
    /// </summary>
    public interface ITaskStore : IStoreOf<ITask>, IStoreOfUniqueId, IEvictingStore
    {
    }

}
