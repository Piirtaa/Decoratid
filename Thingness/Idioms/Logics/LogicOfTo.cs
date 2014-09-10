using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.ValuesOf;
using Decoratid.Extensions;
using Decoratid.Serialization;
using Decoratid.Thingness.Idioms.ObjectGraph;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;

namespace Decoratid.Thingness.Idioms.Logics
{
    /// <summary>
    /// does some stuff with some context input and returns a result
    /// </summary>
    /// <typeparam name="TOf"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// 
    [Serializable]
    public sealed class LogicOfTo<TOf, TTo> : ILogicOf<TOf>, ILogicTo<TTo>, ICloneableLogic,  IEquatable<LogicOfTo<TOf, TTo>>, ISerializable, IManagedHydrateable
    {
        #region Ctor
        public LogicOfTo(Func<TOf, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            this.Function = function;
        }
        public LogicOfTo(Func<TOf, TTo> function, IValueOf<TOf> context)
        {
            Condition.Requires(function).IsNotNull();
            this.Function = function;
            this.Context = context;
        }
        #endregion

        #region ISerializable
        private LogicOfTo(SerializationInfo info, StreamingContext context)
        {
            ////pull out added info
            //var data = info.GetString("_Function");
            //var bytes = Encoding.Unicode.GetBytes(data);

            //var formatter = new BinaryFormatter();
            //using (var stream = new MemoryStream(bytes))
            //{
            //    Func<TOf, TTo> obj = (Func<TOf, TTo>)formatter.Deserialize(stream);
            //    this.Function = obj;
            //}

            this.Function = (Func<TOf, TTo>)info.GetValue("_Function", typeof(Func<TOf, TTo>));
            this.Result = (TTo)info.GetValue("_Result", typeof(TTo));
            this.Context = (IValueOf<TOf>)info.GetValue("_Context", typeof(IValueOf<TOf>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ////serialize the strategy
            //var formatter = new BinaryFormatter();
            //using (var stream = new MemoryStream())
            //{
            //    formatter.Serialize(stream, this.Function);
            //    stream.Position = 0;
            //    var buffer = stream.GetBuffer();

            //    var data = Encoding.Unicode.GetString(buffer);
            //    info.AddValue("_Function", data);
            //}
            info.AddValue("_Function", this.Function);
            info.AddValue("_Result", this.Result);
            info.AddValue("_Context", this.Context);
        }
        #endregion

        #region IManagedHydrateable
        /// <summary>
        /// Do an implementation like this if you want to use .net's native serialization - delegating responsibility back to it.
        /// </summary>
        /// <returns></returns>
        string IManagedHydrateable.GetValueManagerId()
        {
            return SerializableValueManager.ID;
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
        public void Perform()
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
        public ILogic Clone()
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

        #region Equals Overrides
        public override bool Equals(object obj)
        {
            //if we're dealing with the same reference, we're good
            if (object.ReferenceEquals(this, obj))
                return true;

            return Equals(obj as LogicOfTo<TOf, TTo>);
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override string ToString()
        {
            SerializableValueManager mgr = new SerializableValueManager();
            var rv = mgr.DehydrateValue(this, null);
            return rv;
        }
        #endregion

        #region IEquatable
        public bool Equals(LogicOfTo<TOf, TTo> other)
        {
            if (other == null)
                return false;

            //if we're dealing with the same reference, we're good
            if (object.ReferenceEquals(this, other))
                return true;

            //seems hackish.  Since we're dealing with "logic", the only way to make sure it's the same is by binary comparison. eg.  so we need to serialize.  yes, it is expensive
            return this.ToString().Equals(other.ToString());
        }
        #endregion
    }
}
