﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// a list of strings wrapped as an IStringableList.  Use as an entrance point into 
    /// the StringableList idiom.  Is stringable and formats as a delimited list.
    /// </summary>
    [Serializable]
    public class NaturalStringableList : List<string>, IStringableList, ISerializable
    {
        public static string ITEM_DELIM = Delim.US.ToString();

        #region Ctor
        public NaturalStringableList() : base()
        {
            
        }
        public NaturalStringableList(params string[] list)
            : base(list)
        {

        }
        #endregion
        
        #region ISerializable
        protected NaturalStringableList(SerializationInfo info, StreamingContext context) : base()
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
        public static implicit operator string(NaturalStringableList o)
        {
            if (o == null) { return null; }
            return o.GetValue();
        }
        public static implicit operator NaturalStringableList(string text)
        {
            var rv =  new NaturalStringableList();
            rv.Parse(text);
            return rv;
        }
        #endregion

        #region Static Fluent
        public static NaturalStringableList New()
        {
            return new NaturalStringableList();
        }
        public static NaturalStringableList New(params string[] list)
        {
            return new NaturalStringableList(list);
        }
        public static NaturalStringableList ParseNew(string data)
        {
            var rv = new NaturalStringableList();
            rv.Parse(data);
            return rv;
        }
        #endregion
    }

    public static class StringableListExtensions
    {
        public static NaturalStringableList MakeStringableList(this List<string> list)
        {
            return NaturalStringableList.New(list.ToArray());
        }
        public static NaturalStringableList MakeStringableList(this string[] list )
        {
            return NaturalStringableList.New(list);
        }
    }
}
