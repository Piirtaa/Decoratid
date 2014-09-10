using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Stringable
{
    /// <summary>
    /// a list of strings wrapped as an IStringableList.  Use as an entrance point into 
    /// the StringableList idiom.  Is stringable and formats as a delimited list.
    /// </summary>
    public class RawStringableList : List<string>, IStringableList
    {
        public static string ITEM_DELIM = Delim.US.ToString();

        #region Ctor
        public RawStringableList() : base()
        {
            
        }
        public RawStringableList(params string[] list)
            : base(list)
        {

        }
        #endregion

        #region IStringable
        public string GetValue()
        {
            var rv = string.Join(ITEM_DELIM, this.ToList());
            return rv;
        }
        public void Parse(string text)
        {
            this.Clear();

            if (!string.IsNullOrEmpty(text))
            {
                string[] split = new string[]{ITEM_DELIM};
                var arr = text.Split(split, StringSplitOptions.None);
                if (arr != null && arr.Length > 0)
                {
                    foreach (var each in arr)
                    {
                        this.Add(each);
                    }
                }
            }
        }
        #endregion

        #region Implicit Conversions
        public static implicit operator string(RawStringableList o)
        {
            if (o == null) { return null; }
            return o.GetValue();
        }
        public static implicit operator RawStringableList(string text)
        {
            var rv =  new RawStringableList();
            rv.Parse(text);
            return rv;
        }
        #endregion

        #region Static Fluent
        public static RawStringableList New()
        {
            return new RawStringableList();
        }
        public static RawStringableList New(params string[] list)
        {
            return new RawStringableList(list);
        }
        public static RawStringableList ParseNew(string data)
        {
            var rv = new RawStringableList();
            rv.Parse(data);
            return rv;
        }
        #endregion
    }
}
