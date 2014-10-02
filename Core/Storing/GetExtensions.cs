using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Core.Storing
{
    public static class GetExtensions
    {
        public static StoredObjectId GetStoredObjectId(this IHasId item)
        {
            if (item == null)
                return null;

            return new StoredObjectId(item);
        }

        public static bool Exists(this IStore store, StoredObjectId id)
        {
            var item = store.Get(id);
            return item != null;
        }

        public static IHasId Get(this IGettableStore store, Type type, object id)
        {
            if (store == null)
                return null;

            StoredObjectId soId = new StoredObjectId(type, id);

            return store.Get(soId);
        }

        public static T Get<T>(this IGettableStore store, object id) where T : IHasId
        {
            if (store == null)
                return default(T);

            StoredObjectId soId = StoredObjectId.New(typeof(T), id);
            var item = store.Get(soId);

            if (item == null)
                return default(T);

            return (T)item;
        }

    }
}
