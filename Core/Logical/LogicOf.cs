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
        public LogicOf(Action<T> action, IValueOf<T> context) : base()
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
        public static LogicOf<T> New(Action<T> action, IValueOf<T> context)
        {
            Condition.Requires(action).IsNotNull();
            Condition.Requires(context).IsNotNull();
            return new LogicOf<T>(action, context);
        }
        #endregion

        #region ISerializable
        protected LogicOf(SerializationInfo info, StreamingContext context) : base(info, context)
        {       
            this.Action = (Action<T>)info.GetValue("_Action", typeof(Action<T>));
            this.Context = (IValueOf<T>)info.GetValue("_Context", typeof(IValueOf<T>)); 
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Action", this.Action);
            info.AddValue("_Context", this.Context); //let valueOf handle itself

            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasContext
        public IValueOf<T> Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { this.Context = (IValueOf<T>)value; } }
        #endregion

        #region Properties
        private Action<T> Action { get; set; }
        #endregion

        #region ILogic
        public override void Perform()
        {
            Condition.Requires(this.Context).IsNotNull();
            var arg = this.Context.GetValue();
            Action(arg);
        }
        #endregion

        #region ILogicOf
        public void SetContextAndPerform(IValueOf<T> value)
        {
            this.Context = value;
            this.Perform();
        }
        #endregion

        #region ICloneableLogic
        public override ILogic Clone()
        {
            return new LogicOf<T>(this.Action, this.Context);
        }
        #endregion

        #region Clone and Run
        public void CloneAndPerform(IValueOf<T> arg)
        {
            LogicOf<T> clone = (LogicOf<T>)this.Clone();
            clone.Context = arg;
            clone.Perform();
        }
        #endregion
    }

    public static class LogicOfExtensions
    {
        public static Action<T> ToAction<T>(this LogicOf<T> logic)
        {
            if (logic == null) { return null; }

            return (x) => { logic.CloneAndPerform(x.AsNaturalValue()); };
        }
        public static LogicOf<T> MakeLogicOf<T>(this Action<T> action)
        {
            Condition.Requires(action).IsNotNull();
            return new LogicOf<T>(action);
        }
        public static LogicOf<T> MakeLogicOf<T>(this Action<T> action, IValueOf<T> context)
        {
            Condition.Requires(action).IsNotNull();
            Condition.Requires(context).IsNotNull();
            return new LogicOf<T>(action, context);
        }
    }
}
