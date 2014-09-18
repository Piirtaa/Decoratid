using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Core.Logical;

using System.Runtime.Serialization;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Core.Conditional
{
    /// <summary>
    /// produces an Or condition based on the passed in conditions
    /// </summary>
    [Serializable]
    public sealed class Or : ICondition, ISerializable, ICloneableCondition
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
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Conditions", this.Conditions);
        }
        #endregion

        #region Properties
        public ICondition[] Conditions { get; protected set; }
        #endregion

        #region Methods
        public bool? Evaluate()
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
        public ICondition Clone()
        {
            return new Or(this.Conditions);
        }
        #endregion
    }

    public static class OrExtensions
    {
  
        /// <summary>
        /// fluent Or
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="conds"></param>
        /// <returns></returns>
        public static ICondition Or(this ICondition cond, params ICondition[] conds)
        {
            if (conds == null || conds.Length == 0)
                return cond;

            List<ICondition> newConds = new List<ICondition>();

            //if THIS is null, don't include it in the return condition
            if (cond != null)
                newConds.Add(cond);

            newConds.AddRange(conds);

            return new Or(newConds.ToArray());

        }
        /// <summary>
        /// reverses a fluent Or operation. Returns the conditions involved in an or, with the first element being the
        /// initial or'ed condition, and every other condition being the array of conditions that were or'ed upon this.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public static List<ICondition> DeOr(this ICondition cond)
        {
            Condition.Requires(cond).IsNotNull();
            if (!(cond is Or))
                throw new InvalidOperationException("Not an Or");

            Or or = cond as Or;
            return or.Conditions.ToList();
        }
    }
}
