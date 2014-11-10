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
    /// An encoded string will look like this {RS}Length{US}Data{RS}.  
    /// </summary>
    public interface ILengthFormattedStringable : IStringable
    {
        bool IsLengthFormatted(string text);
    }

    /// <summary>
    /// encodes a string with a length prefix, delimited with rarechar.
    /// An encoded string will look like this {RS}Length{US}Data{RS}.  
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
    public class LengthDecoration : StringableDecorationBase, ILengthFormattedStringable
    {
        public static string PREFIX = Delim.RS.ToString();
        public static string DELIM = Delim.US.ToString();
        public static string SUFFIX = Delim.RS.ToString();

        #region Ctor
        public LengthDecoration(IStringable decorated)
            : base(decorated)
        {

        }
        #endregion

        #region ISerializable
        protected LengthDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public bool IsLengthFormatted(string text)
        {
            if (!text.StartsWith(PREFIX))
                return false;
            
            if (!text.EndsWith(SUFFIX))
                return false;

            var length = text.GetFrom(PREFIX).GetTo(DELIM).ConvertToInt();
            if (length < 0)
                return false;

            //validate payload by length parse, and by delim parse
            var payloadByCount = text.GetFrom(DELIM).Substring(length);
            if (payloadByCount.Length != length)
                return false;
            
            var payloadByDelim = text.GetFrom(DELIM);
            payloadByDelim = payloadByDelim.Substring(0, payloadByDelim.Length - 1);
            if (!payloadByCount.Equals(payloadByDelim))
                return false;

            return true;
        }
        public override string GetValue()
        {
            var val = this.Decorated.GetValue();
            int length = val == null ? 0 : val.Length;
            var rv = string.Format("{0}{1}{2}{3}{4}", PREFIX, length.ToString(), DELIM, val, SUFFIX);
            return rv;
        }
        public override void Parse(string text)
        {
            if (!IsLengthFormatted(text))
                throw new InvalidOperationException("bad format");

            var data = text.GetFrom(DELIM);
            data = data.Substring(0, data.Length - 1);

            //recursively, along the decoration chain, parse
            this.Decorated.Parse(data);
        }
        public override IDecorationOf<IStringable> ApplyThisDecorationTo(IStringable thing)
        {
            return new LengthDecoration(thing);
        }

        #endregion
    }

    public static class LengthPrefixDecorationExtensions
    {
        public static LengthDecoration DecorateWithLength(this IStringable thing)
        {
            Condition.Requires(thing).IsNotNull();
            return new LengthDecoration(thing);
        }

        public static bool IsLengthFormatted(this string text)
        {
            var stringable = text.MakeStringable().DecorateWithLength();
            return stringable.IsLengthFormatted(text);
        }
    }
}
