using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.ValueOfing
{
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

        #region Static Fluent
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
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_value", this.Value);
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
