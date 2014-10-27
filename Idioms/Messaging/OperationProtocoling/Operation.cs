using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Depending;
using System;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Tasking;
using Decoratid.Idioms.Tasking.Decorations;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
{
    /// <summary>
    /// An implementation of IOperation that wraps Logic 
    /// </summary>
    /// <typeparam name="TReq"></typeparam>
    /// <typeparam name="TResp"></typeparam>
    [Serializable]
    public class Operation<TReq, TResp> : IOperation
    {
        #region Ctor
        public Operation(string id, LogicOfTo<TReq, TResp> logic)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
            this.PerformLogic = logic;
        }
        public Operation(string id, Func<TReq, TResp> logic)
            : this(id, logic.MakeLogicOfTo())
        {
        }
        #endregion

        #region Properties
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        public ICloneableLogic PerformLogic { get; protected set; }
        public Type ResponseType { get { return typeof(TReq); } }
        public Type RequestType { get { return typeof(TResp); } }
        #endregion

        #region Methods
        public bool IsRequested(IStore requestStore)
        {
            Condition.Requires(requestStore).IsNotNull();

            var req = requestStore.Get<OperationRequest>(this.Id);
            return req != null;
        }
        public ITask GetPerformTask(IStore requestStore, IStore responseStore)
        {
            var task = StrategizedTask.New(this.Id, Logic.New(() =>
            {
                this.PerformOperation(requestStore, responseStore);
            }));
            return task;
        }
        public void PerformOperation(IStore requestStore, IStore responseStore)
        {
            Condition.Requires(requestStore).IsNotNull();
            Condition.Requires(responseStore).IsNotNull();

            var req = requestStore.Get<OperationRequest>(this.Id);
            TReq reqObj = (TReq)req.Data;
            try
            {
                LogicOfTo<TReq, TResp> logic = (LogicOfTo<TReq, TResp>)this.PerformLogic;

                var resp = logic.CloneAndPerform(reqObj.AsNaturalValue());
                responseStore.SaveItem(OperationResponse.New(this.Id, resp));
            }
            catch (Exception ex)
            {
                responseStore.SaveItem(OperationError.New(this.Id, ex));
            }
        }
        #endregion

        #region Static Fluent Methods
        public static Operation<TReq, TResp> New(string id, LogicOfTo<TReq, TResp> logic)
        {
            return new Operation<TReq, TResp>(id, logic);
        }
        public static Operation<TReq, TResp> New(string id, Func<TReq, TResp> logic)
        {
            return new Operation<TReq, TResp>(id, logic);
        }
        #endregion
    }
}
