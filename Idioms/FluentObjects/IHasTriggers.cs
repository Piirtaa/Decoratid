using System;
using System.Collections.Generic;
using CuttingEdge.Conditions;
using Sandbox.Conditions;
using System.Linq;

namespace Sandbox.FluentObjects
{
    public interface IHasTriggers<TContext> : IHasBehaviour<TContext>
    {
        List<Trigger<TContext>> Triggers { get; }
    }

    /// <summary>
    /// defines condition and arguments to execute a behaviour
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class Trigger<TContext>
    {
        #region Properties
        public string BehaviourName { get; internal set; }

        /// <summary>
        /// when this condition is met, we will initiate the transition call
        /// </summary>
        public IConditionOf<TContext> TriggerCondition { get; private set; }

        /// <summary>
        /// A factory method that produces arguments to send to the transition function in a triggered transition
        /// </summary>
        public Func<TContext, object[]> TriggerArgumentFactory { get; private set; }
        #endregion

        #region Methods
        public Trigger<TContext> TriggeredWhen(IConditionOf<TContext> triggerCondition)
        {
            Condition.Requires(triggerCondition).IsNotNull();
            this.TriggerCondition = triggerCondition;
            return this;
        }
        public Trigger<TContext> TriggeredWithArgumentFactory(Func<TContext, object[]> argFactory)
        {
            Condition.Requires(argFactory).IsNotNull();
            this.TriggerArgumentFactory = argFactory;
            return this;
        }
        #endregion
    }

    public static class TriggersExtensions
    {
        internal static Trigger<TContext> WithTrigger<TContext>(this IHasTriggers<TContext> fluentObj, string name)
        {
            Trigger<TContext> t = null;
            t = fluentObj.Triggers.SingleOrDefault(x => x.BehaviourName == name);
            return t;
        }



    }
}
