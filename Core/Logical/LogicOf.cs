using CuttingEdge.Conditions;
using Decoratid.Core.Contextual;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.Logical
{
    /// <summary>
    /// does some stuff with context input
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class LogicOf<T> : LogicBase, ILogicOf<T>
    {
        #region Ctor
        public LogicOf(Action<T> action) : base()
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
        }
        public LogicOf(Action<T> action, T context)
            : base()
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
            this.Context = context;
        }
        #endregion

        #region Static Methods
        public static LogicOf<T> New(Action<T> action)
        {
            Condition.Requires(action).IsNotNull();
            return new LogicOf<T>(action);
        }
        #endregion

        #region ISerializable
        private LogicOf(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {       
            this.Action = (Action<T>)info.GetValue("_Action", typeof(Action<T>));
            this.Context = (T)info.GetValue("_Context", typeof(T)); 
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Action", this.Action);
            info.AddValue("_Context", this.Context); //let valueOf handle itself

            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasContext
        public T Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { if (value != null) { this.Context = (T)value; } } }
        #endregion

        #region Properties
        internal Action<T> Action { get; set; }
        #endregion

        #region ILogic
        protected override void perform(object context = null)
        {
            if (context != null)
            {
                this.Context = ((T)context);
            }
            Action(this.Context);
        }
        #endregion

        #region ICloneableLogic
        public override ILogic Clone()
        {
            var rv = new LogicOf<T>(this.Action, this.Context);
            return rv;
        }
        #endregion
    }

    public static class LogicOfExtensions
    {
        public static Action<T> ToAction<T>(this LogicOf<T> logic)
        {
            if (logic == null) { return null; }
            return logic.Action;
        }
        public static LogicOf<T> MakeLogicOf<T>(this Action<T> action)
        {
            Condition.Requires(action).IsNotNull();
            return new LogicOf<T>(action);
        }
    }
}
