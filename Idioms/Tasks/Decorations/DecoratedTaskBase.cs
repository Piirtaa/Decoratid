using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Storing;
using Decoratid.Thingness;
using Decoratid.Idioms.Dependencies;
using Decoratid.Idioms.Decorating;

namespace Decoratid.Tasks.Decorations
{
    /// <summary>
    /// decoration interface
    /// </summary>
    public interface IDecoratedTask : ITask, IDecorationOf<ITask>
    {
    }

    /// <summary>
    /// abstract class that provides templated implementation of a Decorator/Wrapper of ITask
    /// </summary>
    public abstract class DecoratedTaskBase : DecorationOfBase<ITask>, IDecoratedTask
    {
        #region Ctor
        public DecoratedTaskBase(ITask decorated) : base(decorated)
        {
        }
        #endregion

        #region Properties
        public override ITask This { get { return this; } }
        #endregion

        #region ITask
        //readonly properties
        public DecoratidTaskStatusEnum Status { get { return this.Decorated.Status; } }
        public string Id { get { return this.Decorated.Id; } }
        object IHasId.Id { get { return this.Id; } }
        public Exception Error { get { return this.Decorated.Error; } }
        public ITaskStore TaskStore { get { return this.Decorated.TaskStore; } set { this.Decorated.TaskStore = value; } }
        //virtual methods
        public virtual bool Perform() { return this.Decorated.Perform(); }
        public virtual bool Cancel() { return this.Decorated.Cancel(); }
        public virtual bool MarkComplete() { return this.Decorated.MarkComplete(); }
        public virtual bool MarkError(Exception ex) { return this.Decorated.MarkError(ex); }
        public void SetId(string id) { this.Decorated.SetId(id); }
        void SetId(object id) { this.SetId(id as string); }
        #endregion

        #region Helpers
        protected ITask GetTask(string id)
        {
            var list = this.TaskStore.GetAllById<ITask>(id);

            return list.FirstOrDefault();
        }
        #endregion



    }
}
