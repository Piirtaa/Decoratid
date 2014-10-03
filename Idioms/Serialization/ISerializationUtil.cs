using Decoratid.Core.Identifying;
using System;

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
