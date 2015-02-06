using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Decoratid.Extensions;
using Decoratid.Core.Logical;

namespace Decoratid.Idioms.StateMachining
{
    //exactly the same as the generic version, but uses string

    /// <summary>
    /// defines a state transition (as an item in a store)
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TTrigger"></typeparam>
    [Serializable]
    public class StringStateTransition : IHasId<string>
    {
        #region Ctor
        public StringStateTransition(string fromState, string toState, string transitionTrigger)
        {
            this.FromState = fromState;
            this.ToState = toState;
            this.TransitionTrigger = transitionTrigger;
        }
        #endregion

        #region Properties
        public string FromState { get; private set; }
        public string ToState { get; private set; }
        public string TransitionTrigger { get; private set; }
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
    public class StringStateMachineGraph : ISerializable
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StringStateMachineGraph(string initialState)
        {
            if (initialState == null) { throw new ArgumentNullException("initialState"); }
            this.InitialState = initialState;
            this.CurrentState = initialState;

            this.Store = NaturalInMemoryStore.New().IsOf<StringStateTransition>();
        }
        public StringStateMachineGraph(string initialState, IStoreOf<StringStateTransition> store)
        {
            if (initialState == null) { throw new ArgumentNullException("initialState"); }
            this.InitialState = initialState;
            this.CurrentState = initialState;

            Condition.Requires(store).IsNotNull();
            this.Store = store;
        }
        #endregion

        #region ISerializable
        protected StringStateMachineGraph(SerializationInfo info, StreamingContext context)
        {
            this.InitialState = info.GetString("InitialState");
            this.CurrentState = info.GetString("CurrentState");

            List<StringStateTransition> list = (List<StringStateTransition>)info.GetValue("list", typeof(List<StringStateTransition>));
            this.Store = NaturalInMemoryStore.New().IsOf<StringStateTransition>();
            var newList = list.ConvertListTo<IHasId, StringStateTransition>();
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
        public string InitialState { get; private set; }
        private IStoreOf<StringStateTransition> Store { get; set; }
        public string CurrentState { get; private set; }
        #endregion

        #region Registration Methods
        public void AllowTransition(string fromState, string toState, string trigger)
        {
            lock (this._stateLock)
            {
                StringStateTransition trans = new StringStateTransition(fromState, toState, trigger);
                this.Store.SaveItem(trans);
            }
        }
        #endregion

        #region Methods
        public void SetCurrentState(string state)
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
        public bool CanTrigger(string trigger)
        {
            //does this transition from the current state exist?
            LogicOfTo<StringStateTransition, bool> filter = LogicOfTo<StringStateTransition, bool>.New((x) =>
            {
                return x.FromState.Equals(this.CurrentState) && x.TransitionTrigger.Equals(trigger);
            });

            var list = this.Store.SearchOf<StringStateTransition>(filter);
            return list != null && list.Count > 0;
        }

        /// <summary>
        /// returns true if the trigger changes state
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public bool Trigger(string trigger)
        {
            lock (this._stateLock)
            {
                //does this transition from the current state exist?
                LogicOfTo<StringStateTransition, bool> filter = LogicOfTo<StringStateTransition, bool>.New((x) =>
                {
                    return x.FromState.Equals(this.CurrentState) && x.TransitionTrigger.Equals(trigger);
                });

                var list = this.Store.SearchOf<StringStateTransition>(filter);

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
        public static StringStateMachineGraph Clone(StringStateMachineGraph graph)
        {
            if (graph == null) { return null; }
            StringStateMachineGraph returnValue = new StringStateMachineGraph(graph.InitialState, graph.Store);
            returnValue.SetCurrentState(graph.CurrentState);
            return returnValue;
        }
        public static StringStateMachineGraph New(string initialState)
        {
            return new StringStateMachineGraph(initialState);
        }
        public static StringStateMachineGraph New(string initialState, IStoreOf<StringStateTransition> store)
        {
            return new StringStateMachineGraph(initialState, store);
        }
        #endregion
    }
}
