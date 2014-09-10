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
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Utils;

namespace Decoratid.Thingness.Idioms.ValuesOf
{
    /// <summary>
    /// use a factory to get a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// We use the concrete type LogicTo to explicitly enforce the requirement that the factory (eg. ILogicTo) is fully hydrated
    /// before querying it.  If we allowed use of ILogicOfTo in the factory, the context would need to be hydrated.  
    /// Additionally, we use CloneAndPerform as the perform mechanism to ensure the stateless nature of the logic, and LogicTo supports this.
    /// </remarks>
    public sealed class FactoriedValueOf<T> : IValueOf<T>, IEquatable<FactoriedValueOf<T>>
    {
        #region Ctor
        /// <summary>
        /// Here we supply a factory delegate method
        /// </summary>
        /// <param name="factory"></param>
        public FactoriedValueOf(LogicTo<T> factory)
        {
            Condition.Requires(factory).IsNotNull();
            this.Factory = factory;
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

        #region Static Methods
        public static FactoriedValueOf<T> New(LogicTo<T> factory)
        {
            return new FactoriedValueOf<T>(factory);
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            return Equals(obj as FactoriedValueOf<T>);
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override string ToString()
        {
            string dat = SerializationManager.Instance.Serialize(BinarySerializationUtil.ID, this);
            return dat;
        }
        #endregion

        #region IEquatable
        public bool Equals(FactoriedValueOf<T> other)
        {
            if (other == null)
                return false;

            FactoriedValueOf<T> cObj = (FactoriedValueOf<T>)other;
            return cObj.Factory.Equals(this.Factory);
        }
        #endregion
    }


}
