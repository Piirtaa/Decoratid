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
    /// produces an And condition based on the passed in conditions
    /// </summary>
    [Serializable]
    public class And : ICondition, ISerializable, ICloneableCondition
    {
        #region Ctor
        public And(params ICondition[] conditions)
        {
            this.Conditions = conditions;
        }
        #endregion

        #region ISerializable
        protected And(SerializationInfo info, StreamingContext context)
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

            //in an AND, if any of the terms are false the expression is false
            foreach (ICondition each in this.Conditions)
            {
                if (!each.Evaluate().GetValueOrDefault())
                {
                    return false;
                }
            }
            return true;
        }
        public virtual ICondition Clone()
        {
            return new And(this.Conditions);
        }
        #endregion
    }


}
