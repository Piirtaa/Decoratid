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
    /// marker interface indicating the length prefix decoration is applied
    /// </summary>
    public interface ILengthPrefixStringable : IStringable
    {

    }

    /// <summary>
    /// encodes a string with a length prefix, delimited with rarechar.
    /// An encoded string will look like this Length{US}Data{RS}.  
    /// </summary>
    /// <remarks>
    /// This decoration does the following:
    /// -provides validation in terms of length
    /// -provides data required for the read process to work (presuming we follow a particular string reading approach.  this is an interesting
    /// design characteristic - giving partial behaviour on the process itself. see lists below)
    /// -removes special character encoding issues from the text
    /// 
    /// It does NOT address lists of strings, and "delimiter injection" problems, but it gives us an approach to use to handle them,
    /// which we do in LengthPrefixList decoration - to put the length of each item in a prefix.
    /// </remarks>
    [Serializable]
    public class LengthPrefixDecoration : StringableDecorationBase, ILengthPrefixStringable
    {
        public static string DELIM_PRE = Delim.US.ToString();
        public static string DELIM_POST = Delim.RS.ToString();

        #region Ctor
        public LengthPrefixDecoration(IStringable decorated)
            : base(decorated)
        {

        }
        #endregion

        #region ISerializable
        protected LengthPrefixDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public override string GetValue()
        {
            var val = this.Decorated.GetValue();
                int length = val == null? 0: val.Length;
                var rv = string.Format("{0}{1}{2}{3}", length.ToString(), DELIM_PRE, val, DELIM_POST);
                return rv;
        }
        public override void Parse(string text)
        {
            Condition.Requires(text).EndsWith(DELIM_POST);

            var length = text.MustGetTo(DELIM_PRE).ConvertToInt();
            var data = text.MustGetFrom(DELIM_PRE).Substring(0,length); //get data using the length parse
            var checkData = text.MustGetFrom(DELIM_PRE);
            checkData = checkData.Substring(0, checkData.Length - 1); //now get data using the delims
            Condition.Requires(data).IsEqualTo(checkData); //they should match

            //recursively, along the decoration chain, parse
            this.Decorated.Parse(data);
        }
        public override IDecorationOf<IStringable> ApplyThisDecorationTo(IStringable thing)
        {
            return new LengthPrefixDecoration(thing);
        }

        #endregion
    }

    public static class LengthPrefixDecorationExtensions
    {
        public static LengthPrefixDecoration DecorateWithLengthPrefix(this IStringable thing)
        {
            Condition.Requires(thing).IsNotNull();
            return new LengthPrefixDecoration(thing);
        }
    }
}
