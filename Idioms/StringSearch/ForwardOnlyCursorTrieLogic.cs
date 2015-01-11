using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// The default logic used by a Trie
    /// </summary>
    public static class ForwardOnlyCursorTrieLogic
    {
        public static List<StringSearchMatch> FindMatches(ITrieStructure trie, string text)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();

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

                        //if this matches, update the rv
                        var match = dequeueItem.GetWordMatch();
                        if (match != null)
                            rv.Add(match);

                        //can we carry the item over?
                        if (dequeueItem.MoveNext(currentChar))
                        {
                            reQueuedItems.Add(dequeueItem);
                        }
                    }

                    //queue up the ones that continue
                    foreach (var each in reQueuedItems)
                        queue.Enqueue(each);
                }


                //Possibly create a unit of work for this particular character (starting from root)
                var node = trie.Root[currentChar];
                if (node == null)
                    continue;

                MatchUoW uow = new MatchUoW(i, currentChar, node);
                queue.Enqueue(uow);
            }
            return rv;
        }

        public static List<StringSearchMatch> FindMatches2(ITrieStructure trie, string text)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();

            if (string.IsNullOrEmpty(text))
                return rv;

            var maxIndex = text.Length - 1;

            Stack<MatchUoW> bucket = new Stack<MatchUoW>();
            int bucketSize = 0;

            for (int i = 0; i <= maxIndex; i++)
            {
                //get current char
                char currentChar = text[i];

                //1.  set up new items
                Stack<MatchUoW> newItems = new Stack<MatchUoW>();
                var node = trie.Root[currentChar];
                if (node != null)
                {
                    MatchUoW uow = new MatchUoW(i, currentChar, node);
                    newItems.Push(uow);
                }

                //2.  handle old items
                for (int j = 0; j < bucketSize; j++)
                {
                    var eachOld = bucket.Pop();

                    //if this matches, update the rv
                    var match = eachOld.GetWordMatch();
                    if (match != null)
                        rv.Add(match);

                    //can we carry the item over?
                    if (eachOld.MoveNext(currentChar))
                    {
                        newItems.Push(eachOld);
                    }
                }

                bucketSize = newItems.Count;
                for (int j = 0; j < bucketSize; j++)
                {
                    bucket.Push(newItems.Pop());
                }
            }
            return rv;
        }

        /// <summary>
        /// a unit of work for each possible match.  
        /// </summary>
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
            public StringSearchMatch GetWordMatch()
            {
                if (!this.CurrentNode.HasWord)
                    return null;

                var match = StringSearchMatch.New(this.StartingIndex, this.CurrentNode.Id, this.CurrentNode.Value);
                return match;
            }
            #endregion
        }

    }

}
