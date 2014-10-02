using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Core.Storing
{
    public static class CommitExtensions
    {

        public static void SaveItemIfUniqueElseThrow(this IStore store, IHasId obj)
        {
            if (store == null)
                return;

            //throw if the item exists
            var dup = store.Get(obj.GetStoredObjectId());
            if (dup != null)
                throw new InvalidOperationException("Item already exists " + obj.Id.ToString());

            store.Commit(new CommitBag().MarkItemSaved(obj));
        }
        public static void SaveItemsIfUniqueElseThrow(this IStore store, List<IHasId> objs)
        {
            if (store == null)
                return;

            objs.WithEach(obj =>
            {
                //throw if the item exists
                var dup = store.Get(obj.GetStoredObjectId());
                if (dup != null)
                    throw new InvalidOperationException("Item already exists " + obj.Id.ToString());
            });

            store.Commit(new CommitBag().MarkItemsSaved(objs));
        }

        public static void SaveItem(this IWriteableStore store, IHasId obj)
        {
            if (store == null)
                return;

            store.Commit(CommitBag.New().MarkItemSaved(obj));
        }
        public static void SaveItems(this IWriteableStore store, List<IHasId> objs)
        {
            if (store == null)
                return;

            store.Commit(new CommitBag().MarkItemsSaved(objs));
        }
        public static void DeleteItem(this IWriteableStore store, StoredObjectId soid)
        {
            if (store == null)
                return;

            store.Commit(new CommitBag().MarkItemDeleted(soid));
        }
        public static void DeleteItems(this IWriteableStore store, List<StoredObjectId> objs)
        {
            if (store == null)
                return;
            
            var commitBag = CommitBag.New();
            commitBag.MarkItemsDeleted(objs);
            store.Commit(commitBag);
        }
    }
}
