using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Core.Conditional
{
    public static partial class ConditionBuilder
    {
        /// <summary>
        /// builds a strategized condition
        /// </summary>
        /// <param name="conditionStrategy"></param>
        /// <returns></returns>
        public static StrategizedCondition New(Func<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedCondition(conditionStrategy);
        }
        /// <summary>
        /// builds a strategized conditionOf
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="conditionStrategy"></param>
        /// <returns></returns>
        public static StrategizedConditionOf<TArg> New<TArg>(Func<TArg, bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedConditionOf<TArg>(conditionStrategy);
        }
        /// <summary>
        /// builds a mutable strategized condition
        /// </summary>
        /// <param name="conditionStrategy"></param>
        /// <param name="mutateStrategy"></param>
        /// <returns></returns>
        public static MutableStrategizedCondition NewMutable(Func<bool?> conditionStrategy, Action mutateStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new MutableStrategizedCondition(conditionStrategy, mutateStrategy);
        }
        /// <summary>
        /// builds a contextual condition (eg. converts conditionOf to condition by providing context state)
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="context"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ContextualCondition<TArg> NewContextual<TArg>(IValueOf<TArg> context, IConditionOf<TArg> condition)
        {
            return new ContextualCondition<TArg>(context, condition);
        }
        /// <summary>
        /// builds a mutable contextual condition 
        /// </summary>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="context"></param>
        /// <param name="condition"></param>
        /// <param name="mutateStrategy"></param>
        /// <returns></returns>
        public static MutableContextualCondition<TArg> NewMutableContextual<TArg>(IValueOf<TArg> context, IConditionOf<TArg> condition, Func<TArg, TArg> mutateStrategy)
        {
            return new MutableContextualCondition<TArg>(context, condition, mutateStrategy);
        }

    }
}
