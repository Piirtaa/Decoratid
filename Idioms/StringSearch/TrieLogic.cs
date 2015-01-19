using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// Different ways of walking a trie for search purposes
    /// </summary>
    public static class TrieLogic
    {
        #region Forward-Only Matching
        /// <summary>
        /// alg that only reads each character of the text to search, once.  at each index it creates possible
        /// match item that is queued.  Iterating thru the index these queued items are dequeued and processed for
        /// matching.  Sort of a snowplow approach that accumulates potential matches and simultaneously filters out bad
        /// ones, and when a word appears, it accumulates and returns it.
        /// </summary>
        /// <param name="trie"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<StringSearchMatch> FindMatchesUsingForwardOnlyCursor(ITrieStructure trie, string text)
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

        public static List<StringSearchMatch> FindMatchesUsingForwardOnlyCursor2a(ITrieStructure trie, string text)
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
                Queue<MatchUoW> reQueuedItems = new Queue<MatchUoW>();
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
                            reQueuedItems.Enqueue(dequeueItem);
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
        /// <summary>
        /// an attempt to do the forwardonlycursor alg using a stack instead of a queue. is slightly slower. not
        /// the tightest implementation either.
        /// </summary>
        /// <param name="trie"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<StringSearchMatch> FindMatchesUsingForwardOnlyCursor2(ITrieStructure trie, string text)
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
        #endregion

        #region By-position Matching
        /// <summary>
        /// for the provided index position, examines the trie to see if it can handle the character, and moves through
        /// the trie graph until it no longer matches.  returns list to account for situation where matches share a common suffix.
        /// </summary>
        /// <param name="trie"></param>
        /// <param name="idx"></param>
        /// <param name="text"></param>
        /// <param name="graspLengthOUT">the number of characters processed</param>
        /// <returns></returns>
        public static List<StringSearchMatch> FindMatchesAtPosition(ITrieStructure trie, int idx, string text, out int graspLengthOUT)
        {
            List<StringSearchMatch> rv = new List<StringSearchMatch>();
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
                    var match = StringSearchMatch.New(currentIdx - 1 - currentNode.Id.Length, currentNode.Id, currentNode.Value);
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

        #endregion
    }

}
