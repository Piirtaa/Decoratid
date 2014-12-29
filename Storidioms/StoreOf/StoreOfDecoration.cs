using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Storidioms.ItemValidating;
using System.Collections.Generic;

namespace Decoratid.Storidioms.StoreOf
{
    #region  IStore Of Constructs
    /// <summary>
    /// a store restricted to items that are of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOf<T> : IStore, IValidatingStore where T : IHasId
    {
        new List<T> GetAll();
    }
    #endregion

    /// <summary>
    /// Turns a store into a "storeOf".  decorates a store such that the only items that can be stored within the store are of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StoreOfDecoration<T> : ValidatingDecoration, IStoreOf<T> where T : IHasId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StoreOfDecoration(IStore decorated)
            : base(decorated, IsOfValidator.New<T>())
        {
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreOfDecoration<T>(store);
        }
        #endregion

        #region Overrides
        public new List<T> GetAll()
        {
            return base.GetAll().ConvertListTo<T, IHasId>();
        }
        #endregion
    }

    public static class StoreOfDecorationExtensions
    {
        /// <summary>
        /// constrains the store to operate on items of only T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static StoreOfDecoration<T> IsOf<T>(this IStore decorated)
              where T : IHasId
        {
            Condition.Requires(decorated).IsNotNull();
            return new StoreOfDecoration<T>(decorated);
            //could alternately do
            //return decorated.WithValidation(new IsOfValidator<T>());
        }

    }
}
