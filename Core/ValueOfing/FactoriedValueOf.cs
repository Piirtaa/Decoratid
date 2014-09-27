using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.ValueOfing
{
    /// <summary>
    /// a value of that utilizes a factory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactoriedValueOf<T>: IValueOf<T>
    {
        /// <summary>
        /// the strategy to build the ValueOf 
        /// </summary>
        /// <remarks>
        /// We use the concrete type LogicTo instead of ILogicTo to explicitly enforce the requirement that the factory
        /// is fully hydrated before using it - put the onus on the setter to conform to LogicTo
        /// </remarks>
        LogicTo<T> Factory {get;}
    }

    /// <summary>
    /// use a factory to get a value.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
   /// </remarks>
    [Serializable]
    public sealed class FactoriedValueOf<T> : IValueOf<T>, ISerializable, IEquatable<FactoriedValueOf<T>>
    {
        #region Ctor
        /// <summary>
        /// Here we supply a factory delegate method
        /// </summary>
        /// <param name="factory"></param>
        /// <remarks>
        /// We use the concrete type LogicTo to explicitly enforce the requirement that the factory (eg. ILogicTo) is fully hydrated
        /// before querying it.  If we allowed use of ILogicOfTo in the factory, the context would need to be hydrated.  
        /// Additionally, we use CloneAndPerform as the perform mechanism to ensure the stateless nature of the logic, and LogicTo supports this.
        /// </remarks>
        public FactoriedValueOf(LogicTo<T> factory) 
        {
            Condition.Requires(factory).IsNotNull();
            this.Factory = factory;
        }
        #endregion

        #region Static Methods
        public static FactoriedValueOf<T> New(LogicTo<T> factory)
        {
            return new FactoriedValueOf<T>(factory);
        }
        #endregion
        
        #region ISerializable
        private FactoriedValueOf(SerializationInfo info, StreamingContext context)
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
