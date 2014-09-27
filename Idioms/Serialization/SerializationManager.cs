using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations.StoreOf;
using Decoratid.TypeLocation;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using ServiceStack.Text;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Serialization
{
    /// <summary>
    /// Singleton container of ISerializationUtil, that helps with serialization
    /// </summary>
    public class SerializationManager
    {
        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static SerializationManager _instance = new SerializationManager(); //the singleton instance

        #endregion

        #region Ctor
        static SerializationManager()
        {
        }
        private SerializationManager()
        {
            this.Store = new NaturalInMemoryStore().DecorateWithIsOfUniqueId<ISerializationUtil>();
            //hydrate it by loading types
            var types = new TypeContainer<ISerializationUtil>();
            types.ContainedTypes.WithEach(x =>
            {
                try
                {
                    var instance = Activator.CreateInstance(x);
                    var ser = instance as ISerializationUtil;
                    this.Store.SaveItem(ser);
                }
                catch { }
            });
        }
        #endregion

        #region Properties
        public static SerializationManager Instance { get { return _instance; } }
        private IStoreOfUniqueId<ISerializationUtil> Store { get; set; }
        #endregion

        #region Calculated Properties
        public List<ISerializationUtil> All
        {
            get
            {
                return this.Store.GetAll<ISerializationUtil>();
            }
        }
        #endregion

        #region Get Serializer
        public ISerializationUtil GetSerializer(string serializerId)
        {
            var ser = this.Store.GetAllById<ISerializationUtil>(serializerId).FirstOrDefault();
            return ser;
        }
        #endregion

        #region Envelope-y stuff
        /// <summary>
        /// Serializes with the specified serializer
        /// </summary>
        /// <param name="serializerId"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private SerializationEnvelope SerializeToEnvelope(string serializerId, object obj)
        {
            if (obj == null) { return null; }

            var ser = this.Store.GetAllById<ISerializationUtil>(serializerId).FirstOrDefault();
            Condition.Requires(ser).IsNotNull();

            SerializationEnvelope env = SerializationEnvelope.New(ser, obj);
            return env;
        }
        /// <summary>
        /// Serializes with the first serializer that works
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private SerializationEnvelope SerializeToEnvelopeWithFirstApplicable(object obj)
        {
            if (obj == null) { return null; }

            var sers = this.Store.GetAll<ISerializationUtil>();

            foreach (var ser in sers)
            {
                try
                {
                    SerializationEnvelope env = SerializationEnvelope.New(ser, obj);
                    return env;
                }
                catch
                {
                    continue;
                }
            }

            throw new InvalidOperationException("No valid serializers found");
        }
        private object DeserializeEnvelopePayload(SerializationEnvelope env)
        {
            if (env == null) { return null; }

            var ser = this.Store.GetAllById<ISerializationUtil>(env.Id).FirstOrDefault();
            Condition.Requires(ser).IsNotNull();

            var rv = env.DeserializePayload(ser);
            return rv;
        }
        #endregion

        #region Your more standard type of sigs
        /// <summary>
        /// finds the first serializer that works and serializes the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string SerializeWithFirstApplicable(object obj)
        {
            var env = SerializeToEnvelopeWithFirstApplicable(obj);
            if (env == null)
                return null;

            var rv = ((IReconstable)env).Dehydrate();
            return rv;
        }
        public string Serialize(string serializerId, object obj)
        {
            var env = SerializeToEnvelope(serializerId, obj);
            if (env == null)
                return null;

            var rv = ((IReconstable)env).Dehydrate();
            return rv;
        }
        public object Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            SerializationEnvelope env = SerializationEnvelope.Parse(data);
            var rv = this.DeserializeEnvelopePayload(env);
            return rv;
        }
        #endregion

    }
}
