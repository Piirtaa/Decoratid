using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Thingness;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Core.Storing.Decorations.Masking
{
    /// <summary>
    /// Masks a store (ie. only enables the indicated functions)
    /// </summary>
    public interface IMaskingStore : IDecoratedStore
    {
        StoreOperation Mask { get; }
    }

    /// <summary>
    /// Masks a store (ie. only enables the indicated functions)
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class MaskingDecoration : DecoratedStoreBase, IMaskingStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public MaskingDecoration(IStore decorated, StoreOperation mask)
            : base(decorated)
        {
            this.Mask = mask;
        }
        #endregion

        #region Properties
        public StoreOperation Mask { get; private set; }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<MaskingDecoration>();
            hydrationMap.RegisterDefault("Mask", x => x.Mask, (x, y) => { x.Mask = (StoreOperation)y; });
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

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new MaskingDecoration(store, this.Mask);

            return returnValue;
        }
        #endregion

        #region Overrides
        public override IHasId Get(IStoredObjectId soId)
        {
            if (!this.Mask.Has(StoreOperation.Get))
                throw new InvalidOperationException("operation masked");

            return this.Decorated.Get(soId);
        }
        public override List<T> Search<T>(SearchFilter filter)
        {
            if (!this.Mask.Has(StoreOperation.Search))
                throw new InvalidOperationException("operation masked");

            return this.Decorated.Search<T>(filter);
        }
        public override void Commit(ICommitBag bag)
        {
            if (!this.Mask.Has(StoreOperation.Commit))
                throw new InvalidOperationException("operation masked");

            this.Decorated.Commit(bag);
        }
        public override List<IHasId> GetAll()
        {
            if (!this.Mask.Has(StoreOperation.GetAll))
                throw new InvalidOperationException("operation masked");

            return this.Decorated.GetAll();
        }
        #endregion


    }
}
