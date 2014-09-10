//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CuttingEdge.Conditions;
//using Sandbox.Extensions;

//namespace Sandbox.Thingness
//{

//    public class StateNode<TState, TTrigger>
//    {
//        public StateNode(TState state)
//        {
//            this.State = state;
//            this.Transitions = new Dictionary<TTrigger, TState>();
//        }

//        #region Properties
//        public TState State { get; set; }
//        public Dictionary<TTrigger, TState> Transitions { get; set; }
//        #endregion

//        #region Wire Methods
//        public StateNode<TState, TTrigger> Allow(TTrigger trigger, TState state)
//        {
//            this.Transitions[trigger] = state;
//            return this;
//        }
//        #endregion

//        #region Methods
//        public TState Trigger(TTrigger trigger)
//        {
//            if (!this.Transitions.ContainsKey(trigger))
//            {
//                return this.State;
//                //throw new InvalidOperationException("bad transition");
//            }
//            return this.Transitions[trigger];
//        }
//        #endregion
//    }

//    /// <summary>
//    /// Use this class to define a simple state machine graph. 
//    /// This class itself does not keep behaviour - it's simply a definition of the paths.
//    /// </summary>
//    /// <remarks>
//    /// see ServiceBase for an implementation example
//    /// </remarks>
//    [Serializable]
//    public class StateMachineGraph<TState, TTrigger>
//    {
//        #region Declarations
//        private readonly object _stateLock = new object();
//        #endregion

//        #region Ctor
//        public StateMachineGraph(TState initialState)
//        {
//            if (initialState == null) { throw new ArgumentNullException("initialState"); }

//            this.States = new Dictionary<TState, StateNode<TState, TTrigger>>();
//            this.InitialState = initialState;
//            this.CurrentState = initialState;
//        }
//        #endregion

//        #region Properties
//        public TState InitialState { get; private set; }
//        public Dictionary<TState, StateNode<TState, TTrigger>> States { get; private set; }
//        public TState CurrentState { get; private set; }
//        #endregion

//        #region Derived Properties
//        protected StateNode<TState, TTrigger> CurrentStateNode
//        {
//            get
//            {
//                return this.States[this.CurrentState];
//            }
//        }
//        #endregion

//        #region Registration Methods
//        /// <summary>
//        /// After defining a state, give it allowable transitions. 
//        /// </summary>
//        public StateNode<TState, TTrigger> ConfigureState(TState state)
//        {
//            StateNode<TState, TTrigger> node = null;

//            lock (this._stateLock)
//            {
//                //if the state doesn't exist, add it
//                if (!this.States.ContainsKey(state))
//                {
//                    node = new StateNode<TState, TTrigger>(state);
//                    this.States[state] = node;
//                }
//                else
//                {
//                    node = this.States[state];
//                }
//            }

//            return node;
//        }
//        #endregion

//        #region Methods
//        public void SetCurrentState(TState state)
//        {
//            this.CurrentState = state;
//        }
//        public void ResetToInitialState()
//        {
//            this.CurrentState = this.InitialState;
//        }
//        /// <summary>
//        /// returns true if the trigger will succeed
//        /// </summary>
//        /// <param name="trigger"></param>
//        /// <returns></returns>
//        public bool CanTrigger(TTrigger trigger)
//        {
//            bool returnValue = false;

//            lock (this._stateLock)
//            {
//                var newState = this.CurrentStateNode.Trigger(trigger);
//                if (!newState.Equals(this.CurrentState))
//                {
//                    returnValue = true;
//                }
//            }
//            return returnValue;
//        }

//        /// <summary>
//        /// returns true if the trigger changes state
//        /// </summary>
//        /// <param name="trigger"></param>
//        /// <returns></returns>
//        public bool Trigger(TTrigger trigger)
//        {
//            bool returnValue = false;

//            lock (this._stateLock)
//            {
//                var newState = this.CurrentStateNode.Trigger(trigger);
//                if (!newState.Equals(this.CurrentState))
//                {
//                    returnValue = true;
//                    this.CurrentState = newState;
//                }
//            }
//            return returnValue;
//        }
//        #endregion

//        #region Static Methods
//        /// <summary>
//        /// Clones a state machine
//        /// </summary>
//        /// <typeparam name="TState"></typeparam>
//        /// <param name="graph"></param>
//        /// <returns></returns>
//        public static StateMachineGraph<TState, TTrigger> Clone(StateMachineGraph<TState, TTrigger> graph)
//        {
//            if (graph == null) { return null; }

//            StateMachineGraph<TState, TTrigger> returnValue = new StateMachineGraph<TState, TTrigger>(graph.InitialState);

//            graph.States.WithEach(x =>
//            {
//                var state = returnValue.ConfigureState(x.Key);

//                x.Value.Transitions.WithEach(transition =>
//                {
//                    state.Allow(transition.Key, transition.Value);
//                });

//            });
//            returnValue.SetCurrentState(graph.CurrentState);
//            return returnValue;
//        }
//        #endregion

//    }



//}
