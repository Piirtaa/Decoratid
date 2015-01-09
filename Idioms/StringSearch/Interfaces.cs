using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// a single match from a text search
    /// </summary>
    public interface ITextSearchMatch
    {
        string Match { get; }
        int MatchPos { get; }
        object Value { get; }
    }

    /// <summary>
    /// the text searcher
    /// </summary>
    public interface ITextSearcher
    {
        IEnumerable<ITextSearchMatch> Search(string query);
        void AddKey(string key, object value);
        void RemoveKey(string key);
    }
}
