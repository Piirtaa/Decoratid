using CuttingEdge.Conditions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.Logical
{
    /// <summary>
    /// does some stuff
    /// </summary>
    [Serializable]
    public sealed class Logic : LogicBase
    {
        #region Ctor
        public Logic(Action action)
            : base()
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
        }
        #endregion

        #region Static Methods
        public static Logic New(Action action)
        {
            Condition.Requires(action).IsNotNull();
            return new Logic(action);
        }
        #endregion

        #region ISerializable
        private Logic(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Action = (Action)info.GetValue("_Action", typeof(Action));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Action", this.Action);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        internal Action Action { get; set; }
        #endregion

        #region ILogic
        protected override void perform(object context = null)
        {
            Action();
        }
        #endregion

        #region ICloneableLogic
        public override ILogic Clone()
        {
            return new Logic(this.Action);
        }
        #endregion
    }

    public static class LogicExtensions
    {
        public static Action ToAction(this Logic logic)
        {
            if (logic == null) { return null; }
            return logic.Action;
        }

        public static Logic MakeLogic(this Action action)
        {
            Condition.Requires(action).IsNotNull();
            return new Logic(action);
        }
    }

}
