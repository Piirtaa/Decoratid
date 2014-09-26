using CuttingEdge.Conditions;
using Storidiom.Reflection;
using Storidiom.Thingness.Idioms.Store.CoreStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Storidiom.Thingness.Idioms.Store
{
    /// <summary>
    /// converts objects to a store, and vice versa.  Basically, it maps references. 
    /// </summary>
    public static class Object2StoreConverter
    {
        private const string TYPEKEY = "__TYPE";

        #region object conversions

        /// <summary>
        /// serializes an object's state to a store
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static InMemoryStore ConvertToStore(object obj, Func<FieldInfo, bool> filter)
        {
            Condition.Requires(obj).IsNotNull();

            InMemoryStore store = new InMemoryStore();

            Type objType = obj.GetType();

            //register the object type
            var objectTypeEntry = ContextualAsId<string, Type>.New(TYPEKEY, objType);
            store.SaveItem(objectTypeEntry);

            var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(objType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //for each field, build data to save to the store
            foreach (FieldInfo field in fields)
            {
                if (filter != null && filter(field))
                    continue;

                //key the field 
                var id = field.DeclaringType.Name + "_" + field.Name;

                //get the field value
                var val = field.GetValue(obj);
                
                //build the entries (isNull, and actual value)
                var isNullEntry = ContextualAsId<string,bool>.New(id, val == null);
                store.SaveItem(isNullEntry);

                if(val !=null)
                {
                    var valueEntry = ContextualAsId<string,object>.New(id, val); //we specify the context type as object to simplify lookup
                    store.SaveItem(valueEntry);
                }
            }
            return store;
        }
        
        /// <summary>
        /// hydrates an object
        /// </summary>
        /// <param name="store"></param>
        /// <param name="obj"></param>
        /// <param name="filter"></param>
        public static void HydrateFromStore(IStore store, object obj, Func<FieldInfo, bool> filter)
        {
            Condition.Requires(obj).IsNotNull();
            Condition.Requires(store).IsNotNull();

            //validate we're dealing with the right object
            var objectTypeEntry = store.Get<ContextualAsId<string, Type>>(TYPEKEY);
            Condition.Requires(objectTypeEntry).IsNotNull();

            Type objType = obj.GetType();
            Condition.Requires(objType.Equals(objectTypeEntry.Context));

            var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(objType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //for each field, build data to save to the store
            foreach (FieldInfo field in fields)
            {
                if (filter != null && filter(field))
                    continue;

                //key the field 
                var id = field.DeclaringType.Name + "_" + field.Name;

                //is it null?
                var isNullEntry = store.Get<ContextualAsId<string, bool>>(id);
                Condition.Requires(isNullEntry).IsNotNull();
                if (!isNullEntry.Context)
                {
                    //var typeEntry = store.Get<ContextualAsId<string, Type>>(id);
                    var val = store.Get<ContextualAsId<string, object>>(id);

                    //now set it
                    field.SetValue(obj, val.Context);
                }
            }
        }
        public static object ConvertFromStore(IStore store, Func<FieldInfo, bool> filter)
        {
            Condition.Requires(store).IsNotNull();

            //validate we're dealing with the right object
            var objectTypeEntry = store.Get<ContextualAsId<string, Type>>(TYPEKEY);
            Condition.Requires(objectTypeEntry).IsNotNull();

            var obj = New.Create(objectTypeEntry.Context);
            HydrateFromStore(store, obj, filter);

            return obj;
        }
        #endregion
    }
}
