using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Serialization;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;

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
        public LogicOfTo(Func<TOf, TTo> function, IValueOf<TOf> context)
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
        public static LogicOfTo<TOf, TTo> New(Func<TOf, TTo> function, IValueOf<TOf> context)
        {
            Condition.Requires(function).IsNotNull();
            Condition.Requires(context).IsNotNull();
            return new LogicOfTo<TOf, TTo>(function, context);
        }
        #endregion

        #region ISerializable
        protected LogicOfTo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Function = (Func<TOf, TTo>)info.GetValue("_Function", typeof(Func<TOf, TTo>));
            this.Result = (TTo)info.GetValue("_Result", typeof(TTo));
            this.Context = (IValueOf<TOf>)info.GetValue("_Context", typeof(IValueOf<TOf>));
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
        public IValueOf<TOf> Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { this.Context = (IValueOf<TOf>)value; } }
        #endregion

        #region Properties
        private Func<TOf, TTo> Function { get; set; }
        public TTo Result { get; private set; }
        #endregion

        #region ILogic
        public override void Perform()
        {
            Condition.Requires(this.Context).IsNotNull();

            var arg = this.Context.GetValue();
            this.Result = Function(arg);
        }
        #endregion

        #region ILogicOf
        public void SetContextAndPerform(IValueOf<TOf> value)
        {
            this.Context = value;
            this.Perform();
        }
        #endregion

        #region ICloneableLogic
        public override ILogic Clone()
        {
            return new LogicOfTo<TOf, TTo>(this.Function, this.Context);
        }
        #endregion

        #region Clone and Run
        public TTo CloneAndPerform(IValueOf<TOf> arg)
        {
            LogicOfTo<TOf, TTo> clone = (LogicOfTo<TOf, TTo>)this.Clone();
            clone.Context = arg;
            clone.Perform();
            return clone.Result;
        }
        #endregion

    }

    public static class LogicOfToExtensions
    {
        public static Func<Targ, Tres> ToFunc<Targ, Tres>(this LogicOfTo<Targ, Tres> logic)
        {
            if (logic == null) { return null; }

            return (x) =>
            {
                Tres res = logic.CloneAndPerform(x.AsNaturalValue());

                return res;
            };
        }
        public static LogicOfTo<Targ, TRes> MakeLogicOfTo<Targ, TRes>(this Func<Targ, TRes> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicOfTo<Targ, TRes>(function);
        }
        public static LogicOfTo<Targ, TRes> MakeLogicOfTo<Targ, TRes>(this Func<Targ, TRes> function, IValueOf<Targ> context)
        {
            Condition.Requires(function).IsNotNull();
            Condition.Requires(context).IsNotNull();
            return new LogicOfTo<Targ, TRes>(function, context);
        }
    }
}
