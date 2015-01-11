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

namespace Decoratid.Idioms.StringSearch
{
    public class StringSearcher : IStringSearcher
    {
        #region Ctor
        public StringSearcher()
        {
            this.Words = new Dictionary<string, object>();
        }
        #endregion

        #region Properties
        private Dictionary<string, object> Words {get;set;}
        #endregion

        #region Methods

        public void Add(string word, object value)
        {
            this.Words[word] = value;
        }
        public List<StringSearchMatch> FindMatches(string text)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();

            var words = this.Words.Keys.ToArray();

            Parallel.For(0, words.Length - 1,
                () => new List<StringSearchMatch>(),
                (x, loop, subList) =>
                {
                    var word = words[x];
                    object val = this.Words[word];
                    string searchText = text;

                    int idx = searchText.IndexOf(word);

                    //search for this
                    while (idx > 0)
                    {
                        subList.Add(StringSearchMatch.New(idx, word, val));
                        searchText = searchText.Substring(idx + word.Length);
                        idx = searchText.IndexOf(word);
                    }

                    return subList;
                },
                (x) => { rv.AddRange(x); }
            );

            return rv;
        }
        #endregion

    }
}
