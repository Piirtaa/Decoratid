using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.CommandLine
{

    /*
     *  The parsing approach:
     *      given some text, a position, and a parser - the parser will attempt to parse the text and return 
     *      some return data (the token), the new position in text, and the next parser to continue with.
     * 
     *      It's a chain of responsibility that produces a list of tokens whilst parsing a string.  
     *      The program continues only when there is some text to parse and a parser to parse it with.
     *      Each step of the parse determines whether to continue (ie. to find the next parser).
     *      It's conceptually similar to a Turing machine, with each parsing step being 
     *      equivalent to a step in a computer program.  With Turing (unlike this), he needed 
     *      an infinite amount of text (ie. storage), and he moved back and forth along the text -
     *      enabling loops, and he wrote on the text itself. This approach simply moves forward only, 
     *      and tokenizes the source text.
     */ 

    public interface IToken
    {
        string GetStringValue();
    }

    public interface IForwardMovingTokenParser
    {

    }
    /// <summary>
    /// primitives are stringable (ie. parse/hydrate) things that also tell us if they can be parsed
    /// </summary>
    public interface ICommandLinePrimitive : IStringable
    {
        bool CanParse(string text);
    }
    /*
     * Decoratid Command Line primitive types are convertable to .NET value types, and also  
     * provide
     */

    /// <summary>
    /// struct that wraps DateTime as a YYYYMMDDhhmmss string
    /// </summary>
    public struct YYYYMMDDhhmmss : ICommandLinePrimitive
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

        #region ICommandLinePrimitive
        public bool CanParse(string text)
        {
            throw new NotImplementedException();
        }

        public string GetValue()
        {
            throw new NotImplementedException();
        }

        public void Parse(string text)
        {
            throw new NotImplementedException();
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

    public struct DCLText
    {

    }
}
