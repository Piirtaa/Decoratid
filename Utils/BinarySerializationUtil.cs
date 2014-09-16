using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace Decoratid.Utils
{
    public static class BinarySerializationUtil  
    {
        public static string Serialize(object obj)
        {
            string returnValue = string.Empty;
            if (obj == null) { return returnValue; }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.SurrogateSelector = new UnattributedTypeSurrogateSelector();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    byte[] b = ms.ToArray();
                    returnValue =  Convert.ToBase64String(b);//Encoding.ASCII.GetString(b);//
                }
            }
            catch
            {
                throw;
            }
            return returnValue;
        }
        public static object Deserialize(Type type, string s)
        {
            object returnValue = null;
            if (string.IsNullOrEmpty(s)) { return returnValue; }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.SurrogateSelector = new UnattributedTypeSurrogateSelector();
                byte[] b = Convert.FromBase64String(s);//Encoding.ASCII.GetBytes(s);// 

                using (MemoryStream ms = new MemoryStream(b))
                {
                    ms.Position = 0;
                    returnValue = bf.Deserialize(ms);
                }
            }
            catch
            {
                throw;
            }
            return returnValue;
        }


    }
}
