using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Stringable
{
    /// <summary>
    /// a string wrapped as an IStringable.  Is implicitly convertable to/from string.  Use as an entrance point into 
    /// the Stringable idiom.
    /// </summary>
    public class RawStringable : IStringable
    {
        #region Declarations
        private string _value = null;
        #endregion

        #region Ctor
        public RawStringable(string text = null)
        {
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
        public static implicit operator string(RawStringable o)
        {
            if (o == null) { return null; }
            return o.GetValue();
        }
        public static implicit operator RawStringable(string text)
        {
            return new RawStringable(text);
        }
        #endregion

        #region Static Fluent 

        public static RawStringable New(string text = null)
        {
            return new RawStringable(text);
        }
        #endregion
    }
}
