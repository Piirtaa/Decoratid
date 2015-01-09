using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// a node in a trie
    /// </summary>
    /// <remarks>
    /// the id value represents the current path/ aka the Word
    /// </remarks>
    public interface ITrieNode : IHasId<string>
    {
        ///// <summary>
        ///// the letter defining this node.  calculated: the last letter of the id
        ///// </summary>
        //char Letter { get; }

        /// <summary>
        /// the children nodes
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        ITrieNode this[char c] { get; set; }

        /// <summary>
        /// indicates whether this node terminates a word
        /// </summary>
        bool HasWord { get; set; }

        /// <summary>
        /// a placeholder for any values we want to lookup immediately upon a node match
        /// </summary>
        object Value { get; set; }
    }

    public class TrieNode : ITrieNode
    {
        #region Declarations
        private readonly Dictionary<char, ITrieNode> _children = new Dictionary<char, ITrieNode>();
        private string _word;
        #endregion

        #region Ctor
        public TrieNode(string id = null)
        {
            this.Id = id;
        }
        #endregion

        #region Fluent Static
        public static TrieNode New(string id = null)
        {
            return new TrieNode(id);
        }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        //public char Letter { get { return this.Id.LastOrDefault(); } }
        public bool HasWord { get; set; }
        public object Value { get; set; }
        /// <summary>
        /// Children for this node indexed by their char
        /// </summary>
        /// <param name="c">Child word.</param>
        /// <returns>Child node.</returns>
        public ITrieNode this[char c]
        {
            get
            {
                ITrieNode item = null;
                _children.TryGetValue(c, out item);
                return item;
            }
            set { _children[c] = value; }
        }
        #endregion
    }

    /// <summary>
    /// a trie structure
    /// </summary>
    public interface ITrie
    {
        ITrieNode Root { get; }
        void Add(string word);
        ITrieNode this[string path] { get; set; }
    }

    public class Trie : ITrie
    {
        #region Declarations
        /// <summary>
        /// Root of the trie. It has no value and no parent.
        /// </summary>
        private readonly ITrieNode _root = TrieNode.New();
        #endregion

        #region Ctor
        public Trie()
        {
        }
        #endregion

        #region ITrie
        public ITrieNode Root { get { return _root; } }
        public void Add(string word)
        {
            // start at the root
            var node = _root;

            string prefix = "";

            // build a branch for the word, one letter at a time
            // if a letter node doesn't exist, add it
            foreach (char c in word)
            {
                var child = node[c];

                if (child == null)
                    child = node[c] = TrieNode.New(prefix + c); //sets item on the node

                //update the prefix
                prefix = prefix + c;

                //set the node.  on final run this will be the final node that holds the value
                node = child;
            }

            node.HasWord = true;
        }
        public ITrieNode this[string path]
        {
            get
            {
                //walk from the root to the node, by each character in the path
                ITrieNode node = this.Root;

                foreach (char c in path)
                {
                    node = node[c]; //walk to child
                    if (node == null)
                        return null; //kack out with a null
                }

                return node;
            }
            set
            {
                //validate the value
                if(value != null)
                    Condition.Requires(value.Id).IsEqualTo(path);

                //walk from the root to the parent of the node we want to set
                ITrieNode node = this.Root;

                var parentPath = path.Substring(path.Length - 1);
                foreach (char c in parentPath)
                {
                    node = node[c];
                    if (node == null)
                        return; //kack out 
                }

                node[path.Last()] = value;
            }
        }
        #endregion
    }

    public class TrieMatch
    {
        public int PositionInText { get; private set; }
        public string Word { get; private set; }
        public object Value { get; private set; }

        public static TrieMatch New(int pos, string word, object value)
        {
            return new TrieMatch() { PositionInText = pos, Word = word, Value = value };
        }
    }

    //AND NOW DEFINE THE SEARCH ALGORITHMS THAT OPERATE ON THE TRIE STRUCTURE!!!!!

    public static class TrieSearch_ForwardOnlyCursor
    {

        public static List<TrieMatch> FindMatches(ITrie trie, string text)
        {
            List<TrieMatch> rv = new List<TrieMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            Queue<MatchUoW> queue = new Queue<MatchUoW>();

            for (int i = 0; i <= maxIndex; i++)
            {
                //get current char
                char currentChar = text[i];

                //dequeue all carryover items, and identify which ones can continue
                List<MatchUoW> reQueuedItems = new List<MatchUoW>();
                int queueCount = queue.Count;
                if (queueCount > 0)
                {
                    for (int j = 0; j < queueCount; j++)
                    {
                        MatchUoW dequeueItem = queue.Dequeue();

                        //can we carry the item over?
                        if (dequeueItem.MoveNext(currentChar))
                        {
                            reQueuedItems.Add(dequeueItem);

                            //test for word match
                            var match = dequeueItem.GetWordMatch();
                            if (match != null)
                                rv.Add(match);
                        }
                    }

                    //queue up the ones that continue
                    foreach (var each in reQueuedItems)
                        queue.Enqueue(each);
                }

                var node = trie.Root[currentChar];
                if (node == null)
                    continue;

                MatchUoW uow = new MatchUoW(i, currentChar, node);
                queue.Enqueue(uow);
            }
            return rv;
        }
        
        private class MatchUoW
        {
            #region Ctor
            public MatchUoW(int index, char currentChar, ITrieNode currentNode)
            {
                Condition.Requires(currentNode).IsNotNull();

                this.StartingIndex = index;
                this.CurrentIndex = index;
                this.CurrentWord = new string(currentChar, 1);
                this.CurrentNode = currentNode;
            }
            #endregion

            #region Properties
            public int StartingIndex { get; private set; }
            public int CurrentIndex { get; private set; }
            public string CurrentWord { get; private set; }
            public ITrieNode CurrentNode { get; private set; }
            #endregion

            #region Methods
            /// <summary>
            /// tests if the current node can handle the next char, and updates match if so
            /// </summary>
            /// <param name="nextChar"></param>
            /// <returns></returns>
            public bool MoveNext(char nextChar)
            {
                var node = this.CurrentNode[nextChar];
                if (node == null)
                    return false;

                //increment index
                this.CurrentIndex++;
                this.CurrentNode = node;
                this.CurrentWord = this.CurrentWord + nextChar;
                return true;
            }
            /// <summary>
            /// if the current node has a word, we build a match to return
            /// </summary>
            /// <returns></returns>
            public TrieMatch GetWordMatch()
            {
                if (!this.CurrentNode.HasWord)
                    return null;

                var match = TrieMatch.New(this.StartingIndex, this.CurrentNode.Id, this.CurrentNode.Value);
                return match;
            }
            #endregion
        }

    }

    public static class TrieSearch_SeekAhead
    {
        /// <summary>
        /// for the provided index position, examines the trie to see if it can handle the character, and moves through
        /// the trie graph until it no longer matches.  returns list to account for situation where matches share a common suffix.
        /// </summary>
        /// <param name="trie"></param>
        /// <param name="idx"></param>
        /// <param name="text"></param>
        /// <param name="graspLengthOUT">the number of characters processed</param>
        /// <returns></returns>
        public static List<TrieMatch> FindMatchesAtPosition(ITrie trie, int idx, string text, out int graspLengthOUT)
        {
            List<TrieMatch> rv = new List<TrieMatch>();
            graspLengthOUT = 0;

            //basic cursor position validation
            int maxIndex = text.Length - 1;

            if (maxIndex < idx)
                return rv;

            graspLengthOUT = 1;//we're at least processing 1 character
            var currentIdx = idx;
            var currentNode = trie.Root;
            var currentChar = text[currentIdx];

            //do the first test in the loop
            currentNode = currentNode[currentChar];
            if (currentNode == null)
                return rv;

            var queue = new Queue<Tuple<int, char, ITrieNode>>();

            //define function to increment position, and update currentIdx, currentChar, and queue it up
            //implicit is the currentNode position has ALREADY been incremented at this point
            Func<bool> enqueue = () =>
            {
                //get the next char
                currentIdx++;
                //validate position
                if (maxIndex < currentIdx)
                    return false;

                currentChar = text[currentIdx];
                queue.Enqueue(new Tuple<int, char, ITrieNode>(currentIdx, currentChar, currentNode));
                return true;
            };

            enqueue();

            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                currentNode = tuple.Item3;
                currentChar = tuple.Item2;
                currentIdx = tuple.Item1;

                //if we're matching, return it
                if (currentNode.HasWord)
                {
                    var match = TrieMatch.New(currentIdx - 1 - currentNode.Id.Length, currentNode.Id, currentNode.Value);
                    rv.Add(match);

                    //update grasp length
                    var matchGraspLength = match.Word.Length;
                    if (graspLengthOUT < matchGraspLength)
                        graspLengthOUT = matchGraspLength;
                }

                //does the trie even handle (aka lift, bro) this particular character?
                currentNode = currentNode[currentChar];
                if (currentNode != null)
                {
                    enqueue();
                }
            }

            return rv;
        }
        
        public static List<TrieMatch> FindMatches(ITrie trie, string text)
        {
            List<TrieMatch> rv = new List<TrieMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            for (int i = 0; i <= maxIndex; i++)
            {
                int grasp;
                var list = FindMatchesAtPosition(trie, i, text, out grasp);
                rv.AddRange(list);
            }
            return rv;
        }
        public static List<TrieMatch> FindNonOverlappingMatches(ITrie trie, string text)
        {
            List<TrieMatch> rv = new List<TrieMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            for (int i = 0; i <= maxIndex; )
            {
                int grasp;
                var list = FindMatchesAtPosition(trie, i, text, out grasp);
                rv.AddRange(list);
                i = i + grasp;

            }
            return rv;
        }

    }

}