//using System;
//using System.Runtime.Serialization;
//using System.Reflection;
//using Storidiom.Thingness.Idioms.Store.Decorations;
//using Storidiom.Thingness.Idioms.Store;
//using System.Collections.Generic;

//namespace Storidiom.Thingness.Idioms.Store
//{

//    /// <summary>
//    /// serializes things that have stores in them
//    /// </summary>
//    public class StoreSerializationSurrogate : ISerializationSurrogate
//    {
//        private const BindingFlags publicOrNonPublicInstanceFields = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

//        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
//        {
//            GetObjectDataHelper(obj, info, context, null);
//        }

//        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
//        {
//            return SetObjectDataHelper(obj, info, context, selector, null);
//        }

//        #region Helpers
//        private static string GetMemberKey(MemberInfo mi)
//        {
//            return mi.DeclaringType.Name + "+" + mi.Name;
//        }
//        /// <summary>
//        /// finds any fields that are stores and serializes them into a dictionary
//        /// </summary>
//        /// <returns></returns>
//        public static Dictionary<string, string> SerializeStoreMembers(object obj)
//        {
//            Dictionary<string, string> rv = new Dictionary<string, string>();

//            Type thisType = obj.GetType();
//            var fields = thisType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//            foreach (FieldInfo field in fields)
//            {
//                if (!typeof(IStore).IsAssignableFrom(field.FieldType))
//                    continue;

//                IStore store = field.GetValue(obj) as IStore;
//                if (store == null)
//                    continue;

//                var data = StoreSerializer.SmartStoreSerialize(store);
//                string id = GetMemberKey(field);
//                rv[id] = data;
//            }

//            return rv;
//        }
//        /// <summary>
//        /// takes the dictionary we serialized to and rehydrates the decoration's stores
//        /// </summary>
//        /// <param name="dict"></param>
//        public static void DeserializeStoreMembers(object obj, Dictionary<string, string> dict)
//        {
//            Type thisType = obj.GetType();
//            var fields = thisType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//            foreach (FieldInfo field in fields)
//            {
//                if (!typeof(IStore).IsAssignableFrom(field.FieldType))
//                    continue;

//                string id = GetMemberKey(field);
//                if (dict.ContainsKey(id))
//                {
//                    var data = dict[id];
//                    var store = StoreSerializer.SmartStoreDeserialize(data);
//                    field.SetValue(obj, store);
//                }
//            }
//        }
//        #endregion

//        #region Static Binary Serialization Helpers
//        /// <summary>
//        /// serialization helper that will serialize all the store members using SerializeStoreMembers() and then
//        /// serialize all the other members, excluding the ones filtered out
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        /// <param name="skipFilter"></param>
//        public static void GetObjectDataHelper(object obj, SerializationInfo info, StreamingContext context, Func<FieldInfo, bool> skipFilter)
//        {
//            //do stores first
//            var dict = SerializeStoreMembers(obj);
//            info.AddValue("_decoratedStores", dict, typeof(Dictionary<string, string>));

//            //do everything else

//            // Get the set of serializable members for our class and base classes
//            Type thisType = obj.GetType();
//            MemberInfo[] mi = FormatterServices.GetSerializableMembers(thisType, context);

//            // Serialize the base class's fields to the info object
//            for (Int32 i = 0; i < mi.Length; i++)
//            {
//                //// Don't serialize fields for this class
//                //if (mi[i].DeclaringType == thisType) continue;

//                // To ease coding, treat the member as a FieldInfo object
//                FieldInfo fi = (FieldInfo)mi[i];

//                //if we've already handled this with the store stuff, skip it
//                string id = GetMemberKey(fi);

//                if (dict.ContainsKey(id))
//                    continue;

//                //if the filter catches it, skip it
//                if (skipFilter != null && skipFilter(fi))
//                    continue;

//                info.AddValue(mi[i].Name, fi.GetValue(obj));
//            }
//        }

//        /// <summary>
//        /// deserialization helper that will deserialize all the store members using DeserializeStoreMembers() and then
//        /// serialize all the other members, excluding the ones filtered out
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        /// <param name="selector"></param>
//        /// <param name="skipFilter"></param>
//        /// <returns></returns>
//        public static object SetObjectDataHelper(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector, Func<FieldInfo, bool> skipFilter)
//        {
//            //do stores first
//            Dictionary<string, string> dict = (Dictionary<string, string>)info.GetValue("_decoratedStores", typeof(Dictionary<string, string>));
//            DeserializeStoreMembers(obj, dict);

//            //do everything else

//            // Get the set of serializable members for our class and base classes
//            Type thisType = obj.GetType();
//            MemberInfo[] mi = FormatterServices.GetSerializableMembers(thisType, context);

//            // Deserialize the base class's fields from the info object
//            for (Int32 i = 0; i < mi.Length; i++)
//            {
//                //// Don't deserialize fields for this class
//                //if (mi[i].DeclaringType == thisType) continue;

//                // To ease coding, treat the member as a FieldInfo object
//                FieldInfo fi = (FieldInfo)mi[i];

//                //if we've already handled this with the store stuff, skip it
//                string id = GetMemberKey(fi);

//                if (dict.ContainsKey(id))
//                    continue;

//                //if the filter catches it, skip it
//                if (skipFilter != null && skipFilter(fi))
//                    continue;

//                // Set the field to the deserialized value
//                fi.SetValue(obj, info.GetValue(fi.Name, fi.FieldType));
//            }

//            return obj;
//        }

//        #endregion
//    }
//}

