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
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Utils;

namespace Decoratid.Idioms.Core.ValueOfing
{
    /// <summary>
    /// get a value by being given a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class NaturalValueOf<T> : IValueOf<T>, ISerializable, IEquatable<NaturalValueOf<T>>
    {
        #region Ctor
        public NaturalValueOf(T value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.Value = value;
        }
        #endregion

        #region Static Fluent
        public static NaturalValueOf<T> New(T value)
        {
            return new NaturalValueOf<T>(value);
        }
        #endregion

        #region ISerializable
        protected NaturalValueOf(SerializationInfo info, StreamingContext context)
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
            return Equals(obj as NaturalValueOf<T>);
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
        public bool Equals(NaturalValueOf<T> other)
        {
            if (other == null)
                return false;

            NaturalValueOf<T> cObj = (NaturalValueOf<T>)other;
            return cObj.Value.Equals(this.Value);
        }
        #endregion
    }


}
