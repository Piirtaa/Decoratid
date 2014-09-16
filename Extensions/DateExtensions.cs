using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Extensions
{
    public static class DateExtensions
    {
        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }

        public static string ToStringUnixTime(this DateTime? dt)
        {
            if (dt.HasValue == false)
                return string.Empty;

            return dt.GetValueOrDefault().ToUnixTime();
        }
        public static DateTime? FromStringUnixTime(this string unixTime)
        {
            if (string.IsNullOrEmpty(unixTime))
                return null;

            return FromUnixTime(long.Parse(unixTime));
        }
    }
}
