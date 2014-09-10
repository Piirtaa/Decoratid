using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Serialization;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Utils;

namespace Decoratid.Thingness.Idioms.ValuesOf
{

    /// <summary>
    /// interface giving us facility to decorate getting a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueOf<T>
    {
        T GetValue();
    }

    /// <summary>
    /// get a value by being given a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class ValueOf<T> : IValueOf<T>, ISerializable, IEquatable<ValueOf<T>>
    {
        #region Ctor
        public ValueOf(T value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.Value = value;
        }
        #endregion

        #region Properties
        private T Value { get; set; }
        #endregion

        #region Methods
        public T GetValue()
        {
            return this.Value;
        }
        #endregion

        #region Static Methods
        public static ValueOf<T> New(T value)
        {
            return new ValueOf<T>(value);
        }

        #endregion

        #region ISerializable
        private ValueOf(SerializationInfo info, StreamingContext context)
        {
            this.Value = (T)info.GetValue("_value", typeof(T));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_value", this.Value);
        }
        #endregion

        #region Equality Overrides
        public override bool Equals(object obj)
        {
            return Equals(obj as ValueOf<T>);
        }
        public override string ToString()
        {
            return this.Value.ToString();
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        #endregion

        #region IEquatable
        public bool Equals(ValueOf<T> other)
        {
            if (other == null)
                return false;

            ValueOf<T> cObj = (ValueOf<T>)other;
            return cObj.Value.Equals(this.Value);
        }
        #endregion
    }


}
