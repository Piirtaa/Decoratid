using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Extensions;
using Decoratid.Idioms.Storing.Decorations.StoreOf;
using Decoratid.Thingness;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Idioms.Storing.Decorations.Validating
{
    /// <summary>
    /// a writeable store that validates each item prior to commit
    /// </summary>
    public interface IValidatingStore : IWriteableStore 
    {
        IItemValidator ItemValidator { get; }
    }

    /// <summary>
    /// adds validation logic to item commits
    /// </summary>
    public class ValidatingDecoration : DecoratedStoreBase, IValidatingStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected ValidatingDecoration() : base() {}
        public ValidatingDecoration(IStore decorated, IItemValidator validator)
            : base(decorated)
        {
            Condition.Requires(validator).IsNotNull();
            this.ItemValidator = validator;
        }
        #endregion

        #region Properties
        public IItemValidator ItemValidator { get; protected set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new ValidatingDecoration(store, this.ItemValidator);
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<ValidatingDecoration>();
            hydrationMap.RegisterDefault("ItemValidator", x => x.ItemValidator, (x, y) => { x.ItemValidator = y as IItemValidator; }); 
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

        #region Overrides
        public override void Commit(ICommitBag bag)
        {
            //examine the stuff we're saving (not deleting - item would have been validated as a prerequisite to saving)
            bag.ItemsToSave.WithEach(x =>
            {
                if(!this.ItemValidator.IsValidCondition.Evaluate(x).GetValueOrDefault())
                    throw new InvalidOperationException("invalid item " + new StoredObjectId(x).ToString());
            });

            this.Decorated.Commit(bag);
        }
        #endregion


    }
}
