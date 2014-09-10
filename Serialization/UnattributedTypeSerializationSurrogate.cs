using System;
using System.Runtime.Serialization;
using System.Reflection;
using Decoratid.Reflection;

namespace Decoratid.Serialization
{
    //see: http://jamesryangray.blogspot.ca/2010_09_01_archive.html

    /// <summary>
    /// if a type is not marked as serializable (eg. anonymous types), we need to provide a surrogate selector (eg. surrogate 
    /// factory) to find ISerializationSurrogate's to handle these types.
    /// </summary>
    public class UnattributedTypeSurrogateSelector : ISurrogateSelector
    {
        private readonly SurrogateSelector innerSelector = new SurrogateSelector();
        private readonly Type iFormatter = typeof(IFormatter);

        public void ChainSelector(ISurrogateSelector selector)
        {
            innerSelector.ChainSelector(selector);
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (!type.IsSerializable)
            {
                selector = this;
                return new UnattributedTypeSerializationSurrogate();
            }
            return innerSelector.GetSurrogate(type, context, out selector);
        }

        public ISurrogateSelector GetNextSelector()
        {
            return innerSelector.GetNextSelector();
        }
    }
    /// <summary>
    /// serializes types not marked serializable
    /// </summary>
    public class UnattributedTypeSerializationSurrogate : ISerializationSurrogate
    {
        private const BindingFlags publicOrNonPublicInstanceFields = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private bool ContainsEntry(SerializationInfo info, string name)
        {
            bool returnValue = false;
            foreach(SerializationEntry entry in info) 
            {
                if (entry.Name == name)
                {
                    returnValue = true;
                    break;
                }
            }
            return returnValue;
        }
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var type = obj.GetType();
            
            var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(type, publicOrNonPublicInstanceFields);
            //var fields =  type.GetFields(publicOrNonPublicInstanceFields);
            
            foreach (var field in fields)
            {
                //if the field has already been serialized, we skip
                if(this.ContainsEntry(info, field.Name))
                    continue;

                var fieldValue = field.GetValue(obj);
                var fieldValueIsNotNull = fieldValue != null;
                
                if (fieldValueIsNotNull)
                {
                    var fieldValueRuntimeType = fieldValue.GetType();
                    info.AddValue(field.Name + "RuntimeType", fieldValueRuntimeType.AssemblyQualifiedName);
                }

                info.AddValue(field.Name + "ValueIsNotNull", fieldValueIsNotNull);
                info.AddValue(field.Name, fieldValue);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var type = obj.GetType();
            var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(type, publicOrNonPublicInstanceFields);
            //var fields =  type.GetFields(publicOrNonPublicInstanceFields);

            foreach (var field in fields)
            {
                var fieldValueIsNotNull = info.GetBoolean(field.Name + "ValueIsNotNull");
                if (fieldValueIsNotNull)
                {
                    var fieldValueRuntimeType = info.GetString(field.Name + "RuntimeType");
                    field.SetValue(obj, info.GetValue(field.Name, Type.GetType(fieldValueRuntimeType)));
                }
            }

            return obj;
        }
    }
}

