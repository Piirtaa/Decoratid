using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Extensions;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Storing.Core;
using Decoratid.Idioms.Storing.Decorations.StoreOf;

namespace Decoratid.Idioms.StateMachineable
{
    /// <summary>
    /// defines a state transition (as an item in a store)
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TTrigger"></typeparam>
    public class StateTransition<TState, TTrigger> : IHasId<string>
    {
        #region Ctor
        public StateTransition(TState fromState, TState toState, TTrigger transitionTrigger)
        {
            this.FromState = fromState;
            this.ToState = toState;
            this.TransitionTrigger = transitionTrigger;
        }
        #endregion

        #region Properties
        public TState FromState { get; private set; }
        public TState ToState { get; private set; }
        public TTrigger TransitionTrigger { get; private set; }
        #endregion

        #region IHasId
        public string Id { get { return string.Format("{0}:{1}:{2}", this.FromState, this.ToState, this.TransitionTrigger); } }
        object IHasId.Id { get { return this.Id; } }
        #endregion
    }

    /// <summary>
    /// Use this class to define a simple state machine graph. 
    /// This class itself does not keep behaviour - it's simply a definition of the paths. 
    /// Wraps a store - so has access to Decoratids.
    /// </summary>
    /// <remarks>
    /// see ServiceBase for an implementation example
    /// </remarks>
    [Serializable]
    public class StateMachineGraph<TState, TTrigger>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StateMachineGraph(TState initialState)
        {
            if (initialState == null) { throw new ArgumentNullException("initialState"); }
            this.InitialState = initialState;
            this.CurrentState = initialState;

            this.Store = new StoreOfDecoration<StateTransition<TState,TTrigger>>( new NaturalInMemoryStore());
        }
        public StateMachineGraph(TState initialState, IStoreOf<StateTransition<TState, TTrigger>> store)
        {
            if (initialState == null) { throw new ArgumentNullException("initialState"); }
            this.InitialState = initialState;
            this.CurrentState = initialState;

            Condition.Requires(store).IsNotNull();
            this.Store = store;
        }
        #endregion

        #region Properties
        public TState InitialState { get; private set; }
        private IStoreOf<StateTransition<TState,TTrigger>> Store { get;  set; }
        public TState CurrentState { get; private set; }
        #endregion

        #region Registration Methods
        public void AllowTransition(TState fromState, TState toState, TTrigger trigger)
        {
            lock (this._stateLock)
            {
                StateTransition<TState, TTrigger> trans = new StateTransition<TState, TTrigger>(fromState, toState, trigger);
                this.Store.SaveItem(trans);
            }
        }
        #endregion

        #region Methods
        public void SetCurrentState(TState state)
        {
            lock (this._stateLock)
            {
                this.CurrentState = state;
            }
        }
        public void ResetToInitialState()
        {
            lock (this._stateLock)
            {
                this.CurrentState = this.InitialState;
            }
        }
        /// <summary>
        /// returns true if the trigger will succeed
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public bool CanTrigger(TTrigger trigger)
        {
            //does this transition from the current state exist?
            SearchFilterOf<StateTransition<TState,TTrigger>> filter = new SearchFilterOf<StateTransition<TState,TTrigger>>((x)=>
            {
                return x.FromState.Equals(this.CurrentState) && x.TransitionTrigger.Equals(trigger);
            });

            var list = this.Store.Search<StateTransition<TState, TTrigger>>(filter);
            return list != null && list.Count > 0;
        }

        /// <summary>
        /// returns true if the trigger changes state
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public bool Trigger(TTrigger trigger)
        {
            lock (this._stateLock)
            {
                //does this transition from the current state exist?
                SearchFilterOf<StateTransition<TState, TTrigger>> filter = new SearchFilterOf<StateTransition<TState, TTrigger>>((x) =>
                {
                    return x.FromState.Equals(this.CurrentState) && x.TransitionTrigger.Equals(trigger);
                });

                var list = this.Store.Search<StateTransition<TState, TTrigger>>(filter);

                if (list == null || list.Count == 0)
                    return false;

                var toState = list.FirstOrDefault();
                this.SetCurrentState(toState.ToState);
                return true;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Clones a state machine
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static StateMachineGraph<TState, TTrigger> Clone(StateMachineGraph<TState, TTrigger> graph)
        {
            if (graph == null) { return null; }
           
            var data = StoreSerializer.SerializeStore(graph.Store);
            IStore cloneStore = StoreSerializer.DeserializeStore(data);
            var store = new StoreOfDecoration<StateTransition<TState, TTrigger>>(cloneStore);
            StateMachineGraph<TState, TTrigger> returnValue = new StateMachineGraph<TState, TTrigger>(graph.InitialState, store);
            returnValue.SetCurrentState(graph.CurrentState);
            return returnValue;
        }
        #endregion
    }
}
