using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Extensions;
using Decoratid.Idioms.Intercepting.Decorating;
using System.Runtime.Serialization;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Storidioms.Intercepting
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
        DecoratingInterceptChain<IStoredObjectId, IHasId> GetOperationIntercept { get; set; }
        /// <summary>
        /// intercept for Search operation.  Takes arg of Tuple:Type,SearchFilter (eg. Search of X), returns list
        /// </summary>
        DecoratingInterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept { get; set; }
        /// <summary>
        /// intercept for GetAll operation.  Takes nothing, returns list
        /// </summary>
        DecoratingInterceptChain<Decoratid.Thingness.Nothing, List<IHasId>> GetAllOperationIntercept { get; set; }
        /// <summary>
        /// intercept for commit operation.  Takes arg of CommitBag, returns nothing
        /// </summary>
        DecoratingInterceptChain<ICommitBag, Decoratid.Thingness.Nothing> CommitOperationIntercept { get; set; }
        #endregion
    }

    /// <summary>
    /// provides hooks on a store's methods via DecoratingInterceptChain
    /// </summary>
    /// <remarks>
    public class InterceptingDecoration : DecoratedStoreBase, IInterceptingStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// ctor.  requires IStore to wrap
        /// </summary>
        /// <param name="decorated"></param>
        public InterceptingDecoration(IStore decorated)
            : base(decorated)
        {
            //init the intercept chains, wrapping them around the core operations
            this.CommitOperationIntercept = new DecoratingInterceptChain<ICommitBag, Decoratid.Thingness.Nothing>((bag) =>
            {
                this.Decorated.Commit(bag);
                return Decoratid.Thingness.Nothing.VOID;
            });
            this.CommitOperationIntercept.Completed += CommitOperationIntercept_Completed;

            this.GetOperationIntercept = new DecoratingInterceptChain<IStoredObjectId, IHasId>((x) =>
            {
                return this.Decorated.Get(x);
            });
            this.GetOperationIntercept.Completed += GetOperationIntercept_Completed;

            this.GetAllOperationIntercept = new DecoratingInterceptChain<Thingness.Nothing, List<IHasId>>((x) =>
            {
                return this.Decorated.GetAll();
            });
            this.GetAllOperationIntercept.Completed += GetAllOperationIntercept_Completed;
            this.SearchOperationIntercept = new DecoratingInterceptChain<Tuple<Type, SearchFilter>, List<IHasId>>((x) =>
            {
                return this.Decorated.Search_NonGeneric(x.Item1, x.Item2);
            });
            this.SearchOperationIntercept.Completed += SearchOperationIntercept_Completed;
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<InterceptingDecoration>();
            hydrationMap.RegisterDefault("GetOperationIntercept", x => x.GetOperationIntercept, (x, y) => { x.GetOperationIntercept = y as DecoratingInterceptChain<IStoredObjectId, IHasId>; });
            hydrationMap.RegisterDefault("SearchOperationIntercept", x => x.SearchOperationIntercept, (x, y) => { x.SearchOperationIntercept = y as DecoratingInterceptChain<Tuple<Type, SearchFilter>, List<IHasId>>; });
            hydrationMap.RegisterDefault("CommitOperationIntercept", x => x.CommitOperationIntercept, (x, y) => { x.CommitOperationIntercept = y as DecoratingInterceptChain<ICommitBag, Decoratid.Thingness.Nothing>; });
            hydrationMap.RegisterDefault("GetAllOperationIntercept", x => x.GetAllOperationIntercept, (x, y) => { x.GetAllOperationIntercept = y as DecoratingInterceptChain<Decoratid.Thingness.Nothing, List<IHasId>>; });
            return hydrationMap;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.GetHydrationMap().DehydrateValue(this, uow);
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            this.GetHydrationMap().HydrateValue(this, text, uow);
        }
        #endregion

        #region Properties
        protected InterceptingDecoration InterceptCore
        {
            get
            {
                return this.FindDecoratorOf<InterceptingDecoration>(true);
            }
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new InterceptingDecoration(store);

            return returnValue;
        }
        #endregion

        #region IInterceptingStore
        public DecoratingInterceptChain<IStoredObjectId, IHasId> GetOperationIntercept { get; set; }
        public DecoratingInterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept { get; set; }
        public DecoratingInterceptChain<ICommitBag, Decoratid.Thingness.Nothing> CommitOperationIntercept { get; set; }
        public DecoratingInterceptChain<Decoratid.Thingness.Nothing, List<IHasId>> GetAllOperationIntercept { get; set; }
        #endregion

        #region IStore Overrides
        public override IHasId Get(IStoredObjectId soId)
        {
            return this.GetOperationIntercept.Perform(soId.AsNaturalValue());
        }
        public override List<T> Search<T>(SearchFilter filter)
        {
            //execute it
            var list = this.SearchOperationIntercept.Perform(new Tuple<Type, SearchFilter>(typeof(T), filter).AsNaturalValue());

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
            this.CommitOperationIntercept.Perform(bag.AsNaturalValue());
        }
        public override List<IHasId> GetAll()
        {
            return this.GetAllOperationIntercept.Perform(Thingness.Nothing.VOID.ValueOf());
        }
        #endregion

        #region UoW Hooks- Event Handlers
        public virtual void SearchOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<Tuple<Type, SearchFilter>, List<IHasId>>> e)
        {

        }

        public virtual void GetOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<IStoredObjectId, IHasId>> e)
        {

        }

        public virtual void CommitOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<ICommitBag, Thingness.Nothing>> e)
        {

        }

        public virtual void GetAllOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<Thingness.Nothing, List<IHasId>>> e)
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
}
