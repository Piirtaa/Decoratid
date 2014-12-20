using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;
using Sandbox.Extensions;


namespace Sandbox.StateMachines
{
    /// <summary>
    /// statemachine with a specified context/statebag, and transition behaviours
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class FluentStateMachine<TContext> : StateMachineBase
        where TContext : new()
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// creates a statemachine with a default context, in the graph's default initial state
        /// </summary>
        /// <param name="graph"></param>
        public FluentStateMachine()
            : this(graph, null, new TContext())
        {
        }



        #endregion

        #region Properties
        public TContext Context { get; private set; }
        private List<FluentStateTransitionBehaviour<TContext>> Behaviours { get; set; }
        #endregion

        #region Fluent Building Methods
        public FluentStateTransitionBehaviour<TContext> RegisterTransition(string transitionName, string startState, params string[] endStates)
        {
            var trns = base.RegisterTransition(transitionName, startState, endStates);
            FluentStateTransitionBehaviour<TContext> behav = new FluentStateTransitionBehaviour<TContext>();
            behav.Transition = trns;
            return behav;
        }
        #endregion

        #region Fluent Methods
        /// <summary>
        /// Returns the Transition behaviour for a specific transition
        /// </summary>
        /// <param name="transitionName"></param>
        /// <param name="fromStateName"></param>
        /// <returns></returns>
        public FluentStateTransitionBehaviour<TContext> WithTransition(string transitionName, string fromStateName)
        {
            //could change this to use an equality test on trns itself
            var b = this.Behaviours.Find(x => x.Transition.FromStateName == fromStateName
                && x.Transition.TransitionName == transitionName);

            if (b == null)
                throw new InvalidOperationException("Cannot find transition");

            return b;
        }
        /// <summary>
        /// Returns the Transition behaviour for the only transition named.  If there is ambiguity or no transition
        /// an exception is raised
        /// </summary>
        /// <param name="transitionName"></param>
        /// <returns></returns>
        public FluentStateTransitionBehaviour<TContext> WithTransition(string transitionName)
        {
            //could change this to use an equality test on trns itself
            var b = this.Behaviours.FindAll(x => x.Transition.TransitionName == transitionName);

            if (b == null || b.Count > 1)
                throw new InvalidOperationException("Cannot find transition");

            return b.First();
        }
        #endregion

        #region Lookup  Methods
        /// <summary>
        /// look up behaviour by transition name and the current state
        /// </summary>
        /// <param name="transitionName"></param>
        /// <returns></returns>
        private FluentStateTransitionBehaviour<TContext> GetCurrentTransitionBehaviour(string transitionName)
        {
            if (!this.HasTransition(transitionName))
                throw new InvalidOperationException("Cannot find transition");

            //could change this to use an equality test on trns itself
            var b = this.Behaviours.Find(x => x.Transition.FromStateName == this.CurrentStateName
                && x.Transition.TransitionName == transitionName);

            if (b == null)
                throw new InvalidOperationException("Cannot find transition");
            return b;
        }
        #endregion

        #region State Machine Methods
        /// <summary>
        /// calls the transition function and updates state appropriately
        /// </summary>
        /// <remarks>
        /// if there is no explicit behaviour defined for this transition, we mark state forward
        /// if there is no explicit transition function for this transition, we mark state forward
        /// if the state change is to self, we ignore
        /// </remarks>
        public void DoTransition(string transitionName, params object[] args)
        {
            Condition.Requires(transitionName).IsNotNullOrEmpty();

            var beh = this.GetCurrentTransitionBehaviour(transitionName);
            string newState = beh.PerformTransition(this.Context, args);

            //validate/mark the state transition
            if (newState != this.CurrentStateName)
            {
                if (!this.MarkStateChange(newState))
                {
                    throw new InvalidOperationException(string.Format("Invalid state change returned.  {0}", newState));
                }
            }
        }

        /// <summary>
        /// Attempts to "automatically" call a transition method.
        /// If the trigger condition is true, Uses the TransitionFunctionArgumentFactory to build arguments, 
        /// and then calls the transition with those arguments
        /// </summary>
        /// <param name="machine"></param>
        public bool DoAutomaticTransition(string transitionName)
        {
            bool returnValue = false;

            Condition.Requires(transitionName).IsNotNullOrEmpty();
            var beh = this.GetCurrentTransitionBehaviour(transitionName);

            if (beh.IsTriggered(this.Context))
            {
                //we have a condition for a transition
                lock (this._stateLock)
                {
                    string newState = beh.PerformTriggeredTransition(this.Context);

                    //validate/mark the state transition
                    if (newState != this.CurrentStateName)
                    {
                        if (!this.MarkStateChange(newState))
                        {
                            throw new InvalidOperationException(string.Format("Invalid state change returned.  {0}", newState));
                        }
                    }
                    returnValue = true;
                }
            }
            return returnValue;
        }
        #endregion

        #region Event / Condition Check
        /// <summary>
        /// listens for a condition to be triggered (From our current state)
        /// </summary>
        public void CheckTriggers()
        {
            var trns = this.Graph.GetTransitionsFrom(this.CurrentStateName);

            foreach (var x in trns)
            {
                //if we succeed on a transition we stop the loop, because a new set of transitions is now in play
                if (this.DoAutomaticTransition(x.TransitionName))
                    break;
            }
        }
        #endregion







    }
}
