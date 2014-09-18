using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Extensions;
using Decoratid.Interception;
using Decoratid.Idioms.Storing.Decorations.Intercepting;
using Decoratid.Interception.Decorating;
using System.Runtime.Serialization;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Idioms.Storing.Decorations.Eventing
{
    /// <summary>
    /// using intercept idioms, exposes a bunch of events
    /// </summary>
    public interface IEventingStore : IDecoratedStore
    {
        /// <summary>
        /// fires when an item is retrieved by either Get or Search
        /// </summary>
        event EventHandler<EventArgOf<IHasId>> ItemRetrieved;
        /// <summary>
        /// fires when an item is filtered from retrieval during either Get or Search
        /// </summary>
        event EventHandler<EventArgOf<IHasId>> ItemRetrievedFiltered;
        /// <summary>
        /// fires when an item is added
        /// </summary>
        event EventHandler<EventArgOf<IHasId>> ItemSaved;
        /// <summary>
        /// fires when an item is filtered from being added
        /// </summary>
        event EventHandler<EventArgOf<IHasId>> ItemSavedFiltered;
        /// <summary>
        /// fires when an item is removed
        /// </summary>
        event EventHandler<EventArgOf<IHasId>> ItemDeleted;
        /// <summary>
        /// fires when an item is filtered from being removed
        /// </summary>
        event EventHandler<EventArgOf<IHasId>> ItemDeletedFiltered;
    }

    /// <summary>
    /// provides IEventingStore events. 
    /// </summary>
    /// <remarks>
    /// The implementation of when the IEventingStore events are raised is VERY SPECIFIC.  ItemRetrievedFiltered events on reads
    ///  (Get, GetAll, Search) are raised if the Result is scrubbed. Conversely, ItemSavedFiltered and ItemDeletedFiltered events
    ///  are raised if the Arg is scrubbed.  
    ///  Also, this decoration will leak the filtered items via events. So it should not be used in secure applications.
    /// </remarks>
    public class EventingDecoration : DecoratedStoreBase, IEventingStore, IInterceptingStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// ctor.  requires IStore to wrap
        /// </summary>
        /// <param name="decorated"></param>
        public EventingDecoration(IStore decorated)
            : base(decorated.DecorateWithInterception())
        {
            //wire to the intercepts
            this.InterceptCore.CommitOperationIntercept.Completed += CommitOperationIntercept_Completed;
            this.InterceptCore.SearchOperationIntercept.Completed += SearchOperationIntercept_Completed;
            this.InterceptCore.GetOperationIntercept.Completed += GetOperationIntercept_Completed;
            this.InterceptCore.GetAllOperationIntercept.Completed += GetAllOperationIntercept_Completed;
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<EventingDecoration>();
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

        #region Intercept Subscriptions
        void GetOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<IStoredObjectId, IHasId>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                //if the result is nulled, fire the filtered event
                if (uow.Result.LastValue == null && uow.Result.FirstValue != null)
                {
                    this.ItemRetrievedFiltered.BuildAndFireEventArgs(uow.Result.FirstValue);
                }
                else
                {

                    this.ItemRetrieved.BuildAndFireEventArgs(uow.Result.LastValue);
                }
            }
        }

        void SearchOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<Tuple<Type, SearchFilter>, List<IHasId>>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                //examine the before and after lists to find deletions
                var origList = uow.Result.FirstValue;
                var finalList = uow.Result.LastValue;

                var delItems = origList.FindDeletedItems(finalList);

                delItems.WithEach(x =>
                {
                    this.ItemRetrievedFiltered.BuildAndFireEventArgs(x);
                });

                finalList.WithEach(x =>
                {
                    this.ItemRetrieved.BuildAndFireEventArgs(x);
                });
            }
        }
        void GetAllOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<Thingness.Nothing, List<IHasId>>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                //examine the before and after lists to find deletions
                var origList = uow.Result.FirstValue;
                var finalList = uow.Result.LastValue;

                var delItems = origList.FindDeletedItems(finalList);

                delItems.WithEach(x =>
                {
                    this.ItemRetrievedFiltered.BuildAndFireEventArgs(x);
                });

                finalList.WithEach(x =>
                {
                    this.ItemRetrieved.BuildAndFireEventArgs(x);
                });
            }
        }
        void CommitOperationIntercept_Completed(object sender, EventArgOf<DecoratingInterceptUnitOfWork<ICommitBag, Thingness.Nothing>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                var origItemsToSave = uow.Arg.FirstValue.ItemsToSave.ToList();
                var origItemsToDelete = uow.Arg.FirstValue.ItemsToDelete.ToList();
                var finalItemsToSave = uow.Arg.LastValue.ItemsToSave.ToList();
                var finalItemsToDelete = uow.Arg.LastValue.ItemsToDelete.ToList();
                var scrubbedItemsToSave = origItemsToSave.FindDeletedItems(finalItemsToSave);
                var scrubbedItemsToDel = origItemsToDelete.FindDeletedItems(finalItemsToDelete);

                scrubbedItemsToSave.WithEach(x =>
                {
                    this.ItemSavedFiltered.BuildAndFireEventArgs(x);
                });
                finalItemsToSave.WithEach(x =>
                {
                    this.ItemSaved.BuildAndFireEventArgs(x);
                });
                scrubbedItemsToDel.WithEach(x =>
                {
                    this.ItemDeletedFiltered.BuildAndFireEventArgs(x);
                });
                finalItemsToDelete.WithEach(x =>
                {
                    this.ItemDeleted.BuildAndFireEventArgs(x);
                });
            }
        }
        #endregion

        #region IInterceptingStore
        public DecoratingInterceptChain<IStoredObjectId, IHasId> GetOperationIntercept
        {
            get
            {
                return this.InterceptCore.GetOperationIntercept;
            }
            set
            {
                this.InterceptCore.GetOperationIntercept = value;
            }
        }

        public DecoratingInterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept
        {
            get
            {
                return this.InterceptCore.SearchOperationIntercept;
            }
            set
            {
                this.InterceptCore.SearchOperationIntercept = value;
            }
        }

        public DecoratingInterceptChain<ICommitBag, Thingness.Nothing> CommitOperationIntercept
        {
            get
            {
                return this.InterceptCore.CommitOperationIntercept;
            }
            set
            {
                this.InterceptCore.CommitOperationIntercept = value;
            }
        }
        public DecoratingInterceptChain<Decoratid.Thingness.Nothing, List<IHasId>> GetAllOperationIntercept
        {
            get
            {
                return this.InterceptCore.GetAllOperationIntercept;
            }
            set
            {
                this.InterceptCore.GetAllOperationIntercept = value;
            }
        }
        #endregion

        #region IEventingStore
        public event EventHandler<EventArgOf<IHasId>> ItemRetrieved;
        public event EventHandler<EventArgOf<IHasId>> ItemRetrievedFiltered;
        public event EventHandler<EventArgOf<IHasId>> ItemSaved;
        public event EventHandler<EventArgOf<IHasId>> ItemSavedFiltered;
        public event EventHandler<EventArgOf<IHasId>> ItemDeleted;
        public event EventHandler<EventArgOf<IHasId>> ItemDeletedFiltered;
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new EventingDecoration(store);

            return returnValue;
        }
        #endregion

        #region Disposable
        protected override void DisposeManaged()
        {
            //remove event subscriptions
            this.ItemRetrieved = null;
            this.ItemRetrievedFiltered = null;
            this.ItemSaved = null;
            this.ItemSavedFiltered = null;
            this.ItemDeleted = null;
            this.ItemDeletedFiltered = null;

            base.DisposeManaged();
        }
        #endregion


    }
}
