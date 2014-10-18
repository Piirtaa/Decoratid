using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Adjusting
{
    [Serializable]
    public class Adjustment<T> : IAdjustment<T>, ISerializable
    {
        #region Ctor
        public Adjustment(LogicOfTo<T, T> logic)
        {
            Condition.Requires(logic).IsNotNull();
            this.AdjustmentLogic = logic;
        }
        #endregion

        #region ISerializable
        protected Adjustment(SerializationInfo info, StreamingContext context)
        {
            this.AdjustmentLogic = (LogicOfTo<T, T>)info.GetValue("Adjustment", typeof(LogicOfTo<T, T>));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected virtual void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Adjustment", this.AdjustmentLogic);
        }
        #endregion

        #region IAdjustment
        public LogicOfTo<T, T> AdjustmentLogic { get; private set; }
        public T AdjustedValue { get { return this.AdjustmentLogic.Result; } }
        #endregion

    }
}
