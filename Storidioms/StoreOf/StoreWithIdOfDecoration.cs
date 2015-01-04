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
    #region  IStoreWithIdOf Of Constructs
    /// <summary>
    /// a store restricted to items that have id's of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreWithIdOf<T> : IStore, IValidatingStore 
    {
    }
    #endregion

    /// <summary>
    /// Turns a store into a "storeOf".  decorates a store such that the only items that can be stored within the store are of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StoreWithIdOfDecoration<T> : ValidatingDecoration, IStoreWithIdOf<T> 
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StoreWithIdOfDecoration(IStore decorated)
            : base(decorated, IdIsOfValidator.New<T>())
        {
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreWithIdOfDecoration<T>(store);
        }
        #endregion
    }

    public static class StoreWithIdOfDecorationExtensions
    {
        public static StoreWithIdOfDecoration<T> WithIdsOf<T>(this IStore decorated)
              where T : IHasId
        {
            Condition.Requires(decorated).IsNotNull();
            return new StoreWithIdOfDecoration<T>(decorated);
        }

    }
}
