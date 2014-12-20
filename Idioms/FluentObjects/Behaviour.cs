using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;

namespace Sandbox.FluentObjects
{
    public interface IBehaviour
    {
        string Name { get;  }
    }

    /// <summary>
    /// Describes a "behaviour".  Something that has a context and takes an argument, producing a result.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class Behaviour<TContext, TArg, TResult> : IBehaviour
    {
        #region Ctor
        public Behaviour(string name)
        {
            Condition.Requires(name).IsNotNullOrEmpty();
            this.Name = name;
        }
        #endregion

        #region Properties
        public string Name { get; private set; }
        private Func<TContext, TArg, TResult> BehaviourFunction { get; set; }
        private Func<TContext, ICondition> TriggerConditionFactory { get; set; }
        private Func<TContext, TArg> TriggerArgumentFactory { get; set; }
        private Func<TContext, TArg, bool> MessageHandlingFilter { get; set; }
        private bool EnqueuesResultAsSystemMessage { get; set; }
        #endregion

        #region Derived Properties

        #endregion

        #region Fluent Setup Methods
        /// <summary>
        /// Fluent.  Sets the behaviour function.
        /// </summary>
        /// <param name="behaviourFunction"></param>
        /// <returns></returns>
        public Behaviour<TContext, TArg, TResult> Does(Func<TContext, TArg, TResult> behaviourFunction)
        {
            this.BehaviourFunction = behaviourFunction;
            return this;
        }
        /// <summary>
        /// Fluent.  Sets the trigger to run the behaviour, and the behaviour's argument factory
        /// </summary>
        /// <param name="conditionFactory"></param>
        /// <returns></returns>
        public Behaviour<TContext, TArg, TResult> TriggeredWhen(Func<TContext, ICondition> conditionFactory,
            Func<TContext, TArg> argFactory)
        {
            this.TriggerConditionFactory = conditionFactory;
            this.TriggerArgumentFactory = argFactory;
            return this;
        }

        /// <summary>
        /// Fluent.  Sets whether this behaviour handles a TArg bus message, that returns a TRes response.
        /// If the filter is null, no messages are handled.  If the filter returns true, the message is handled.
        /// </summary>
        /// <param name="handleFilter"></param>
        /// <returns></returns>
        public Behaviour<TContext, TArg, TResult> HandlesMessage(Func<TContext, TArg, bool> handleFilter)
        {
            this.MessageHandlingFilter = handleFilter;
            return this;
        }
        #endregion

        #region Instance Methods
        public TResult DoBehaviour(TContext context, TArg arg)
        {
            return this.BehaviourFunction.Invoke(context, arg);
        }
        public bool IsTriggered(TContext context)
        {
            if (this.TriggerConditionFactory == null) { return false; }
            return this.TriggerConditionFactory(context).IsTrue();
        }
        public TResult DoTriggeredBehaviour(TContext context)
        {
            Condition.Requires(this.TriggerArgumentFactory).IsNotNull();
            var arg = this.TriggerArgumentFactory(context);
            return this.DoBehaviour(context, arg);
        }
        /// <summary>
        /// can this behaviour handle the specified message types.  
        /// returns false if no message filter exists
        /// returns false if the provided types are not assignable to the TArg and TResult types
        /// returns true otherwise
        /// </summary>
        /// <param name="argType"></param>
        /// <param name="respType"></param>
        /// <returns></returns>
        public bool CanHandleMessagesOf(Type argType, Type respType)
        {
            if (this.MessageHandlingFilter == null) { return false; }

            if (typeof(TArg).IsAssignableFrom(argType) &&
                typeof(TResult).IsAssignableFrom(respType))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// can this behaviour handle the specified arg
        /// </summary>
        /// <param name="context"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool CanHandleMessage(TContext context, TArg arg)
        {
            if (this.MessageHandlingFilter == null) { return false; }
            return this.MessageHandlingFilter(context, arg);
        }
        #endregion
    }
}
