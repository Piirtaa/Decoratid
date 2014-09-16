using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core.Logical
{
    /// <summary>
    /// base class for ILogic.  Wires up serialization and ToString/Equals plumbing
    /// </summary>
    [Serializable]
    public abstract class LogicBase : ILogic, ICloneableLogic,  ISerializable
    {
        #region Ctor
        public LogicBase()
        {
        }
        #endregion

        #region ISerializable
        protected LogicBase(SerializationInfo info, StreamingContext context)
        {
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected virtual void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
        #endregion


        #region ILogic
        public abstract void Perform();
        #endregion

        #region ICloneableLogic
        public abstract ILogic Clone();
        #endregion

        #region Equals Overrides
        public override bool Equals(object obj)
        {         
            if (obj == null)
                return false;
            
            //if we're dealing with the same reference, we're good
            if (object.ReferenceEquals(this, obj))
                return true;

            return this.ToString().Equals(obj.ToString());
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override string ToString()
        {
            return BinarySerializationUtil.Serialize(this);
        }
        #endregion
    }
}
