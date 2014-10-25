using CuttingEdge.Conditions;
using Decoratid.Core.Logical;

namespace Decoratid.Idioms.Tasking
{
    /// <summary>
    /// uses ILogic strategies to implement a task
    /// </summary>
    public class StrategizedTask : TaskBase
    {
        #region Ctor

        private StrategizedTask(string id, ILogic performLogic, ILogic cancelLogic = null)
            : base(id)
        {
            Condition.Requires(performLogic).IsNotNull();
            this.PerformLogic = performLogic;
            this.CancelLogic = cancelLogic;
        }
        #endregion

        #region Properties
        public ILogic PerformLogic { get; set; }
        public ILogic CancelLogic { get; set; }
        #endregion

        #region Overrides

        public override bool perform()
        {
            this.PerformLogic.Perform();

            return true;
        }
        public override bool cancel()
        {
            if (this.CancelLogic == null)
                return true;

            this.CancelLogic.Perform();
            return true;
        }
        #endregion

        #region Fluent Methods
        /// <summary>
        /// fluently sets the perform logic
        /// </summary>
        /// <param name="logic"></param>
        /// <returns></returns>
        public StrategizedTask Performs(ILogic logic)
        {
            Condition.Requires(logic).IsNotNull();
            this.PerformLogic = logic;  
            return this;
        }
        /// <summary>
        /// fluently sets the cancel logic
        /// </summary>
        /// <param name="logic"></param>
        /// <returns></returns>
        public StrategizedTask Cancels(ILogic logic)
        {
            Condition.Requires(logic).IsNotNull();
            this.PerformLogic = logic;
            return this;
        }
        #endregion

        #region Static Fluent Methods
        public static StrategizedTask New(string id, ILogic performLogic, ILogic cancelLogic = null)
        {
            return new StrategizedTask(id, performLogic, cancelLogic);
        }
        /// <summary>
        /// creates a strategized task with strategies that don't do anything.  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StrategizedTask NewBlank(string id)
        {
            return new StrategizedTask(id, Logic.New(() => { }));
        }
        #endregion
    }
}
