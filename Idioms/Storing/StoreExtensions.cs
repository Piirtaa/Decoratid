using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.Storing.Core;
using Decoratid.Idioms.Dependencies;

namespace Decoratid.Idioms.Storing
{
    public static class StoreExtensions
    {
        #region HasA Searches
        /// <summary>
        /// performs a regular Search amongst types T that also have a IHasDependencyOf THasA,and then sorts 
        /// by the dependency from least dependent to most
        /// </summary>
        /// <typeparam name="Tobj"></typeparam>
        /// <typeparam name="THasA"></typeparam>
        /// <param name="store"></param>
        public static List<T> SearchHasADependency<T, THasA>(this ISearchableStore store, SearchFilter filter)
    where T : IHasId, IHasDependencyOf<THasA>
        {
            if (store == null)
                return null;

            var list = store.Search<T>(filter);

            List<IHasDependencyOf<THasA>> depList = new List<IHasDependencyOf<THasA>>();
            list.WithEach(x =>
            {
                depList.Add(x);
            });

            //sort deplist
            depList = DependencyUtil.SortHasADependencies(depList);


            //convert to T
            List<T> returnValue = new List<T>();

            depList.WithEach(x =>
            {
                returnValue.Add((T)x);
            });

            return returnValue;
        }
        /// <summary>
        /// performs a regular GetAll of T, where T is IHasDependencyOf THasA, sorted by the Dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THasA"></typeparam>
        /// <param name="store"></param>
        public static List<T> GetAllHasADependency<T, THasA>(this ISearchableStore store)
            where T : IHasId, IHasDependencyOf<THasA>
        {
            if (store == null)
                return null;

            var list = store.GetAll<T>();

            List<IHasDependencyOf<THasA>> depList = new List<IHasDependencyOf<THasA>>();
            list.WithEach(x =>
            {
                depList.Add(x);
            });

            //sort deplist
            depList = DependencyUtil.SortHasADependencies(depList);


            //convert to T
            List<T> returnValue = new List<T>();

            depList.WithEach(x =>
            {
                returnValue.Add((T)x);
            });

            return returnValue;
        }
        #endregion

        #region Search
        /// <summary>
        /// performs a regular Search amongst types T that also have a IHasDependencyOf THasA,and then sorts 
        /// by the dependency from least dependent to most
        /// </summary>
        /// <typeparam name="Tobj"></typeparam>
        /// <typeparam name="THasA"></typeparam>
        /// <param name="store"></param>
        public static List<IHasId> Search_NonGeneric(this ISearchableStore store, Type type, SearchFilter filter)
        {
            if (store == null)
                return null;

            List<IHasId> returnValue = new List<IHasId>();

            //get the type of the 
            var mi = store.GetType().GetMethod("Search");
            MethodInfo generic = mi.MakeGenericMethod(type);
            var retval = generic.Invoke(store, new object[] { filter });

            IEnumerable list = retval as IEnumerable;

            foreach (var each in list)
            {
                returnValue.Add(each as IHasId);
            }

            return returnValue;
        }
        #endregion

        #region Single Calls with Uniqueness Filter
        public static bool Exists(this IStore store, StoredObjectId id)
        {
            var item = store.Get(id);
            return item != null;
        }
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
        #endregion

        #region Commit Helpers
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
        #endregion

        #region Get All
        /// <summary>
        /// does a search that finds items that are of compatible types and have same id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<T> GetAllById<T>(this ISearchableStore store, object id) where T : IHasId
        {
            SearchFilterOf<T> filter = new SearchFilterOf<T>((item) =>
            {
                return item.Id.Equals(id);
            });

            var list = store.Search<T>(filter);

            return list;
        }

        public static List<T> GetAll<T>(this ISearchableStore store) where T : IHasId
        {
            if (store == null)
                return null;

            Func<IHasId, bool> strat = (x) =>
            {
                return true;
            };
            return store.Search<T>(strat);
        }
        #endregion

        #region Get By Id and Variants

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

        #endregion

        #region Delta Utilities
        public static Dictionary<Type, List<IHasId>> SplitItemsByType(this List<IHasId> list)
        {
            Dictionary<Type, List<IHasId>> returnValue = new Dictionary<Type, List<IHasId>>();

            list.WithEach(x =>
            {
                Type type = x.GetType();
                if (!returnValue.ContainsKey(type))
                {
                    returnValue.Add(type, new List<IHasId>());
                }

                returnValue[type].Add(x);
            });

            return returnValue;

        }

        public static List<IHasId> FindDeletedItems(this List<IHasId> list, List<IHasId> newList)
        {
            //find all items in the original list that aren't in the new list
            var items = list.FindAll(x => newList.Exists(y => y.Id == x.Id) == false);

            return items;
        }
        public static List<IHasId> FindAddedItems(this List<IHasId> list, List<IHasId> newList)
        {
            //find all items in the new list that aren't in the old list
            var items = newList.FindAll(x => list.Exists(y => y.Id == x.Id) == false);

            return items;
        }
        public static List<StoredObjectId> FindDeletedItems(this List<StoredObjectId> list, List<StoredObjectId> newList)
        {
            //find all items in the original list that aren't in the new list
            var items = list.FindAll(x => newList.Exists(y => y.Id == x.Id) == false);

            return items;
        }
        public static List<StoredObjectId> FindAddedItems(this List<StoredObjectId> list, List<StoredObjectId> newList)
        {
            //find all items in the new list that aren't in the old list
            var items = newList.FindAll(x => list.Exists(y => y.Id == x.Id) == false);

            return items;
        }
        #endregion

        #region GetStoredObjectId
        public static StoredObjectId GetStoredObjectId(this IHasId item)
        {
            if (item == null)
                return null;

            return new StoredObjectId(item);
        }
        #endregion

        #region Join, Split, Copy

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
        #endregion

        #region AsId, ContextualAsId
        public static AsId<T> BuildAsId<T>(this T thing)
        {
            if (thing == null)
                throw new ArgumentNullException();

            return AsId<T>.New(thing);
        }
        public static ContextualAsId<T, TContext> BuildContextualAsId<T, TContext>(this T thing, TContext context)
        {
            if (thing == null)
                throw new ArgumentNullException();

            return ContextualAsId<T, TContext>.New(thing, context);
        }
        #endregion

        #region Id Shifts
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

        
        #endregion
    }
}
