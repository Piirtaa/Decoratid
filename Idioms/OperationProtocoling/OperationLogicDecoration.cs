using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.StateMachining;
using Decoratid.Idioms.Tasking;
using System;
using System.Runtime.Serialization;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Idioms.OperationProtocoling
{
    /// <summary>
    /// decorates logic as IOperation (that is, OfTo logic that hydrates from a store) 
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class OperationLogicDecoration<TArg, TResult> : DecoratedLogicBase, IOperation
    {
        #region Declarations
        protected readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public OperationLogicDecoration(LogicOfTo<TArg,TResult> decorated, string operationName)
            : base(decorated)
        {
            Condition.Requires(operationName).IsNotNullOrEmpty();
            this.Id = operationName;

        }
        #endregion

        #region Properties
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        public Type ArgumentType { get { return typeof(TResult); } }
        public Type ResultType { get { return typeof(TArg); } }
        public ILogic OperationLogic { get { return this.Decorated; } }
        #endregion

        #region Methods
        public ITask GetTask(IStore requestStore, IStore responseStore)
        {
            var task = StrategizedTask.New(this.Id, Logic.New(() =>
            {
                this.PerformOperation(requestStore, responseStore);
            }));
            return task;
        }
        private void PerformOperation(IStore requestStore, IStore responseStore)
        {
            Condition.Requires(requestStore).IsNotNull();
            Condition.Requires(responseStore).IsNotNull();

            var req = requestStore.Get<OperationRequest>(this.Id);
            TArg reqObj = (TArg)req.Data;
            try
            {
                LogicOfTo<TArg, TResult> logic = (LogicOfTo<TArg, TResult>)this.OperationLogic;

                var resp = logic.CloneAndPerform(reqObj.AsNaturalValue());
                responseStore.SaveItem(OperationResponse.New(this.Id, resp));
            }
            catch (Exception ex)
            {
                responseStore.SaveItem(OperationError.New(this.Id, ex));
            }
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new OperationLogicDecoration<TArg, TResult>(thing as LogicOfTo<TArg, TResult>, this.Id);
        }
        #endregion
    }

    public static class OperationLogicDecorationExtensions
    {
        public static OperationLogicDecoration<TArg, TResult> Operating<TArg, TResult>(this LogicOfTo<TArg, TResult> decorated, string operationName)
        {
            Condition.Requires(decorated).IsNotNull();
            return new OperationLogicDecoration<TArg, TResult>(decorated, operationName);
        }

    }
}
