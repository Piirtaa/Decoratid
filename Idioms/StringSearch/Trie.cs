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
    /// a trie structure
    /// </summary>
    public interface ITrieStructure
    {
        /// <summary>
        /// the topmost or root node
        /// </summary>
        ITrieNode Root { get; }
        
        /// <summary>
        /// string indexer that does a lookup through the entire trie for the node at the given phrase/path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ITrieNode this[string path] { get; set; }
    }

    /// <summary>
    /// a node in a trie.  is recursive.  has list of child characters/nodes, and HasWord flag indicating it's a word, with optional
    /// placeholder Value for quick lookups
    /// </summary>
    /// <remarks>
    /// the id value represents the current path/ aka the Word
    /// </remarks>
    public interface ITrieNode : IHasId<string>
    {
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

    /// <summary>
    /// a trie: trie structure and string search alg to navigate it
    /// </summary>
    public interface ITrie : ITrieStructure, IStringSearcher
    {

    }


    /// <summary>
    /// basic implementation of a trie, with standard forward-only cursor alg
    /// </summary>
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

        #region ITrieStructure
        public ITrieNode Root { get { return _root; } }
        public void Add(string word, object value)
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
            node.Value = value;
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
                if (value != null)
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

        #region ITrieLogic
        /// <summary>
        /// finds matches using the forward only cursor approach
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<StringSearchMatch> FindMatches(string text)
        {
            return TrieLogic.FindMatchesUsingForwardOnlyCursor(this, text);
        }
        #endregion
    }

    public class TrieNode : ITrieNode
    {
        #region Declarations
        private readonly Dictionary<char, ITrieNode> _children = new Dictionary<char, ITrieNode>();
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

        #region Calculated Properties
        public char Letter { get { return this.Id.LastOrDefault(); } }
        #endregion
    }






}