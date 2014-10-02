using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Core.Storing
{
    public static class DeltaExtensions
    {
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
    }
}
