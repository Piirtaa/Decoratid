﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using Decoratid.Thingness.Idioms.Store.Decorations.Validating;
using Decoratid.Extensions;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Thingness.Idioms.Store namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// replaces an item with the same id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="item"></param>
        public static void Replace<T>(this IStoreOfUniqueId<T> store, T newItem) where T : IHasId
        {
            Condition.Requires(store).IsNotNull();

            //remove the old item
            SearchFilterOf<T> filter = new SearchFilterOf<T>((item) =>
            {
                return item.Id.Equals(newItem.Id) && (item.GetType().Equals(newItem.GetType()) == false);
            });

            var list = store.Search<T>(filter);

            list.WithEach(oldItem =>
            {
                store.DeleteItem(oldItem.GetStoredObjectId());
            });

            //commit the new item
            store.SaveItem(newItem);
        }

        /// <summary>
        /// constrains the store to operate on items of only T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static StoreOfDecoration<T> DecorateWithIsOf<T>(this IStore decorated)
              where T : IHasId
        {
            Condition.Requires(decorated).IsNotNull();
            return new StoreOfDecoration<T>(decorated);
            //could alternately do
            //return decorated.WithValidation(new IsOfValidator<T>());
        }
        /// <summary>
        /// Is a "store of" and also requires that ids are unique
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static StoreOfUniqueIdDecoration<T> DecorateWithIsOfUniqueId<T>(this IStore decorated)
      where T : IHasId
        {
            Condition.Requires(decorated).IsNotNull();
            return new StoreOfUniqueIdDecoration<T>(decorated);
            //could alternately do
            //return decorated.WithValidation(new IsOfValidator<T>());
        }

        /// <summary>
        /// constrains the store to operate on items of exactly T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static StoreOfExactlyDecoration<T> DecorateWithExactlyIsOf<T>(this IStore decorated)
      where T : IHasId
        {
            Condition.Requires(decorated).IsNotNull();
            return new StoreOfExactlyDecoration<T>(decorated);
        }
    }



}
