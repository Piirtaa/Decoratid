using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core.ValueOfing.Decorations;

namespace Decoratid.Idioms.Core.ValueOfing
{
    public static class ValueOfExtensions
    {
        /// <summary>
        /// converts a thing into a ValueOf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static NaturalValueOf<T> AsNaturalValue<T>(this T thing)
        {
            if (thing == null)
                throw new ArgumentNullException();

            if (thing is NaturalValueOf<T>)
                return thing as NaturalValueOf<T>;

            //TODO: IDEA? if thing is already a ValueOf, to just return self? What "is" valueOf exactly other than a decoration?
            //but for right now, we'll see where this goes
            return new NaturalValueOf<T>(thing);
        }
        /// <summary>
        /// converts a thing factory into a ValueOf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static NaturalFactoriedValueOf<T> AsNaturalValueFactory<T>(this LogicTo<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            return new NaturalFactoriedValueOf<T>(factory);
        }


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
