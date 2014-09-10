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
    /// does some stuff
    /// </summary>
    [Serializable]
    public sealed class Logic : ILogic, ICloneableLogic,  IEquatable<Logic>, ISerializable, IManagedHydrateable
    {
        #region Ctor
        public Logic(Action action)
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
        }
        #endregion

        #region ISerializable
        private Logic(SerializationInfo info, StreamingContext context)
        {
            this.Action = (Action)info.GetValue("_Action", typeof(Action));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Action", this.Action);
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
        private Action Action { get; set; }
        #endregion

        #region ILogic
        public void Perform()
        {
            Action();
        }
        #endregion

        #region ICloneableLogic
        public ILogic Clone()
        {
            return new Logic(this.Action);
        }
        #endregion

        #region Clone and Run
        public void CloneAndPerform()
        {
            Logic clone = (Logic)this.Clone();
            clone.Perform();
        }
        #endregion

        #region Static Methods
        public static Logic New(Action action)
        {
            Condition.Requires(action).IsNotNull();
            return new Logic(action);
        }
        #endregion

        #region Equals Overrides
        public override bool Equals(object obj)
        {
            //if we're dealing with the same reference, we're good
            if (object.ReferenceEquals(this, obj))
                return true;

            return Equals(obj as Logic);
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
        public bool Equals(Logic other)
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
