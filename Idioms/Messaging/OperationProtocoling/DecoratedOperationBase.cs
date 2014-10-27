using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Tasking;
using System;
using System.Linq;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
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
        public Type ResponseType
        {
            get { return this.Decorated.ResponseType; }
        }

        public Type RequestType
        {
            get { return this.Decorated.RequestType; }
        }

        public Core.Logical.ICloneableLogic PerformLogic
        {
            get { return this.Decorated.PerformLogic; }
        }

        public virtual ITask GetPerformTask(IStore requestStore, IStore responseStore)
        {
            return this.Decorated.GetPerformTask(requestStore, responseStore);
        }       
        public bool IsRequested(IStore requestStore)
        {
            return this.Decorated.IsRequested(requestStore);
        }
        #endregion



    }
}
