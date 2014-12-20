using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;
using Sandbox.StateMachines.Graph;
using Sandbox.Thingness;

namespace Sandbox.StateMachines
{
    /// <summary>
    /// the "genericized" signature of all transition methods
    /// </summary>
    /// <param name="context"></param>
    /// <param name="args"></param>
    /// <returns>the new state we've transitioned to (prior to mark transition validation)</returns>
    public delegate string TransitionFunctionDelegate<TContext>(TContext context, params object[] args);

    [Serializable]
    /// <summary>
    /// This defines the behaviour around a state transition, such as the transition function, the conditions for an automatic transition
    /// , and the automatic transition argument factory
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class FluentStateTransitionBehaviour<TContext> 
    {
        #region Ctor
        public FluentStateTransitionBehaviour()
        {

        }

        #endregion

        #region Properties
        /// <summary>
        /// This is the transition that's defined in a state transition graph
        /// </summary>
        public StateTransition Transition { get; set; }

        /// <summary>
        /// the transition function
        /// </summary>
        public Delegate TransitionFunction { get; set; }

        public FluentStateTransitionTrigger<TContext> Trigger { get; set; }
        #endregion

        #region Fluent Methods to define the behaviour
        public FluentStateTransitionBehaviour<TContext> Does(Delegate transitionLogic)
        {
            Condition.Requires(transitionLogic).IsNotNull();
            this.TransitionFunction = transitionLogic;
            return this;
        }
        public FluentStateTransitionBehaviour<TContext> TriggeredWhen(IConditionOf<TContext> triggerCondition, Func<TContext, object[]> argFactory)
        {
            Condition.Requires(triggerCondition).IsNotNull();
            Condition.Requires(argFactory).IsNotNull();   
         
            FluentStateTransitionTrigger<TContext> trigger = new FluentStateTransitionTrigger<TContext>(triggerCondition, argFactory);
            this.Trigger = trigger;
            return this;
        }
        #endregion

        #region Transition Methods
        /// <summary>
        /// performs the transition function and returns the new state name
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public string PerformTransition(params object[] args)
        {
            //if the transition function doesn't exist, assume no behaviour, and return the expected state
            if (this.TransitionFunction == null)
            {
                return this.Transition.ToStateNames.First();
            }
            else
            {
                var res =  this.TransitionFunction.DynamicInvoke(args);
                if (res == null) { return null; }
                return res.ToString();
            }
        }
        #endregion
    }


}
