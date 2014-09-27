using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;

namespace Decoratid.Serialization
{
    /// <summary>
    /// defines the serialization signature
    /// </summary>
    public interface ISerializationUtil : IHasId<string>
    {
        string Serialize(object obj);
        object Deserialize(Type type, string s);
    }
}
