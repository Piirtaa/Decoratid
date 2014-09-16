//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Text;
//using System.Threading.Tasks;
//using Decoratid.Idioms.Storing;
//using ServiceStack.Text;

//namespace Decoratid.Serialization
//{
//    public class JSONSerializationUtil  : ISerializationUtil
//    {
//        public JSONSerializationUtil() 
//        {
//            //JsConfig.IncludePublicFields = true;
//        }

//        #region IHasId
//        public string Id { get { return this.GetType().Name; } }
//        object IHasId.Id { get { return this.Id; } }
//        #endregion

//        public string Serialize(object obj)
//        {
//            string returnValue = string.Empty;
//            if (obj == null) { return returnValue; }

//            try
//            {
//                returnValue = JsonSerializer.SerializeToString(obj, obj.GetType());
//            }
//            catch
//            {
//                throw;
//            }
//            return returnValue;
//        }
//        public object Deserialize(Type type, string s)
//        {
//            object returnValue = null;
//            if (string.IsNullOrEmpty(s)) { return returnValue; }

//            try
//            {
//                returnValue = JsonSerializer.DeserializeFromString(s, type);
//            }
//            catch
//            {
//                throw;
//            }
//            return returnValue;
//        }
//    }
//}
