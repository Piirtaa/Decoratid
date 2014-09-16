using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Idioms.Core.ValueOfing.Decorations
{

    /// <summary>
    /// the value won't be available until the condition is met, and we block on a wait handle until the 
    /// polling action tells us the condition is true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WaitingConditionalValueOfDecoration<T> : DecoratedValueOfBase<T>, IConditionalValueOf<T>, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        private BackgroundHost _background;
        #endregion

        #region Ctor
        protected WaitingConditionalValueOfDecoration() : base() { }
        public WaitingConditionalValueOfDecoration(IValueOf<T> decorated, ICondition checkCondition)
            : base(decorated)
        {
            Condition.Requires(checkCondition).IsNotNull();
            this.CheckCondition = checkCondition;
            this._background = new BackgroundHost(true, 1000,
                Logic.New(() =>
            {
                lock (_stateLock)
                {
                    if (this.CheckCondition.Evaluate().GetValueOrDefault())
                        Monitor.Pulse(_stateLock);
                }
            }));
        }
        #endregion

        #region Properties
        public ICondition CheckCondition { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            lock (_stateLock)
                while (!CheckCondition.Evaluate().GetValueOrDefault())
                    Monitor.Wait(_stateLock);

            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new WaitingConditionalValueOfDecoration<T>(thing, this.CheckCondition);
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<WaitingConditionalValueOfDecoration<T>>();
            hydrationMap.RegisterDefault("CheckCondition", x => x.CheckCondition, (x, y) => { x.CheckCondition = y as ICondition; });
            hydrationMap.RegisterDefault("_background", x => x._background, (x, y) => { x._background = y as BackgroundHost; });
            
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
