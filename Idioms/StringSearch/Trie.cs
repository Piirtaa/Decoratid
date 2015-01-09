using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    /*Specialized Trie
     * Just like a normal trie, where each word has a value associated with it
        
     * Parsing sequence is different:
     * given RAWTEXT - text to parse
     * 1.  Matches only happen in this circumstance
     *      -the trie matches the word
     *      -the next character in the RAWTEXT is a space (a post match validation)
     *      -if we match a word but the next character isn't a space, it's not a match
     * 2.  After a match the match's ParseExtra function is invoked.  This will move the RAWTEXT cursor forward and grab
     *      any additional data associated with the match, before the next match is attempted
     *      
     * 
     *  
     */


    public class Trie
    {
        #region Inner Classes
        /// <summary>
        /// Node in a trie.
        /// </summary>
        private class TrieNode
        {
            #region Declarations
            private readonly char _char;
            private readonly Dictionary<char, TrieNode> _children = new Dictionary<char, TrieNode>();
            private string _word;
            private object _value;
            #endregion

            #region Ctor
            /// <summary>
            /// Constructor for the root node.
            /// </summary>
            private TrieNode()
            {
            }

            /// <summary>
            /// Constructor for a node with a word
            /// </summary>
            /// <param name="word"></param>
            /// <param name="parent"></param>
            private TrieNode(char word)
            {
                this._char = word;
            }
            #endregion

            #region Fluent Static
            public static TrieNode New()
            {
                return new TrieNode();
            }
            public static TrieNode New(char word)
            {
                return new TrieNode(word);
            }
            #endregion

            #region Properties
            public char Char
            {
                get { return _char; }
            }
            public string Word
            {
                get { return _word; }
            }
            public object Value
            {
                get { return _value; }
            }
            /// <summary>
            /// Children for this node indexed by their char
            /// </summary>
            /// <param name="c">Child word.</param>
            /// <returns>Child node.</returns>
            public TrieNode this[char c]
            {
                get
                {
                    TrieNode item = null;
                    _children.TryGetValue(c, out item);
                    return item;
                }
                set { _children[c] = value; }
            }
            #endregion

            #region Methods
            public bool HasWord()
            {
                return !string.IsNullOrEmpty(this.Word);
            }
            public void SetWordAndValue(string word, object val)
            {
                this._word = word;
                this._value = val;
            }
            #endregion
        }
        #endregion

        #region Declarations
        /// <summary>
        /// Root of the trie. It has no value and no parent.
        /// </summary>
        private readonly TrieNode _root = TrieNode.New();
        #endregion

        #region Ctor
        public Trie()
        {
        }
        #endregion

        #region Init Methods
        public void Add(string word, object val)
        {
            // start at the root
            var node = _root;

            // build a branch for the word, one letter at a time
            // if a letter node doesn't exist, add it
            foreach (char c in word)
            {
                var child = node[c];

                if (child == null)
                    child = node[c] = TrieNode.New(c); //sets item on the node

                //set the node.  on final run this will be the final node that holds the value
                node = child;
            }

            node.SetWordAndValue(word, val);
        }
        #endregion

        #region Search Methods
        public List<TrieMatch> FindTrieMatchesAtPosition(int idx, string text)
        {
            List<TrieMatch> rv = new List<TrieMatch>();

            //basic cursor position validation
            int maxIndex = text.Length - 1;

            if (maxIndex < idx)
                return rv;

            var currentIdx = idx;
            var currentNode = this._root;
            var currentChar = text[currentIdx];

            var queue = new Queue<Tuple<int, char, TrieNode>>();
            queue.Enqueue(new Tuple<int, char, TrieNode>(currentIdx, currentChar, currentNode));

            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                currentNode = tuple.Item3;
                currentChar = tuple.Item2;
                currentIdx = tuple.Item1;

                //if we're matching, return it
                if (currentNode.HasWord())
                {
                    var match = TrieMatch.New(currentIdx - 1 - currentNode.Word.Length, currentNode.Word, currentNode.Value);
                    rv.Add(match);
                }

                //does the trie even handle (aka lift, bro) this particular character?
                currentNode = currentNode[currentChar];
                if (currentNode != null)
                {
                    //get the next char
                    currentIdx++;
                    //validate position
                    if (maxIndex < currentIdx)
                        continue;
                    
                    currentChar = text[currentIdx];
                    queue.Enqueue(new Tuple<int, char, TrieNode>(currentIdx, currentChar, currentNode));
                }
            }

            return rv;
        }

        public List<TrieMatch> FindMatches(string text)
        {
            List<TrieMatch> rv = new List<TrieMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            for (int i = 0; i <= maxIndex; i++)
            {
                var list = FindTrieMatchesAtPosition(i, text);
                rv.AddRange(list);
            }
            return rv;
        }
        #endregion

    
    }



    public class TrieMatch
    {
        public int PositionInText { get; private set; }
        public string Word { get; private set; }
        public string Extra { get; private set; }
        public object Value { get; private set; }

        public static TrieMatch New(int pos, string word, object value)
        {
            return new TrieMatch() { PositionInText = pos, Word = word, Value = value };
        }
    }

    ///// <summary>
    ///// describes a substring of a string
    ///// </summary>
    //public class Substring
    //{
    //    #region Declarations

    //    #endregion

    //    #region Ctor
    //    public Substring(int startPos, int endPos, string data)
    //    {
    //        Condition.Requires(startPos).IsGreaterOrEqual(0);
    //        Condition.Requires(endPos).IsGreaterThan(startPos);
    //        Condition.Requires(data).IsNotNullOrEmpty().HasLength(endPos - startPos);
    //        this.StartPos = startPos;
    //        this.EndPos = endPos;
    //        this.Data = data;
    //    }
    //    #endregion

    //    #region Properties
    //    public int StartPos { get; private set; }
    //    public int EndPos { get; private set; }
    //    public string Data { get; private set; }
    //    #endregion
    //}
}