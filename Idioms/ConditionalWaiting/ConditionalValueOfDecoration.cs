using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Core.ValueOfing.Decorations
{
    /// <summary>
    /// interface defining behaviour such that the value won't be available until the condition is met.  
    /// Kacks if condition isn't true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConditionalValueOf<T> : IValueOfDecoration<T>
    {
        ICondition CheckCondition { get; }
    }

    /// <summary>
    /// the value won't be available until the condition is met, kacks on bad condition getvalue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ConditionalValueOfDecoration<T> : DecoratedValueOfBase<T>, IConditionalValueOf<T>
    {
        #region Ctor
        public ConditionalValueOfDecoration(IValueOf<T> decorated, ICondition checkCondition)
            : base(decorated)
        {
            Condition.Requires(checkCondition).IsNotNull();
            this.CheckCondition = checkCondition;
        }
        #endregion

        #region Fluent Static
        public static ConditionalValueOfDecoration<T> New(IValueOf<T> decorated, ICondition condition)
        {
            return new ConditionalValueOfDecoration<T>(decorated, condition);
        }
        #endregion

        #region ISerializable
        protected ConditionalValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.CheckCondition = (ICondition)info.GetValue("CheckCondition", typeof(ICondition));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CheckCondition", this.CheckCondition);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public ICondition CheckCondition { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            if (!CheckCondition.Evaluate().GetValueOrDefault())
                throw new InvalidOperationException("Condition not met");

            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ConditionalValueOfDecoration<T>(thing, this.CheckCondition);
        }
        #endregion
    }

    public static partial class Extensions
    {
        /// <summary>
        /// appends a condition to the check condition on the valueof, such that the GetValue call will raise 
        /// an exception if the check condition is not true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="checkCondition"></param>
        /// <returns></returns>
        public static IConditionalValueOf<T> IntolerantWhen<T>(this IValueOf<T> valueOf, ICondition checkCondition)
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
    }


}
