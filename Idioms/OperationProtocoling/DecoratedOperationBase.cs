using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Tasking;
using System;
using System.Linq;

namespace Decoratid.Idioms.OperationProtocoling
{
    public interface IDecoratedOperation : IOperation, IDecorationOf<IOperation>
    {
    }

    public abstract class DecoratedOperationBase : DecorationOfBase<IOperation>, IDecoratedOperation
    {
        #region Ctor
        public DecoratedOperationBase(IOperation decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Overrides
        public override IOperation This { get { return this; } }

        public string Id
        {
            get { return this.Decorated.Id; }
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        public Type ArgumentType
        {
            get { return this.Decorated.ArgumentType; }
        }

        public Type ResultType
        {
            get { return this.Decorated.ResultType; }
        }

        public ILogic OperationLogic
        {
            get { return this.Decorated.OperationLogic; }
        }

        public virtual ITask GetTask(IStore requestStore, IStore responseStore)
        {
            return this.Decorated.GetTask(requestStore, responseStore);
        }       
        #endregion



    }
}
