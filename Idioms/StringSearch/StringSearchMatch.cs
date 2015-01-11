using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    public interface IStringSearchMatch
    {
        int PositionInText { get; }
        string Word { get; }
    }

    public class StringSearchMatch : IStringSearchMatch
    {
        public int PositionInText { get; private set; }
        public string Word { get; private set; }
        public object Value { get; private set; }

        public static StringSearchMatch New(int pos, string word, object value)
        {
            return new StringSearchMatch() { PositionInText = pos, Word = word, Value = value };
        }
    }

}
