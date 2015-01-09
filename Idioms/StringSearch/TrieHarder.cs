//using CuttingEdge.Conditions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Decoratid.Idioms.StringSearch
//{

//    public class Trie : ITextMatcher
//    {
//        #region Inner Classes
//        /// <summary>
//        /// Node in a trie.
//        /// </summary>
//        private class TrieNode
//        {
//            #region Declarations
//            private readonly char _char;
//            private readonly Dictionary<char, TrieNode> _children = new Dictionary<char, TrieNode>();
//            private string _word;
//            private object _value;
//            #endregion

//            #region Ctor
//            /// <summary>
//            /// Constructor for the root node.
//            /// </summary>
//            private TrieNode()
//            {
//            }

//            /// <summary>
//            /// Constructor for a node with a word
//            /// </summary>
//            /// <param name="word"></param>
//            /// <param name="parent"></param>
//            private TrieNode(char word)
//            {
//                this._char = word;
//            }
//            #endregion

//            #region Fluent Static
//            public static TrieNode New()
//            {
//                return new TrieNode();
//            }
//            public static TrieNode New(char word)
//            {
//                return new TrieNode(word);
//            }
//            #endregion

//            #region Properties
//            public char Char
//            {
//                get { return _char; }
//            }
//            public string Word
//            {
//                get { return _word; }
//            }
//            public object Value
//            {
//                get { return _value; }
//            }
//            /// <summary>
//            /// Children for this node indexed by their char
//            /// </summary>
//            /// <param name="c">Child word.</param>
//            /// <returns>Child node.</returns>
//            public TrieNode this[char c]
//            {
//                get
//                {
//                    TrieNode item = null;
//                    _children.TryGetValue(c, out item);
//                    return item;
//                }
//                set { _children[c] = value; }
//            }
//            #endregion

//            #region Methods
//            public bool HasWord()
//            {
//                return !string.IsNullOrEmpty(this.Word);
//            }
//            public void SetWordAndValue(string word, object val)
//            {
//                this._word = word;
//                this._value = val;
//            }
//            #endregion
//        }
//        #endregion

//        #region Declarations
//        /// <summary>
//        /// Root of the trie. It has no value and no parent.
//        /// </summary>
//        private readonly TrieNode _root = TrieNode.New();
//        #endregion

//        #region Ctor
//        public Trie()
//        {
//        }
//        #endregion

//        #region ITextMatcher
//        public string[] Words { get; set; }
//        public ITextMatch FindNext(int startingIndex, string text);
//        public List<ITextMatch> FindAll(string text);
//        #endregion

//        #region Init Methods
//        public void Add(string word, object val)
//        {
//            // start at the root
//            var node = _root;

//            // build a branch for the word, one letter at a time
//            // if a letter node doesn't exist, add it
//            foreach (char c in word)
//            {
//                var child = node[c];

//                if (child == null)
//                    child = node[c] = TrieNode.New(c); //sets item on the node

//                //set the node.  on final run this will be the final node that holds the value
//                node = child;
//            }

//            node.SetWordAndValue(word, val);
//        }
//        #endregion

//        #region Search Methods
//        public List<TrieMatch> FindTrieMatchesAtPosition(int idx, string text, out int graspLengthOUT)
//        {
//            List<TrieMatch> rv = new List<TrieMatch>();
//            graspLengthOUT = 0;

//            //basic cursor position validation
//            int maxIndex = text.Length - 1;

//            if (maxIndex < idx)
//                return rv;

//            graspLengthOUT = 1;//we're at least processing 1 character
//            var currentIdx = idx;
//            var currentNode = this._root;
//            var currentChar = text[currentIdx];

//            //do the first test in the loop
//            currentNode = currentNode[currentChar];
//            if (currentNode == null)
//                return rv;

//            var queue = new Queue<Tuple<int, char, TrieNode>>();

//            //define function to increment position, and update currentIdx, currentChar, and queue it up
//            //implicit is the currentNode position has ALREADY been incremented at this point
//            Func<bool> enqueue = () =>
//            {
//                //get the next char
//                currentIdx++;
//                //validate position
//                if (maxIndex < currentIdx)
//                    return false;

//                currentChar = text[currentIdx];
//                queue.Enqueue(new Tuple<int, char, TrieNode>(currentIdx, currentChar, currentNode));
//                return true;
//            };

//            enqueue();

//            while (queue.Count > 0)
//            {
//                var tuple = queue.Dequeue();
//                currentNode = tuple.Item3;
//                currentChar = tuple.Item2;
//                currentIdx = tuple.Item1;

//                //if we're matching, return it
//                if (currentNode.HasWord())
//                {
//                    var match = TrieMatch.New(currentIdx - 1 - currentNode.Word.Length, currentNode.Word, currentNode.Value);
//                    rv.Add(match);

//                    //update grasp length
//                    var matchGraspLength = match.Word.Length;
//                    if (graspLengthOUT < matchGraspLength)
//                        graspLengthOUT = matchGraspLength;
//                }

//                //does the trie even handle (aka lift, bro) this particular character?
//                currentNode = currentNode[currentChar];
//                if (currentNode != null)
//                {
//                    enqueue();
//                }
//            }

//            return rv;
//        }

//        public List<TrieMatch> FindMatches(string text)
//        {
//            List<TrieMatch> rv = new List<TrieMatch>();

//            if (string.IsNullOrEmpty(text))
//                return rv;

//            var maxIndex = text.Length - 1;

//            for (int i = 0; i <= maxIndex; i++)
//            {
//                int grasp;
//                var list = FindTrieMatchesAtPosition(i, text, out grasp);
//                rv.AddRange(list);
//            }
//            return rv;
//        }
//        public List<TrieMatch> FindNonOverlappingMatches(string text)
//        {
//            List<TrieMatch> rv = new List<TrieMatch>();

//            if (string.IsNullOrEmpty(text))
//                return rv;

//            var maxIndex = text.Length - 1;

//            for (int i = 0; i <= maxIndex; )
//            {
//                int grasp;
//                var list = FindTrieMatchesAtPosition(i, text, out grasp);
//                rv.AddRange(list);
//                i = i + grasp;

//            }
//            return rv;
//        }
//        #endregion


//    }

//    public class TrieMatch
//    {
//        public int PositionInText { get; private set; }
//        public string Word { get; private set; }
//        public object Value { get; private set; }

//        public static TrieMatch New(int pos, string word, object value)
//        {
//            return new TrieMatch() { PositionInText = pos, Word = word, Value = value };
//        }
//    }


//}