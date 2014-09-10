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
using Decoratid.Thingness.Idioms.ObjectGraph;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;

namespace Decoratid.Thingness.Idioms.Logics
{
    /// <summary>
    /// does some stuff and outputs a result
    /// </summary>
    /// 
    [Serializable]
    public sealed class LogicTo<T> : ILogic, ILogicTo<T>, ICloneableLogic,  IEquatable<LogicTo<T>>,  ISerializable, IManagedHydrateable
    {
        #region Ctor
        public LogicTo(Func<T> function)
        {
            Condition.Requires(function).IsNotNull();
            this.Function = function;
        }
        #endregion

        #region ISerializable
        private LogicTo(SerializationInfo info, StreamingContext context)
        {
            ////pull out added info
            //var data = info.GetString("_Function");
            //var bytes = Encoding.Unicode.GetBytes(data);

            //var formatter = new BinaryFormatter();
            //using (var stream = new MemoryStream(bytes))
            //{
            //    Func<T> obj = (Func<T>)formatter.Deserialize(stream);
            //    this.Function = obj;
            //}
            this.Function = (Func<T>)info.GetValue("_Function", typeof(Func<T>));
            this.Result = (T)info.GetValue("_Result", typeof(T));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //serialize the strategy
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

        #region Properties
        private Func<T> Function { get; set; }
        public T Result { get; private set; }
        #endregion

        #region ITaskLogic
        public void Perform()
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
        public ILogic Clone()
        {
            return new LogicTo<T>(this.Function);
        }
        #endregion

        #region Static Methods
        public static LogicTo<T> New(Func<T> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicTo<T>(function);
        }
        #endregion

        #region Equals Overrides
        public override bool Equals(object obj)
        {
            //if we're dealing with the same reference, we're good
            if (object.ReferenceEquals(this, obj))
                return true;

            return Equals(obj as LogicTo<T>);
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
        public bool Equals(LogicTo<T> other)
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
