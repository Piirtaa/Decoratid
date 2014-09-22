using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// a list of strings wrapped as an IStringableList.  Use as an entrance point into 
    /// the StringableList idiom.  Is stringable and formats as a delimited list.
    /// </summary>
    public class StringableList : List<string>, IStringableList
    {
        public static string ITEM_DELIM = Delim.US.ToString();

        #region Ctor
        public StringableList() : base()
        {
            
        }
        public StringableList(params string[] list)
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
        public static implicit operator string(StringableList o)
        {
            if (o == null) { return null; }
            return o.GetValue();
        }
        public static implicit operator StringableList(string text)
        {
            var rv =  new StringableList();
            rv.Parse(text);
            return rv;
        }
        #endregion

        #region Static Fluent
        public static StringableList New()
        {
            return new StringableList();
        }
        public static StringableList New(params string[] list)
        {
            return new StringableList(list);
        }
        public static StringableList ParseNew(string data)
        {
            var rv = new StringableList();
            rv.Parse(data);
            return rv;
        }
        #endregion
    }

    public static class StringableListExtensions
    {
        public static StringableList MakeStringableList(this List<string> list)
        {
            return StringableList.New(list.ToArray());
        }
        public static StringableList MakeStringableList(this string[] list )
        {
            return StringableList.New(list);
        }
    }
}
