using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.ObjectGraph;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;
using Decoratid.Thingness.Idioms.Hydrateable;

namespace Decoratid.Serialization
{
    /// <summary>
    /// A container class that contains the information on how to serialize/deserialize the thing it is containing
    /// </summary>
    /// <remarks>
    /// Has the same id as the serializer.  has hydrateable implementation.  
    /// </remarks>
    public class SerializationEnvelope : IHasId<string>, IHydrateable
    {
        #region Ctor
        protected SerializationEnvelope() { }
        #endregion

        #region IHasId
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public Type InstanceType { get; private set; }
        public string SerializedData { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// given the appropriate serializer, the envelope payload is deserialized
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public object DeserializePayload(ISerializationUtil ser)
        {
            Condition.Requires(ser).IsNotNull();
            Condition.Requires(ser.Id).IsEqualTo(this.Id);

            object obj = ser.Deserialize(this.InstanceType, this.SerializedData);
            return obj;
        }
        #endregion

        #region IHydrateable
        string IHydrateable.Dehydrate()
        {
            //split into: serializerid,instancetype name,serialized data
            var rv = TextDecorator.LengthEncodeList(this.Id, this.InstanceType.AssemblyQualifiedName, this.SerializedData);
            return rv;
        }
        void IHydrateable.Hydrate(string text)
        {
            var list = TextDecorator.LengthDecodeList(text);
            Condition.Requires(list).HasLength(3);

            this.Id = list[0];
            this.InstanceType = TypeFinder.FindAssemblyQualifiedType(list[1]);
            this.SerializedData = list[2];
        }
        #endregion

        #region Fluent Static
        /// <summary>
        /// builds a new envelope from a raw object
        /// </summary>
        /// <param name="ser"></param>
        /// <param name="obj"></param>
        public static SerializationEnvelope New(ISerializationUtil ser, object obj)
        {
            Condition.Requires(ser).IsNotNull();
            Condition.Requires(obj).IsNotNull();

            SerializationEnvelope rv = new SerializationEnvelope();
            rv.Id = ser.Id;
            rv.InstanceType = obj.GetType();
            rv.SerializedData = ser.Serialize(obj);
            return rv;
        }
        /// <summary>
        /// Parses/hydrates the envelope from its dehydrated state
        /// </summary>
        /// <param name="text"></param>
        public static SerializationEnvelope Parse(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            
            SerializationEnvelope rv = new SerializationEnvelope();
            ((IHydrateable)rv).Hydrate(text);
            return rv;
        }
        #endregion
    }
}
