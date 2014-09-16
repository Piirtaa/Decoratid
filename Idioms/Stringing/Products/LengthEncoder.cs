using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Core;
using Decoratid.Idioms.Stringing.Decorations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing.Products
{
    /// <summary>
    /// uses istringable concepts to encode/decode text such that length prefix validations are decorated on the 
    /// data.  Supports infinite levels of nesting/decoration as the length encoding explicitly validates/reserves
    /// text.
    /// </summary>
    public static class LengthEncoder
    {
        /// <summary>
        /// encodes with length prefix
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string LengthEncode(string text)
        {
            var stringable = NaturalStringable.New(text).DecorateWithLengthPrefix();
            var rv = stringable.GetValue();
            return rv;
        }
        /// <summary>
        /// decodes something lengthencoded
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string LengthDecode(string text)
        {
            var stringable = NaturalStringable.New().DecorateWithLengthPrefix();
            stringable.Parse(text);
            var rv = stringable.Decorated.GetValue();
            return rv;
        }
        /// <summary>
        /// encodes a list with length prefix
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string LengthEncodeList(params string[] items)
        {
            var stringable = NaturalStringableList.New(items).DecorateWithLengthPrefixList();
            var rv =  stringable.GetValue();
            return rv;
        }
        /// <summary>
        /// decodes a lengthencoded list
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> LengthDecodeList(string text)
        {
            var stringable = NaturalStringableList.ParseNew(text);
            var rv =  stringable.ToList();
            return rv;
        }
    }
}
