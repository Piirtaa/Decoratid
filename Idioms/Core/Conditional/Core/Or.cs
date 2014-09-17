using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Core.Logical;

using System.Runtime.Serialization;

namespace Decoratid.Idioms.Core.Conditional.Core
{
    /// <summary>
    /// produces an Or condition based on the passed in conditions
    /// </summary>
    [Serializable]
    public class Or : ICondition, ISerializable, ICloneableCondition
    {
        #region Ctor
        public Or(params ICondition[] conditions)
        {
            this.Conditions = conditions;
        }
        #endregion

        #region ISerializable
        protected Or(SerializationInfo info, StreamingContext context)
        {
            this.Conditions = (ICondition[])info.GetValue("Conditions", typeof(ICondition[]));
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Conditions", this.Conditions);
        }
        #endregion

        #region Properties
        public ICondition[] Conditions { get; protected set; }
        #endregion

        #region Methods
        public virtual bool? Evaluate()
        {
            if (this.Conditions == null) { return false; }
            if (this.Conditions.Length == 0) { return false; }

            //in an Or, if any of the terms are true the expression is true
            foreach (ICondition each in this.Conditions)
            {
                if (each.Evaluate().GetValueOrDefault())
                {
                    return true;
                }
            }
            return false;
        }
        public virtual ICondition Clone()
        {
            return new Or(this.Conditions);
        }
        #endregion
    }


}
