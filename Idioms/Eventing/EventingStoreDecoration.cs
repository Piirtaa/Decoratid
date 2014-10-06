using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Intercepting;
using Decoratid.Idioms.Logging;
using Decoratid.Storidioms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.Eventing
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
    public class EventingStoreDecoration : DecoratedStoreBase, IEventingStore, IInterceptingStore, IHasLogger
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// ctor.  requires IStore to wrap
        /// </summary>
        /// <param name="decorated"></param>
        public EventingStoreDecoration(IStore decorated, ILogger logger)
            : base(decorated.Intercepting(logger))
        {
            //wire to the intercepts
            this.InterceptCore.CommitOperationIntercept.Completed += CommitOperationIntercept_Completed;
            this.InterceptCore.SearchOperationIntercept.Completed += SearchOperationIntercept_Completed;
            this.InterceptCore.GetOperationIntercept.Completed += GetOperationIntercept_Completed;
            this.InterceptCore.GetAllOperationIntercept.Completed += GetAllOperationIntercept_Completed;
        }
        #endregion

        //#region IHasHydrationMap
        //public virtual IHydrationMap GetHydrationMap()
        //{
        //    var hydrationMap = new HydrationMapValueManager<EventingStoreDecoration>();
        //    return hydrationMap;
        //}
        //#endregion

        //#region IDecorationHydrateable
        //public override string DehydrateDecoration(IGraph uow = null)
        //{
        //    return this.GetHydrationMap().DehydrateValue(this, uow);
        //}
        //public override void HydrateDecoration(string text, IGraph uow = null)
        //{
        //    this.GetHydrationMap().HydrateValue(this, text, uow);
        //}
        //#endregion
        #region IHasLogger
        public ILogger Logger { get { return this.InterceptCore.Logger; } }
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

        #region Intercept Subscriptions
        void GetOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<IStoredObjectId, IHasId>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                //if the result is nulled, fire the filtered event
                if (uow.DecoratedResult.GetValue() == null && uow.Result != null)
                {
                    this.ItemRetrievedFiltered.BuildAndFireEventArgs(uow.Result);
                }
                else
                {

                    this.ItemRetrieved.BuildAndFireEventArgs(uow.DecoratedResult.GetValue());
                }
            }
        }

        void SearchOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<Tuple<Type, SearchFilter>, List<IHasId>>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                //examine the before and after lists to find deletions
                var origList = uow.Result;
                var finalList = uow.DecoratedResult.GetValue();

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
        void GetAllOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<Nothing, List<IHasId>>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                //examine the before and after lists to find deletions
                var origList = uow.Result;
                var finalList = uow.DecoratedResult.GetValue();

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
        void CommitOperationIntercept_Completed(object sender, EventArgOf<InterceptUnitOfWork<ICommitBag, Nothing>> e)
        {
            var uow = e.Value;

            if (uow.Error == null)
            {
                var origItemsToSave = uow.Arg.ItemsToSave.ToList();
                var origItemsToDelete = uow.Arg.ItemsToDelete.ToList();
                var finalItemsToSave = uow.DecoratedArg.GetValue().ItemsToSave.ToList();
                var finalItemsToDelete = uow.DecoratedArg.GetValue().ItemsToDelete.ToList();
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
        public InterceptChain<IStoredObjectId, IHasId> GetOperationIntercept
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

        public InterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept
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

        public InterceptChain<ICommitBag, Nothing> CommitOperationIntercept
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
        public InterceptChain<Nothing, List<IHasId>> GetAllOperationIntercept
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
            var returnValue = new EventingStoreDecoration(store, this.Logger);

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

    public static class EventingStoreDecorationExtensions
    {
        /// <summary>
        /// gets the factory layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static EventingStoreDecoration GetEventingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<EventingStoreDecoration>(true);
        }

        /// <summary>
        /// adds events
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static EventingStoreDecoration DecorateWithEvents(this IStore decorated, ILogger logger)
        {
            Condition.Requires(decorated).IsNotNull();
            return new EventingStoreDecoration(decorated, logger);
        }
    }
}
