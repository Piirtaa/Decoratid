using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch.PDuniho
{
    /*Example usage:
     * 
     * It's a generic class, and you have to create it using two types: a  
"TransitionKey" which is the type that will be used to determine which  
state to transition to from a given state, and a "Terminal" which is  
whatever kind of data you want to get back after matching something.

In your case, I would guess that "TransitionKey" would be "char" and  
"Terminal" might simply be "string" (if all you need is the actual text  
found), or perhaps some kind of index used in your matching (so "int" or  
some other kind of indexing value).  There are methods in the class then  
that take sequences of transition keys and do things with them: add a  
sequence to the graph, remove a sequence from the graph, and search in a  
given sequence for sub-sequences already in the graph.
     * 
     * s an example, let's say you had an array of strings ("rgstrFind") that  
you want to find within some text ("strText").  You might use the graph  
class like this:

     int[] IndicesSearchText(string strText, string[] rgstrFind)
     {
         StateGraph<char, int> graph = new StateGraph<char, int>();

         for (int istr = 0; istr < rgstrFind.Length; istr++)
         {
             graph.Add(rgstrFind[istr], istr);
         }

         return RgobjFromCollection(strText);
     }

The function would return an array of indices, corresponding to the  
entries in "rgstrFind", one for each matching string (with duplicates, if  
a given string matched more than once).
     * 
     * 
     * 
     * In the class I also included the functionality I alluded to with respect  
to finding overlapping strings.  There's a different method, called  
RgobjFromCollectionParallel() that does the same thing as above, except  
that it maintains multiple concurrent states, starting a new traversal  
with each character in the string.  By doing that, it will find all  
instances of your search strings, even if they overlap or are contained in  
one another.  Again, duplicates will be in the results if any given input  
string matches more than once.
     * 
     */
    public class StateGraph<TransitionKey, Terminal>
    {       
        #region Private Members

        // The initial state of the graph
        private StateNode _stateRoot = new StateNode();

        // The current state of the graph. Used only when the client uses
        // the "one state at a time" RgobjFromNextState() method.
        private StateNode _stateCur = null;

        // An individual node within the state graph.
        private class StateNode
        {
            // Lookup table to find the next state, given a transition key
            private Dictionary<TransitionKey, StateNode> _mpchstate = new
Dictionary<TransitionKey, StateNode>();

            // List of objects associated with this state. Empty for any  state
            // that doesn't terminate a string.
            private List<Terminal> _lobj = new List<Terminal>();

            // Gets a copy of the list of objects associated with this  state
            public Terminal[] RgobjItems
            {
                get { return _lobj.ToArray(); }
            }

            // Gets the number of objects associated with this state
            public int CobjTerminal
            {
                get { return _lobj.Count; }
            }

            // Gets the number non-root states reachable by transition from
            // this state.
            public int Clink
            {
                get { return _mpchstate.Count; }
            }

            // Adds a new object associated with this state
            public void AddTerminalObj(Terminal obj)
            {
                _lobj.Add(obj);
            }

            // Removes a new object associated with this state
            public void RemoveTerminalObj(Terminal obj)
            {
                _lobj.Remove(obj);
            }

            public StateNode NextState(TransitionKey tk, bool fAdd)
            {
                StateNode state = null;

                if (_mpchstate.ContainsKey(tk))
                {
                    state = _mpchstate[tk];
                }
                else if (fAdd)
                {
                    state = new StateNode();
                    _mpchstate.Add(tk, state);
                }

                return state;
            }
            //// Given a transition key, find the next state. Optionally,
            //// add a new state when the key isn't yet a valid transition
            //// for this state.
            //public StateNode NextState(TransitionKey tk, bool fAdd)
            //{
            //    StateNode state;

            //    try
            //    {
            //        state = _mpchstate[tk];
            //    }
            //    catch (KeyNotFoundException)
            //    {
            //        state = null;
            //    }

            //    if (state == null && fAdd)
            //    {
            //        state = new StateNode();
            //        _mpchstate.Add(tk, state);
            //    }
            //    return state;
            //}

            // Remove a given transition to another state from this state
            public void RemoveLink(TransitionKey tk)
            {
                _mpchstate.Remove(tk);
            }
        }

        // Removes a terminal object associated with a given sequence of
        // transition keys from the graph. This will also remove any state
        // nodes that as a result wind up without any useful information
        // in them (either a transition to a new state, or a list of  objects
        // associated with a terminal state).
        private StateNode _StateRemove(TransitionKey[] rgtkTerminal, int itk, Terminal obj, StateNode stateCur)
        {
            if (itk < rgtkTerminal.Length)
            {
                TransitionKey tk = rgtkTerminal[itk];
                StateNode stateNext = stateCur.NextState(tk, false);

                if (stateNext == null)
                {
                    throw new ArgumentException("String not found in graph", "rgtkTerminal");
                }

                stateNext = _StateRemove(rgtkTerminal, itk + 1, obj,
stateNext);

                if (stateNext == null)
                {
                    stateCur.RemoveLink(tk);
                    if (stateCur.Clink == 0)
                    {
                        stateCur = null;
                    }
                }
            }
            else
            {
                if (stateCur.CobjTerminal == 0)
                {
                    throw new ArgumentException("String not found in graph", "rgtkTerminal");
                }

                stateCur.RemoveTerminalObj(obj);
                if (stateCur.CobjTerminal == 0)
                {
                    stateCur = null;
                }
            }

            return stateCur;
        }

        private StateNode _NodeNextState(TransitionKey tk, StateNode state)
        {
            StateNode stateNew = state.NextState(tk, false);

            if (stateNew == null)
            {
                stateNew = _stateRoot.NextState(tk, false);
            }

            return stateNew != null ? stateNew : _stateRoot;
        }

        #endregion

        #region Public Methods

        #region Graph Maintenance Methods

        // Adds a new object to a given sequence of transition keys in the
        // graph. Traverses existing transitions and adds new states and
        // transitions as necessary.
        public void Add(IEnumerable<TransitionKey> rgtkTerminal, Terminal obj)
        {
            StateNode state = _stateRoot;

            foreach (TransitionKey tk in rgtkTerminal)
            {
                state = state.NextState(tk, true);
            }

            state.AddTerminalObj(obj);
        }

        // Removes a given sequence of transition keys from the graph.
        public void Remove(TransitionKey[] rgtkTerminal, Terminal obj)
        {
            _StateRemove(rgtkTerminal, 0, obj, _stateRoot);
        }

        // Clears the entire graph
        public void Clear()
        {
            _stateRoot = new StateNode();
        }

        #endregion

        #region Graph Traversal Methods

        // Reset the current state of the graph. Used only in conjunction
        // with the RgobjFromNextState() method.
        public void ResetGraphState()
        {
            _stateCur = _stateRoot;
        }

        // Transition the graph to the next state given a key, and return
        // the array of objects associated with the new state
        public Terminal[] RgobjFromNextState(TransitionKey tk)
        {

            _stateCur = _NodeNextState(tk, _stateCur);

            return _stateCur.RgobjItems;
        }

        // Given a sequence of transition keys, traverses the state graph
        // linearly and returns all of the objects associated with each  state
        // visited during the traversal.
        public Terminal[] RgobjFromCollection(IEnumerable<TransitionKey> rgtk)
        {
            List<Terminal> lobj = new List<Terminal>();

            foreach (TransitionKey tk in rgtk)
            {
                lobj.AddRange(RgobjFromNextState(tk));
            }

            return lobj.ToArray();
        }

        // Given a sequence of transition keys, returns all matching  sequences of
        // transition keys represented in the graph. This method will correctly
        // find all overlapping transition sequences, including transition sequences
        // contained by other transition sequences.
        public Terminal[] RgobjFromCollectionParallel(IEnumerable<TransitionKey> rgtk)
        {
            Queue<StateNode> qstate = new Queue<StateNode>();
            List<Terminal> lobj = new List<Terminal>();

            foreach (TransitionKey tk in rgtk)
            {
                qstate.Enqueue(_stateRoot);
                for (int i = qstate.Count; i > 0; i--)
                {
                    StateNode state = qstate.Dequeue();

                    state = _NodeNextState(tk, state);
                    lobj.AddRange(state.RgobjItems);

                    if (state != _stateRoot && state.Clink > 0)
                    {
                        qstate.Enqueue(state);
                    }
                }
            }

            return lobj.ToArray();
        }
        #endregion

        #endregion


    }
}