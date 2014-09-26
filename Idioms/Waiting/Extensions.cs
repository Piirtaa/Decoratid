using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing.Decorations;

namespace Decoratid.Idioms.Waiting
{
    public static class ValueOfExtensions
    {


        /// <summary>
        /// appends a condition to the check condition on the valueof, such that the GetValue call will raise 
        /// an exception if the check condition is not true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="checkCondition"></param>
        /// <returns></returns>
        public static IConditionalValueOf<T> GivenTheCondition<T>(this IValueOf<T> valueOf, ICondition checkCondition)
        {
            Condition.Requires(valueOf).IsNotNull();
            Condition.Requires(checkCondition).IsNotNull();

            //if we already have condition triggers, just cast the task and return
            if (valueOf is IConditionalValueOf<T>)
            {
                var cVal = valueOf as IConditionalValueOf<T>;
                cVal.CheckCondition.And(checkCondition);
            }

            //wrap task
            return new ConditionalValueOfDecoration<T>(valueOf, checkCondition);
        }

        /// <summary>
        /// appends a condition to the check condition on the valueof.   uses a wait handle block
        /// returning the GetValue until the condition is true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="checkCondition"></param>
        /// <returns></returns>
        public static IConditionalValueOf<T> GivenTheWaitingCondition<T>(this IValueOf<T> valueOf, ICondition checkCondition)
        {
            Condition.Requires(valueOf).IsNotNull();
            Condition.Requires(checkCondition).IsNotNull();

            //if we already have condition triggers, just cast the task and return
            if (valueOf is IConditionalValueOf<T>)
            {
                var cVal = valueOf as IConditionalValueOf<T>;
                cVal.CheckCondition.And(checkCondition);
            }

            //wrap task
            return new WaitingConditionalValueOfDecoration<T>(valueOf, checkCondition);
        }
    }

}
