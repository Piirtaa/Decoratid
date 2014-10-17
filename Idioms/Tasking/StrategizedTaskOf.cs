using CuttingEdge.Conditions;
using Decoratid.Core.Contextual;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Idioms.Tasking.Core
{
    /// <summary>
    /// uses contextual ILogic strategies to implement a task
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StrategizedTaskOf<T> : TaskBase, IHasContext<IValueOf<T>>
    {
        #region Ctor
        private StrategizedTaskOf(string id, IValueOf<T> context, ILogicOf<T> performLogic, ILogicOf<T> cancelLogic = null)
            : base(id)
        {
            Condition.Requires(performLogic).IsNotNull();
            this.PerformLogic = performLogic;
            this.CancelLogic = cancelLogic;
            this.Context = context;
        }
        #endregion

        #region IHasContext
        public IValueOf<T> Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { this.Context = (IValueOf<T>)value; } }
        #endregion

        #region Properties
        public ILogicOf<T> PerformLogic { get; set; }
        public ILogicOf<T> CancelLogic { get; set; }
        #endregion

        #region Overrides
        /// <summary>
        /// unless we have an async decoration, we presume the perform is synchronous and upon successful completion we markcomplete
        /// </summary>
        /// <returns></returns>
        public override bool perform()
        {
            this.PerformLogic.Context = this.Context;
            this.PerformLogic.Perform();
            //unless we have an async decoration, we presume the perform is synchronous and upon successful completion we markcomplete
            this.MarkComplete();
            return true;
        }
        public override bool cancel()
        {
            if (this.CancelLogic == null)
                return true;

            this.CancelLogic.Context = this.Context;
            this.CancelLogic.Perform();
            return true;
        }
        #endregion

        #region Fluent Static Methods
        public static ITask New<Targ>(string id, IValueOf<Targ> context, ILogicOf<Targ> performLogic, ILogicOf<Targ> cancelLogic = null)
        {
            var rv = new StrategizedTaskOf<Targ>(id,context ,performLogic, cancelLogic );

            ////now some fancy stuff...
            ////if the context is IConditionalValueOf<T>, it means there's a condition that prevents the getting of the context value
            ////apply a conditional constraint to the task's Perform indicating this condition must be true
            //if (context is IConditionalValueOf<Targ>)
            //{
            //    IConditionalValueOf<Targ> cContext = context as IConditionalValueOf<Targ>;
            //    var rTask = rv.ANDPerformCondition(cContext.CheckCondition);
            //    return rTask;
            //}

            return rv;
        }
        /// <summary>
        /// creates a strategized task with strategies that don't do anything.  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ITask NewBlank<Targ>(string id)
        {
            return new StrategizedTaskOf<Targ>(id,null, LogicOf<Targ>.New((x) => { }));
        }
        #endregion
    }
}
