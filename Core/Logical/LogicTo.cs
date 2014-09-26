using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Serialization;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;

namespace Decoratid.Core.Logical
{
    /// <summary>
    /// does some stuff and outputs a result
    /// </summary>
    [Serializable]
    public sealed class LogicTo<T> : LogicBase, ILogicTo<T>
    {
        #region Ctor
        public LogicTo(Func<T> function)
            : base()
        {
            Condition.Requires(function).IsNotNull();
            this.Function = function;
        }
        #endregion

        #region Static Methods
        public static LogicTo<T> New(Func<T> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicTo<T>(function);
        }
        #endregion

        #region ISerializable
        protected LogicTo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Function = (Func<T>)info.GetValue("_Function", typeof(Func<T>));
            this.Result = (T)info.GetValue("_Result", typeof(T));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Function", this.Function);
            info.AddValue("_Result", this.Result);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        private Func<T> Function { get; set; }
        public T Result { get; private set; }
        #endregion

        #region ITaskLogic
        public override void Perform()
        {
            this.Result = Function();
        }
        #endregion

        #region Clone and Run
        public T CloneAndPerform()
        {
            LogicTo<T> clone = (LogicTo<T>)this.Clone();
            clone.Perform();
            return clone.Result;
        }
        #endregion

        #region ICloneableLogic
        public override ILogic Clone()
        {
            return new LogicTo<T>(this.Function);
        }
        #endregion
    }

    public static class LogicToExtensions
    {
        public static Func<T> ToFunc<T>(this LogicTo<T> logic)
        {
            if (logic == null) { return null; }

            return () => { return logic.CloneAndPerform(); };
        }
        public static LogicTo<T> MakeLogicTo<T>(this Func<T> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicTo<T>(function);
        }
    }
}
