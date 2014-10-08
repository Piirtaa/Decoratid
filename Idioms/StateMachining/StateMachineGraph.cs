using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Decoratid.Extensions;

namespace Decoratid.Idioms.StateMachining
{
    /// <summary>
    /// defines a state transition (as an item in a store)
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TTrigger"></typeparam>
    [Serializable]
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
    public class StateMachineGraph<TState, TTrigger> : ISerializable
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

            this.Store = NaturalInMemoryStore.New().IsOf<StateTransition<TState, TTrigger>>();
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

        #region ISerializable
        protected StateMachineGraph(SerializationInfo info, StreamingContext context)
        {
            this.InitialState = (TState)info.GetValue("InitialState", typeof(TState));
            this.CurrentState = (TState)info.GetValue("CurrentState", typeof(TState));

            List<StateTransition<TState, TTrigger>> list = (List<StateTransition<TState, TTrigger>>)info.GetValue("list", typeof(List<StateTransition<TState, TTrigger>>));
            this.Store = NaturalInMemoryStore.New().IsOf<StateTransition<TState, TTrigger>>();
            var newList = list.ConvertListTo<IHasId, StateTransition<TState, TTrigger>>();
            this.Store.SaveItems(newList);
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected virtual void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InitialState", this.InitialState);
            info.AddValue("CurrentState", this.CurrentState);

            var list = this.Store.GetAll();
            info.AddValue("list", list);
        }
        #endregion

        #region Properties
        public TState InitialState { get; private set; }
        private IStoreOf<StateTransition<TState, TTrigger>> Store { get; set; }
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
            SearchFilterOf<StateTransition<TState, TTrigger>> filter = new SearchFilterOf<StateTransition<TState, TTrigger>>((x) =>
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
            StateMachineGraph<TState, TTrigger> returnValue = new StateMachineGraph<TState, TTrigger>(graph.InitialState, graph.Store);
            returnValue.SetCurrentState(graph.CurrentState);
            return returnValue;
        }
        public static StateMachineGraph<TState, TTrigger> New(TState initialState)
        {
            return new StateMachineGraph<TState, TTrigger>(initialState);
        }
        public static StateMachineGraph<TState, TTrigger> New(TState initialState, IStoreOf<StateTransition<TState, TTrigger>> store)
        {
            return new StateMachineGraph<TState, TTrigger>(initialState, store);
        }
        #endregion
    }
}
