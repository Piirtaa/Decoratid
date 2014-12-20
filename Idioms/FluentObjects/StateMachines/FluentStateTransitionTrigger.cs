using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Conditions;

namespace Sandbox.StateMachines
{
    [Serializable]
    public class FluentStateTransitionTrigger<TContext> 
    {
        #region Ctor
        public FluentStateTransitionTrigger()
        {

        }
        public FluentStateTransitionTrigger(IConditionOf<TContext> triggerCondition,
            Func<TContext, object[]> transitionFunctionArgumentFactory)
        {
            this.TriggerCondition = triggerCondition;
            this.TriggerArgumentFactory = transitionFunctionArgumentFactory;
        }
        #endregion

        #region Properties
        /// <summary>
        /// when this condition is met, we will initiate the transition call
        /// </summary>
        public IConditionOf<TContext> TriggerCondition { get; set; }

        /// <summary>
        /// A factory method that produces arguments to send to the transition function in a triggered transition
        /// </summary>
        public Func<TContext, object[]> TriggerArgumentFactory { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// tests the condition.  if there is no condition, then returns false
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsTriggered(TContext context)
        {
            bool returnValue = false;

            if (this.TriggerCondition == null)
                return false;

            returnValue = this.TriggerCondition.IsTrue();

            return returnValue;
        }
        /// <summary>
        /// performs the transition function "automatically", with arguments produced by the argument factory if there is one.
        /// If there is no factory, null args are used.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string PerformTriggeredTransition(TContext context, FluentStateTransitionBehaviour<TContext> behav)
        {

            if (this.TriggerArgumentFactory == null)
            {
                return behav.PerformTransition(context);
            }
            else
            {
                var args = this.TriggerArgumentFactory.Invoke(context);
                return behav.PerformTransition(context, args);
            }
        }
        #endregion
    }
}
