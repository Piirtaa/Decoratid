using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// a string wrapped as an IStringable.  Is implicitly convertable to/from string.  Use as an entrance point into 
    /// the Stringable idiom.
    /// </summary>
    [Serializable]
    public class NaturalStringable : IStringable, ISerializable
    {
        #region Declarations
        private string _value = null;
        #endregion

        #region Ctor
        public NaturalStringable(string text = null)
        {
            this._value = text;
        }
        #endregion

        #region ISerializable
        protected NaturalStringable(SerializationInfo info, StreamingContext context)
        {
            var data = info.GetString("data");
            this.Parse(data);
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected virtual void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", this.GetValue());
        }
        #endregion

        #region IStringable
        public string GetValue()
        {
            return _value;
        }
        public void Parse(string text)
        {
            this._value = text;
        }
        #endregion

        #region Implicit Conversions
        public static implicit operator string(NaturalStringable o)
        {
            if (o == null) { return null; }
            return o.GetValue();
        }
        public static implicit operator NaturalStringable(string text)
        {
            return new NaturalStringable(text);
        }
        #endregion

        #region Static Fluent 
        public static NaturalStringable New(string text = null)
        {
            return new NaturalStringable(text);
        }
        #endregion
    }

    public static class StringableExtensions
    {
        public static NaturalStringable MakeStringable(this string text)
        {
            return NaturalStringable.New(text);
        }
    }
}
