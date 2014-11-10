using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Extensions
{
    /// <summary>
    /// Extensions to convert strings to primitives
    /// </summary>
    public static class StringConversionExtensions
    {
        #region To Primitives
        public static bool ConvertToBool(this string data)
        {
            bool x;
            bool.TryParse(data, out x);
            return x;
        }
        public static decimal ConvertToDecimal(this string data)
        {
            decimal x;
            decimal.TryParse(data, out x);
            return x;
        }
        public static float ConvertToFloat(this string data)
        {
            float x;
            float.TryParse(data, out x);
            return x;
        }
        public static int ConvertToInt(this string data)
        {
            int x;
            int.TryParse(data, out x);
            return x;
        }
        #endregion

        #region To Nullable Primitives
        public static bool? ConvertToNullableBool(this string data)
        {
            if (string.IsNullOrEmpty(data)) { return null; }
            bool x;
            if (!bool.TryParse(data, out x))
            {
                return null;
            }
            return x;
        }
        public static decimal? ConvertToNullableDecimal(this string data)
        {
            if (string.IsNullOrEmpty(data)) { return null; }
            decimal x;
            if (!decimal.TryParse(data, out x))
            {
                return null;
            }
            return x;
        }
        public static float? ConvertToNullableFloat(this string data)
        {
            if (string.IsNullOrEmpty(data)) { return null; }
            float x;
            if (!float.TryParse(data, out x))
            {
                return null;
            }
            return x;
        }
        public static int? ConvertToNullableInt(this string data)
        {
            if (string.IsNullOrEmpty(data)) { return null; }
            int x;
            if (!int.TryParse(data, out x))
            {
                return null;
            }
            return x;
        }

        #endregion

        #region To String
        public static string ConvertToString<TInput>(this TInput o)
        where TInput : class
        {
            if (o == null) { return null; }
            return o.ToString();
        }
        public static string ConvertToString<TInput>(this TInput? o)
        where TInput : struct
        {
            if (!o.HasValue) { return null; }
            return o.ToString();
        }
      
        #endregion

        #region Base64 Conversion
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region To List
        /// <summary>
        /// creates a list from this string, with the first item being the original string 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<string> AddToList(this string item)
        {
            var list = new List<string>();
            list.Add(item);
            return list;
        }
        public static List<string> AddToList(this List<string> list, string item)
        {
            if (list == null)
            {
                return item.AddToList();
            }

            list.Add(item);
            return list;
        }
        #endregion

        #region Parsing
        /// <summary>
        /// Given a string beginning with a known prefix and ending with a known suffix, retrieves what is between them.
        /// Note: kacks if the string is non-conformant to this pattern
        /// </summary>
        /// <param name="text"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string GetDelimitedContents(this string text, string prefix, string suffix)
        {
            if(text == null)
                return null;

            if(text == string.Empty)
                return string.Empty;

            int startPos = 0;
            if (!string.IsNullOrEmpty(prefix))
            {
                Condition.Requires(text).StartsWith(prefix);
                startPos = prefix.Length;
            }
            int endPos = text.Length;
            if (!string.IsNullOrEmpty(suffix))
            {   
                Condition.Requires(text).EndsWith(suffix);
                endPos = text.Length - suffix.Length;
            }

            var rv = text.Substring(startPos, endPos - startPos);
            return rv;
        }
        /// <summary>
        /// parses value to the substring.  if not found returns self
        /// </summary>
        /// <param name="text"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
        public static string GetTo(this string text, string substring)
        {
            if (text == null)
                return null;

            if (text == string.Empty)
                return string.Empty;

            int idxOf = text.IndexOf(substring);

            if (idxOf == -1)
            {
                return text;
            }

            var rv = text.Substring(0, idxOf);

            return rv;
        }
       
        public static string GetFrom(this string text, string substring)
        {
            if (text == null)
                return null;

            if (text == string.Empty)
                return string.Empty;

            int idxOf = text.IndexOf(substring);

            if (idxOf == -1)
            {
                return text;
            }

            var rv = text.Substring(idxOf + substring.Length);

            return rv;
        }
        /// <summary>
        /// a GetTo that validates the substring exists and there is something to GetTo
        /// </summary>
        /// <param name="text"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
        public static string MustGetTo(this string text, string substring)
        {
            if (text == null)
                return null;

            if (text == string.Empty)
                return string.Empty;

            int idxOf = text.IndexOf(substring);

            if (idxOf == -1)
                throw new InvalidOperationException("substring not found");

            var rv = text.Substring(0, idxOf);

            return rv;
        }
        /// <summary>
        /// a GetFrom that validates the substring exists and there is something to GetFrom
        /// </summary>
        /// <param name="text"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
        public static string MustGetFrom(this string text, string substring)
        {
            if (text == null)
                return null;

            if (text == string.Empty)
                return string.Empty;

            int idxOf = text.IndexOf(substring);

            if (idxOf == -1)
                throw new InvalidOperationException("substring not found");

            var rv = text.Substring(idxOf + substring.Length);

            return rv;
        }
        #endregion

        #region Padding
        public static string RepeatString(this string text, int times)
        {
            if (times <= 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                sb.Append(text);
            }

            return sb.ToString();
        }
        #endregion
    }
}
