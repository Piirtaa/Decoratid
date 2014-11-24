using CuttingEdge.Conditions;
using Decoratid.Core.Contextual;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.Logical
{
    /// <summary>
    /// does some stuff with some context input and returns a result
    /// </summary>
    /// <typeparam name="TOf"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// 
    [Serializable]
    public sealed class LogicOfTo<TOf, TTo> : LogicBase, ILogicOf<TOf>, ILogicTo<TTo>
    {
        #region Ctor
        public LogicOfTo(Func<TOf, TTo> function)
            : base()
        {
            Condition.Requires(function).IsNotNull();
            this.Function = function;
        }
        public LogicOfTo(Func<TOf, TTo> function, TOf context)
            : base()
        {
            Condition.Requires(function).IsNotNull();
            this.Function = function;
            this.Context = context;
        }
        #endregion

        #region Static Methods
        public static LogicOfTo<TOf, TTo> New(Func<TOf, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicOfTo<TOf, TTo>(function);
        }
        #endregion

        #region ISerializable
        private LogicOfTo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Function = (Func<TOf, TTo>)info.GetValue("_Function", typeof(Func<TOf, TTo>));
            this.Result = (TTo)info.GetValue("_Result", typeof(TTo));
            this.Context = (TOf)info.GetValue("_Context", typeof(TOf));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Function", this.Function);
            info.AddValue("_Result", this.Result);
            info.AddValue("_Context", this.Context);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasContext
        public TOf Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { if (value != null) { this.Context = (TOf)value; } } }
        #endregion

        #region Properties
        internal Func<TOf, TTo> Function { get; set; }
        public TTo Result { get; private set; }
        #endregion

        #region ILogic
        protected override void perform(object context = null)
        {
            if (context != null)
            {
                this.Context = ((TOf)context);
            }
            this.Result = Function(this.Context);
        }
        #endregion

        #region ICloneableLogic
        public override ILogic Clone()
        {
            return new LogicOfTo<TOf, TTo>(this.Function, this.Context);
        }
        #endregion

    }

    public static class LogicOfToExtensions
    {
        public static Func<Targ, Tres> ToFunc<Targ, Tres>(this LogicOfTo<Targ, Tres> logic)
        {
            if (logic == null) { return null; }
            return logic.Function;
        }
        public static LogicOfTo<Targ, TRes> MakeLogicOfTo<Targ, TRes>(this Func<Targ, TRes> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicOfTo<Targ, TRes>(function);
        }
    }
}
