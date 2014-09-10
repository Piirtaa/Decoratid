using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Dependencies;
using Decoratid.Thingness.Idioms.Logics;

namespace Decoratid.Messaging.StoreProtocol.OperationProtocol
{
    /// <summary>
    /// defines an the operation
    /// </summary>
    /// <remarks>
    /// ResponseStore must be decorated as IStoreOfUniqueId as this is a core requirement for the 
    /// OperationProtocol to run.
    /// </remarks>
    public interface IOperation : IHasId<string>, IHasDependencyOf<string>
    {
        Type ResponseType { get; }
        Type RequestType { get; }

        /// <summary>
        /// the logic we execute.  Because it's a "protocol" we specify an implementation (eg. ILogic) here.
        /// </summary>
        ICloneableLogic PerformLogic { get; }
    }

    /// <summary>
    /// An implementation of IOperation that wraps FunctionOfLogic 
    /// </summary>
    /// <typeparam name="TReq"></typeparam>
    /// <typeparam name="TResp"></typeparam>
    [Serializable]
    public class Operation<TReq, TResp> : IOperation
    {
        #region Ctor
        public Operation(string id, LogicOfTo<TReq, TResp> logic, params string[] prereqOperations)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
            this.Dependency = new DependencyOf<string>(id);
            this.Dependency.Prerequisites.AddRange(prereqOperations);
            this.PerformLogic = logic;
        }
        public Operation(string id, Func<TReq, TResp> logic, params string[] prereqOperations)
            : this(id, logic.MakeLogicOfTo(), prereqOperations)
        {
        }
        #endregion

        #region Properties
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        public IDependencyOf<string> Dependency { get; protected set; }
        public ICloneableLogic PerformLogic { get; protected set; }
        public Type ResponseType { get { return typeof(TReq); } }
        public Type RequestType { get { return typeof(TResp); } }
        #endregion

        #region Static Fluent Methods
        public static Operation<TReq, TResp> New(string id, LogicOfTo<TReq, TResp> logic, params string[] prereqOperations)
        {
            return new Operation<TReq, TResp>(id, logic, prereqOperations);
        }
        public static Operation<TReq, TResp> New(string id, Func<TReq, TResp> logic, params string[] prereqOperations)
        {
            return new Operation<TReq, TResp>(id, logic, prereqOperations);
        }
        #endregion
    }
}
