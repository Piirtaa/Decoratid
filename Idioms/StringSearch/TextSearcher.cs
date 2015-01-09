//using Gma.DataStructures.StringSearch;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Decoratid.Idioms.StringSearch
//{
//    public class TextSearcher: ITextSearcher
//    {
//        #region Declarations
//        private ITrie<object> _trie;
//        #endregion

//        #region Ctor
//        public TextSearcher()
//        {
//            _trie = new PatriciaTrie<object>();
//        }
//        #endregion

//        #region ITextSearcher
//        public void AddKey(string key, object value)
//        {
//            _trie.Add(key, value); 
//        }
//        public void RemoveKey(string key)
//        {
//            //no method avail
//        }
//        public IEnumerable<ITextSearchMatch> Search(string query)
//        {
//            _trie.Retrieve(query);

//        }
//        #endregion
//    }
//}
