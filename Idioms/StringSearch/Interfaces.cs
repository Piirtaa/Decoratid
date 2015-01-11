using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{

    public interface IStringSearcher 
    {
        void Add(string word, object value);
        List<StringSearchMatch> FindMatches(string text);
    }

}
