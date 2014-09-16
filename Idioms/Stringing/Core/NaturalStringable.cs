using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing.Core
{
    /// <summary>
    /// a string wrapped as an IStringable.  Is implicitly convertable to/from string.  Use as an entrance point into 
    /// the Stringable idiom.
    /// </summary>
    public class NaturalStringable : IStringable
    {
        #region Declarations
        private string _value = null;
        #endregion

        #region Ctor
        public NaturalStringable(string text = null)
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
}
