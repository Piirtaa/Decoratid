using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Storidioms.ItemValidating
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
    /// 
    [Serializable]
    public class ValidatingDecoration : DecoratedStoreBase, IValidatingStore //, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public ValidatingDecoration(IStore decorated, IItemValidator validator)
            : base(decorated)
        {
            Condition.Requires(validator).IsNotNull();
            this.ItemValidator = validator;
        }
        #endregion
        
        #region ISerializable
        private ValidatingDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ItemValidator = (IItemValidator)info.GetValue("ItemValidator", typeof(IItemValidator));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemValidator", this.ItemValidator);
            base.ISerializable_GetObjectData(info, context);
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

        //#region IHasHydrationMap
        //public virtual IHydrationMap GetHydrationMap()
        //{
        //    var hydrationMap = new HydrationMapValueManager<ValidatingDecoration>();
        //    hydrationMap.RegisterDefault("ItemValidator", x => x.ItemValidator, (x, y) => { x.ItemValidator = y as IItemValidator; }); 
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

        #region Overrides
        public override void Commit(ICommitBag bag)
        {
            //examine the stuff we're saving (not deleting - item would have been validated as a prerequisite to saving)
            bag.ItemsToSave.WithEach(x =>
            {
                if (!this.ItemValidator.IsValidCondition.Evaluate(x).GetValueOrDefault())
                    throw new InvalidOperationException("invalid item " + new StoredObjectId(x).ToString());
            });

            this.Decorated.Commit(bag);
        }
        #endregion


    }
}
