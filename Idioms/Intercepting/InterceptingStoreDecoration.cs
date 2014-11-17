using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Logging;
using Decoratid.Storidioms;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Intercepting
{
    /// <summary>
    /// wraps intercepts around store operations
    /// </summary>
    public interface IInterceptingStore : IDecoratedStore
    {
        #region Intercepts
        /// <summary>
        /// intercept for Get operation.  Takes arg of Tuple:Type,IHasId (eg. Get of X), returns IHasId 
        /// </summary>
        InterceptChain<IStoredObjectId, IHasId> GetOperationIntercept { get; set; }
        /// <summary>
        /// intercept for Search operation.  Takes arg of Tuple:Type,SearchFilter (eg. Search of X), returns list
        /// </summary>
        InterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept { get; set; }
        /// <summary>
        /// intercept for GetAll operation.  Takes nothing, returns list
        /// </summary>
        InterceptChain<Nothing, List<IHasId>> GetAllOperationIntercept { get; set; }
        /// <summary>
        /// intercept for commit operation.  Takes arg of CommitBag, returns nothing
        /// </summary>
        InterceptChain<ICommitBag, Nothing> CommitOperationIntercept { get; set; }
        #endregion
    }

    /// <summary>
    /// provides hooks on a store's methods via InterceptChain
    /// </summary>
    /// <remarks>
    /// 
    [Serializable]
    public class InterceptingStoreDecoration : DecoratedStoreBase, IInterceptingStore, IHasLogger
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// ctor.  requires IStore to wrap
        /// </summary>
        /// <param name="decorated"></param>
        public InterceptingStoreDecoration(IStore decorated, ILogger logger)
            : base(decorated)
        {
            this.Logger = logger;

            //init the intercept chains, wrapping them around the core operations
            this.CommitOperationIntercept = new InterceptChain<ICommitBag, Nothing>((bag) =>
            {
                this.Decorated.Commit(bag);
                return Nothing.VOID;
            });
            this.CommitOperationIntercept.Completed += CommitOperationIntercept_Completed;

            this.GetOperationIntercept = new InterceptChain<IStoredObjectId, IHasId>((x) =>
            {
                return this.Decorated.Get(x);
            });
            this.GetOperationIntercept.Completed += GetOperationIntercept_Completed;

            this.GetAllOperationIntercept = new InterceptChain<Nothing, List<IHasId>>((x) =>
            {
                return this.Decorated.GetAll();
            });
            this.GetAllOperationIntercept.Completed += GetAllOperationIntercept_Completed;
            
            this.SearchOperationIntercept = new InterceptChain<Tuple<Type, SearchFilter>, List<IHasId>>((x) =>
            {
                return this.Decorated.Search_NonGeneric(x.Item1, x.Item2);
            });
            this.SearchOperationIntercept.Completed += SearchOperationIntercept_Completed;
        }
        #endregion
        
        #region ISerializable
        protected InterceptingStoreDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasLogger
        public ILogger Logger { get; set; }
        #endregion

        #region Properties
        protected InterceptingStoreDecoration InterceptCore
        {
            get
            {
                return this.FindDecoratorOf<InterceptingStoreDecoration>(true);
            }
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new InterceptingStoreDecoration(store, this.Logger);

            return returnValue;
        }
        #endregion

        #region IInterceptingStore
        public InterceptChain<IStoredObjectId, IHasId> GetOperationIntercept { get; set; }
        public InterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept { get; set; }
        public InterceptChain<ICommitBag, Nothing> CommitOperationIntercept { get; set; }
        public InterceptChain<Nothing, List<IHasId>> GetAllOperationIntercept { get; set; }
        #endregion

        #region IStore Overrides
        public override IHasId Get(IStoredObjectId soId)
        {
            return this.GetOperationIntercept.Perform(soId, this.Logger);
        }
        public override List<T> Search<T>(SearchFilter filter)
        {
            //execute it
            var list = this.SearchOperationIntercept.Perform(new Tuple<Type, SearchFilter>(typeof(T), filter), this.Logger);

            List<T> returnValue = new List<T>();
            //convert to list of T
            list.WithEach(x =>
            {
                returnValue.Add((T)x);
            });
            return returnValue;
        }
        public override void Commit(ICommitBag bag)
        {
            this.CommitOperationIntercept.Perform(bag, this.Logger);
        }
        public override List<IHasId> GetAll()
        {
            return this.GetAllOperationIntercept.Perform(Nothing.VOID, this.Logger);
        }
        #endregion

        #region UoW Hooks- Event Handlers
        public virtual void SearchOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<Tuple<Type, SearchFilter>, List<IHasId>>> e)
        {

        }

        public virtual void GetOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<IStoredObjectId, IHasId>> e)
        {

        }

        public virtual void CommitOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<ICommitBag, Nothing>> e)
        {

        }

        public virtual void GetAllOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<Nothing, List<IHasId>>> e)
        {

        }
        #endregion

        #region Disposable
        protected override void DisposeManaged()
        {
            base.DisposeManaged();
        }
        #endregion
    }

    public static class InterceptingStoreDecorationExtensions
    {
        /// <summary>
        /// gets the interception layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static InterceptingStoreDecoration GetIntercept(this IStore decorated)
        {
            return decorated.FindDecoratorOf<InterceptingStoreDecoration>(true);
        }
        /// <summary>
        /// adds interception to the store
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static InterceptingStoreDecoration Intercepting(this IStore decorated, ILogger logger)
        {
            Condition.Requires(decorated).IsNotNull();
            return new InterceptingStoreDecoration(decorated, logger);
        }
        public static InterceptingStoreDecoration Intercepting(this ILoggingStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new InterceptingStoreDecoration(decorated, decorated.Logger);
        }
    }
}
