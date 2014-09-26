using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing.Decorations;
using Decoratid.Idioms.Storing.Decorations.Eventing;
using Decoratid.Idioms.Storing.Core;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Idioms.Storing.Products
{
    /// <summary>
    /// simple in-memory store with events
    /// </summary>
    public class EventingInMemoryStore : DecoratedStoreBase, IEventingStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public EventingInMemoryStore()
            : base(NaturalInMemoryStore.New().DecorateWithEvents())
        {
        }
        #endregion

        #region IDecoratedStore
        /// <summary>
        /// throw not implemented exception on "decorated in-memory stores"
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IEventingStore
        public event EventHandler<EventArgOf<IHasId>> ItemRetrieved
        {
            add
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemRetrieved += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemRetrieved -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemRetrievedFiltered
        {
            add
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemRetrievedFiltered += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemRetrievedFiltered -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemSaved
        {
            add
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemSaved += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemSaved -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemSavedFiltered
        {
            add
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemSavedFiltered += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemSavedFiltered -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemDeleted
        {
            add
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemDeleted += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemDeleted -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemDeletedFiltered
        {
            add
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemDeletedFiltered += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoratorOf<EventingDecoration>(true).ItemDeletedFiltered -= value;
            }
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return string.Empty;
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {

        }
        #endregion
    }


}
