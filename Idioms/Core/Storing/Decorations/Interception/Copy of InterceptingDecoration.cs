//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CuttingEdge.Conditions;
//using Sandbox.Thingness;
//using Sandbox.Extensions;
//using Sandbox.Interception;

//namespace Sandbox.Store.Decorations.Intercepting
//{
//    /// <summary>
//    /// provides hooks on a store's methods via InterceptChain
//    /// </summary>
//    /// <remarks>
//    public class InterceptingDecoration : AbstractDecoration, IInterceptingStore
//    {
//        #region Ctor
//        /// <summary>
//        /// ctor.  requires IStore to wrap
//        /// </summary>
//        /// <param name="decorated"></param>
//        public InterceptingDecoration(IStore decorated)
//            : base(decorated)
//        {
//            //init the intercept chains, wrapping them around the core operations
//            this.CommitOperationIntercept = new Interception.InterceptChain<CommitBag, Sandbox.Thingness.Void>((bag) =>
//            {
//                this.Decorated.Commit(bag);
//                return Sandbox.Thingness.Void.VOID;
//            });
//            this.CommitOperationIntercept.Completed += CommitOperationIntercept_Completed;
            
//            this.GetOperationIntercept = new Interception.InterceptChain<Tuple<Type,IHasId>, IHasId>((x) =>
//            {
//                return this.Decorated.GetById(x.Item1, x.Item2);
//            });
//            this.GetOperationIntercept.Completed += GetOperationIntercept_Completed;

//            this.SearchOperationIntercept = new InterceptChain<Tuple<Type, SearchFilter>, List<IHasId>>((x) =>
//            {
//                return this.Decorated.Search_NonGeneric(x.Item1, x.Item2);
//            });
//            this.SearchOperationIntercept.Completed += SearchOperationIntercept_Completed;
//        }


//        #endregion

//        #region Properties
//        protected InterceptingDecoration InterceptCore
//        {
//            get
//            {
//                return this.FindDecoratorOf<InterceptingDecoration>(true);
//            }
//        }
//        #endregion

//        #region IInterceptingStore
//        public InterceptChain<Tuple<Type, IHasId>, IHasId> GetOperationIntercept { get; set; }
//        public InterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept { get; set; }
//        public InterceptChain<CommitBag, Sandbox.Thingness.Void> CommitOperationIntercept { get; set; }
//        #endregion

//        #region IStore Overrides
//        public override T Get<T>(IHasId id) 
//        {
//            return (T)this.GetOperationIntercept.Perform(new Tuple<Type, IHasId>(typeof(T), id));
//        }
//        public override List<T> Search<T>(SearchFilter filter)
//        {
//            //execute it
//            var list = this.SearchOperationIntercept.Perform(new Tuple<Type, SearchFilter>(typeof(T), filter));

//            List<T> returnValue = new List<T>();
//            //convert to list of T
//            list.WithEach(x =>
//            {
//                returnValue.Add((T)x);
//            });
//            return returnValue;
//        }
//        public override void Commit(CommitBag bag)
//        {
//            this.CommitOperationIntercept.Perform(bag);
//        }
//        #endregion

//        #region UoW Hooks- Event Handlers
//        public virtual void SearchOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<Tuple<Type, SearchFilter>, List<IHasId>>> e)
//        {

//        }

//        public virtual void GetOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<Tuple<Type, IHasId>, IHasId>> e)
//        {

//        }

//        public virtual void CommitOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<CommitBag, Thingness.Void>> e)
//        {

//        }
//        #endregion
        


//        #region Disposable
//        protected override void DisposeManaged()
//        {
//            base.DisposeManaged();
//        }
//        #endregion
//    }
//}
