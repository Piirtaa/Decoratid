using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Core
{
    /// <summary>
    /// struct that wraps DateTime as a YYYYMMDDhhmmss string
    /// </summary>
    public struct YYYYMMDDhhmmss
    {
        #region Declarations
        private readonly DateTime _val;
        #endregion

        #region Ctor
        public YYYYMMDDhhmmss(DateTime dt)
        {
            this._val = dt;
        }
        public YYYYMMDDhhmmss(string dt)
        {
            Condition.Requires(dt).IsNotNullOrEmpty().IsNotShorterThan(8);

            //TODO: parse this using a trie
            var length = dt.Length;

            int year = int.Parse(dt.Substring(0, 4));
            int month = int.Parse(dt.Substring(4, 2));
            int day = int.Parse(dt.Substring(6, 2));
            int hour = 0; int minute = 0; int second = 0;

            if (dt.Length >= 10)
                hour = int.Parse(dt.Substring(8, 2));

            if (dt.Length >= 12)
                minute = int.Parse(dt.Substring(10, 2));

            if (dt.Length >= 14)
                second = int.Parse(dt.Substring(12, 2));

            this._val = new DateTime(year, month, day, hour, minute, second);
        }
        #endregion

        #region Properties
        public DateTime Value { get { return _val; } }
        #endregion

        #region Implicit Conversions
        public static implicit operator string(YYYYMMDDhhmmss o)
        {
            return o.ToString();
        }
        public static implicit operator DateTime(YYYYMMDDhhmmss o)
        {
            return o.Value;
        }
        public static implicit operator YYYYMMDDhhmmss(DateTime o)
        {
            return new YYYYMMDDhhmmss(o);
        }
        public static implicit operator YYYYMMDDhhmmss(string dt)
        {
            return new YYYYMMDDhhmmss(dt);
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            var year = this.Value.Year.ToString().PadLeft(4, '0');
            var month = this.Value.Month.ToString().PadLeft(2, '0');
            var day = this.Value.Day.ToString().PadLeft(2, '0');
            var hour = this.Value.Hour.ToString().PadLeft(2, '0');
            var minute = this.Value.Minute.ToString().PadLeft(2, '0');
            var second = this.Value.Second.ToString().PadLeft(2, '0');
            return year + month + day + hour + minute + second;
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        #endregion
    }
}
