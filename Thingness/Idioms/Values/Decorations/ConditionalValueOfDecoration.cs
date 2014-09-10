using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Thingness.Idioms.ValuesOf.Decorations
{
    /// <summary>
    /// interface defining behaviour such that the value won't be available until the condition is met
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
    public class ConditionalValueOfDecoration<T> : DecoratedValueOfBase<T>, IConditionalValueOf<T>, IHasHydrationMap
    {
        #region Ctor
        protected ConditionalValueOfDecoration() : base() { }
        public ConditionalValueOfDecoration(IValueOf<T> decorated, ICondition checkCondition)
            : base(decorated)
        {
            Condition.Requires(checkCondition).IsNotNull();
            this.CheckCondition = checkCondition;
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


        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<ConditionalValueOfDecoration<T>>();
            hydrationMap.RegisterDefault("CheckCondition", x => x.CheckCondition, (x, y) => { x.CheckCondition = y as ICondition; });
            return hydrationMap;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.GetHydrationMap().DehydrateValue(this, uow);
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            this.GetHydrationMap().HydrateValue(this, text, uow);
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
    }


}
