using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// says items are delimited with the provided format
    /// </summary>
    public interface IDelimitedStringableList : IStringableList
    {
        string Prefix { get; }
        string Delimiter { get; }
        string Suffix { get; }

        bool IsListDelimitedFormatted(string text);
    }

    [Serializable]
    public class DelimitedStringableListDecoration : StringableListDecorationBase, IDelimitedStringableList
    {
        #region Ctor
        public DelimitedStringableListDecoration(IStringableList decorated, string prefix, string delim, string suffix)
            : base(decorated)
        {
            this.Prefix = prefix;
            this.Delimiter = delim;
            this.Suffix = suffix;
        }
        #endregion

        #region ISerializable
        protected DelimitedStringableListDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IDelimitedStringableList
        public string Prefix { get; private set; }
        public string Delimiter { get; private set; }
        public string Suffix { get; private set; }
        public bool IsListDelimitedFormatted(string text)
        {
            if (!string.IsNullOrEmpty(this.Prefix) && !text.StartsWith(Prefix))
                return false;

            if (!string.IsNullOrEmpty(this.Suffix) && !text.EndsWith(Suffix))
                return false;


            return true;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// converts the list to a length-prefixed string of the format:
        /// {RS}l0{GS}l1{GS},l2..{US}Data{RS}
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns></returns>
        public override string GetValue()
        {
            var coreVal = this.Decorated;
            StringBuilder rv = new StringBuilder();
            rv.Append(this.Prefix);
            rv.Append(string.Join(this.Delimiter, coreVal.ToList()));
            rv.Append(this.Suffix);
            return rv.ToString();
        }
        public override void Parse(string text)
        {
            //clear first
            this.Decorated.Clear();

            //validate
            if(!string.IsNullOrEmpty(this.Prefix))
                Condition.Requires(text).StartsWith(this.Prefix);
            if(!string.IsNullOrEmpty(this.Suffix))
                Condition.Requires(text).EndsWith(this.Suffix);
            
            var delimlist = text.MustGetFrom(this.Prefix);
            delimlist = delimlist.Substring(0, delimlist.Length - this.Suffix.Length);
            var split = new string[] { this.Delimiter };
            var list = delimlist.Split(split, StringSplitOptions.None);

            //slice up the rest of the data
            list.WithEach(x =>
            {
                this.Decorated.Add(x);
            });

        }
        public override IDecorationOf<IStringableList> ApplyThisDecorationTo(IStringableList thing)
        {
            return new DelimitedStringableListDecoration(thing, this.Prefix, this.Delimiter, this.Suffix);
        }
        #endregion
    }

    public static class DelimitedStringableListDecorationExtensions
    {
        public static DelimitedStringableListDecoration Delimit(this IStringableList thing, string prefix, string delim, string suffix)
        {
            Condition.Requires(thing).IsNotNull();
            return new DelimitedStringableListDecoration(thing, prefix, delim, suffix);
        }
        public static bool IsListDelimitedFormatted(this string text, string prefix, string delim, string suffix)
        {
            var stringable = NaturalStringableList.New().Delimit(prefix, delim, suffix);
            return stringable.IsListDelimitedFormatted(text);
        }
    }
}
