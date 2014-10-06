using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.ValueOfing
{
    /// <typeparam name="T"></typeparam>
    public interface IModifyingValueOf<T>: IValueOf<T>
    {
        LogicOfTo<T,T> ModificationLogic {get;}
    }

    /// <summary>
    /// use a factory to get a value.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
   /// </remarks>
    [Serializable]
    public sealed class ModifyingValueOf<T> : IModifyingValueOf<T>, ISerializable
    {
        #region Ctor
        public ModifyingValueOf(LogicOfTo<T,T> factory) 
        {
            Condition.Requires(factory).IsNotNull();
            this.Factory = factory;
        }
        #endregion

        #region Static Methods
        public static ModifyingValueOf<T> New(LogicOfTo<T, T> factory)
        {
            return new ModifyingValueOf<T>(factory);
        }
        #endregion
        
        #region ISerializable
        private ModifyingValueOf(SerializationInfo info, StreamingContext context)
        {
            this.Factory = (LogicTo<T>)info.GetValue("Factory", typeof(T));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Factory", this.Factory);
        }
        #endregion

        #region Properties
        private LogicTo<T> Factory { get; set; }
        #endregion

        #region Methods
        public T GetValue()
        {
            return this.Factory.CloneAndPerform();
        }
        #endregion

        #region Equality Overrides
        public override bool Equals(object obj)
        {
            return Equals(obj as ValueOf<T>);
        }
        public override string ToString()
        {
            //note we use the Logic's ability to serialize itself
            return this.Factory.ToString();
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        #endregion

        #region IEquatable
        public bool Equals(FactoriedValueOf<T> other)
        {
            if (other == null)
                return false;

            FactoriedValueOf<T> cObj = (FactoriedValueOf<T>)other;
            return cObj.ToString().Equals(this.ToString());
        }
        #endregion
    }


}
