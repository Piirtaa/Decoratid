using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using System.Runtime.Serialization;

namespace Decoratid.Extensions
{
    public static class SerializationExtensions
    {
        /// <summary>
        /// whether an object is marked as ISerializable or it has the Serializable attribute
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsMarkedSerializable(this object obj)
        {
            if (obj == null)
                return false;

            return ((obj is ISerializable) || (Attribute.IsDefined(obj.GetType(), typeof(SerializableAttribute))));
        }

        public static string JSONSerialize(this object x)
        {
            if (x == null)
                return null;
            return JsonSerializer.SerializeToString(x, x.GetType());
        }
    }
}
