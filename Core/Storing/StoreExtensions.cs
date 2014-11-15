using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Core.Storing
{
    public static class StoreExtensions
    {
        public static bool Contains(this IStore store, StoredObjectId soId)
        {
            var item = store.Get(soId);
            return item != null;
        }
        /// <summary>
        /// copies an item from one store to the other
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeToMoveTo"></param>
        /// <param name="itemToMove"></param>
        public static void CopyItem(this IStore store, IStore storeToMoveTo, StoredObjectId itemToMove)
        {
            if (store == null)
                return;

            if (storeToMoveTo == null)
                return;


            var item = store.Get(itemToMove);
            if (item != null)
                storeToMoveTo.SaveItem(item);
        }

        /// <summary>
        /// moves an item from one store to the other
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeToMoveTo"></param>
        /// <param name="itemToMove"></param>
        public static void MoveItem(this IStore store, IStore storeToMoveTo, StoredObjectId itemToMove)
        {
            if (store == null)
                return;

            if (storeToMoveTo == null)
                return;

            var item = store.Get(itemToMove);
            if (item != null)
            {
                storeToMoveTo.SaveItem(item);
                store.DeleteItem(item.GetStoredObjectId());
            }
        }

        /// <summary>
        /// copies items between stores
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeToMoveTo"></param>
        /// <param name="itemsToMove"></param>
        public static void CopyItems(this IStore store, IStore storeToMoveTo, List<StoredObjectId> itemsToMove)
        {
            itemsToMove.WithEach(x =>
            {
                CopyItem(store, storeToMoveTo, x);
            });
        }
        /// <summary>
        /// movesitems between stores
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeToMoveTo"></param>
        /// <param name="itemsToMove"></param>
        public static void MoveItems(this IStore store, IStore storeToMoveTo, List<StoredObjectId> itemsToMove)
        {
            itemsToMove.WithEach(x =>
            {
                MoveItem(store, storeToMoveTo, x);
            });
        }
        /// <summary>
        /// joins a store into another  store
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeToJoin"></param>
        public static void JoinStore(this IStore store, IStore storeToJoin)
        {
            if (store == null)
                return;

            var all = storeToJoin.GetAll();
            store.SaveItems(all);
        }

        /// <summary>
        /// splits items out from a store and creates a new store for them 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="itemsToRemove"></param>
        public static NaturalInMemoryStore SplitFromStore(this IStore store, List<StoredObjectId> itemsToRemove)
        {
            if (store == null)
                return null;

            if (itemsToRemove == null || itemsToRemove.Count == 0)
                return null;

            NaturalInMemoryStore returnValue = new NaturalInMemoryStore();

            itemsToRemove.WithEach(x =>
            {
                var item = store.Get(x);
                if (item != null)
                {
                    //move the item from one store to the other
                    returnValue.SaveItem(item);
                    store.DeleteItem(item.GetStoredObjectId());
                }
            });

            return returnValue;
        }

        /// <summary>
        /// Modifies the id of an object in a store
        /// </summary>
        /// <param name="store"></param>
        /// <param name="idToChange"></param>
        /// <param name="newId"></param>
        public static void ChangeStoredObjectId(this IStore store, StoredObjectId idToChange, object newId)
        {
            if (store == null)
                return;

            if (idToChange == null)
                return;

            //if we're not changing anything, skip
            if (idToChange.ObjectId.Equals(newId))
                return;

            //get the object to ensure it exists
            var obj = store.Get(idToChange);
            if (obj == null)
                return;
            
            //mark delete
            var cb = CommitBag.New();
            cb.MarkItemDeleted(idToChange);

            //change the id
            if (obj is IHasSettableId)
            {
                IHasSettableId ihs = obj as IHasSettableId;
                ihs.SetId(newId);
            }
            else
            {
                PropertyInfo pi = obj.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                pi.SetValue(obj, newId);
            }
            //mark save new item and commit
            cb.MarkItemSaved(obj);
            store.Commit(cb);
        }

    }
}
