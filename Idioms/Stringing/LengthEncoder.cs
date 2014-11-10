using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// uses istringable concepts to encode/decode text such that length prefix validations are decorated on the 
    /// data.  Supports infinite levels of nesting/decoration as the length encoding explicitly validates/reserves
    /// text.
    /// </summary>
    public static class LengthEncoder
    {
        #region Basic and List
        /// <summary>
        /// encodes with length prefix
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string LengthEncode(string text)
        {
            var stringable = NaturalStringable.New(text).DecorateWithLength();
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
            var stringable = NaturalStringable.New().DecorateWithLength();
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
            var stringable = NaturalStringableList.New(items).DecorateListWithLength();
            var rv = stringable.GetValue();
            return rv;
        }
        /// <summary>
        /// decodes a lengthencoded list
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> LengthDecodeList(string text)
        {
            var stringable = NaturalStringableList.New().DecorateListWithLength();
            stringable.Parse(text);
            List<string> rv = new List<string>();
            rv.AddRange(stringable);
            return rv;
        }
        #endregion

        #region Readable Formatter
        public static string MakeReadable(string text)
        {
            List<string> lines = new List<string>();
            MakeReadable(text, lines, 0);
            StringBuilder sb = new StringBuilder();
            lines.WithEach(x =>
            {
                sb.AppendLine(x);
            });

            var rv = sb.ToString();
            return rv;
        }
        private static void MakeReadable(string text, List<string> lines, int indentLevel = 0)
        {
            if (text.IsListLengthFormatted())
            {
                var list = LengthDecodeList(text);
                list.WithEach(line =>
                {
                    MakeReadable(line, lines, indentLevel + 1);
                });
            }
            else if (text.IsLengthFormatted())
            {
                //recurse 
                var innerText = LengthDecode(text);
                MakeReadable(innerText, lines, indentLevel + 1);
            }
            else
            {
                string indent = "=>".RepeatString(indentLevel);
                lines.Add(indent + text);
            }
        }
        #endregion
    }
}
