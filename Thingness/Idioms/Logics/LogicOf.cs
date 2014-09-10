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
using Decoratid.Thingness.Idioms.ValuesOf;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Thingness.Idioms.Logics
{
    /// <summary>
    /// does some stuff with context input
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class LogicOf<T> : ILogic, IHasContext<IValueOf<T>>, ILogicOf<T>, ICloneableLogic, IEquatable<LogicOf<T>>, ISerializable, IManagedHydrateable
    {
        #region Ctor
        public LogicOf(Action<T> action)
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
        }
        public LogicOf(Action<T> action, IValueOf<T> context)
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
            this.Context = context;
        }
        #endregion

        #region ISerializable
        private LogicOf(SerializationInfo info, StreamingContext context)
        {       
            this.Action = (Action<T>)info.GetValue("_Action", typeof(Action<T>));
            this.Context = (IValueOf<T>)info.GetValue("_Context", typeof(IValueOf<T>)); 

        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Action", this.Action);
            info.AddValue("_Context", this.Context); //let valueOf handle itself
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
        public IValueOf<T> Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { this.Context = (IValueOf<T>)value; } }
        #endregion

        #region Properties
        private Action<T> Action { get; set; }
        #endregion

        #region ILogic
        public void Perform()
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
        public ILogic Clone()
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

        #region Equals Overrides
        public override bool Equals(object obj)
        {
            //if we're dealing with the same reference, we're good
            if (object.ReferenceEquals(this, obj))
                return true;

            return Equals(obj as LogicOf<T>);
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
        public bool Equals(LogicOf<T> other)
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
