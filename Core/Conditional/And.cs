using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Decoratid.Core.Conditional
{
    /// <summary>
    /// produces an And condition based on the passed in conditions
    /// </summary>
    [Serializable]
    public sealed class And : ICondition, ISerializable, ICloneableCondition
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
        public ICondition Clone()
        {
            return new And(this.Conditions);
        }
        #endregion
    }

    public static class AndExtensions
    {
        /// <summary>
        /// fluent And 
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="conds"></param>
        /// <returns></returns>
        public static ICondition And(this ICondition cond, params ICondition[] conds)
        {
            if (conds == null || conds.Length == 0)
                return cond;

            List<ICondition> newConds = new List<ICondition>();

            //if THIS is null, don't include it in the return condition
            if (cond != null)
                newConds.Add(cond);

            newConds.AddRange(conds);

            return new And(newConds.ToArray());

        }
        /// <summary>
        /// reverses a fluent And operation. Returns the conditions involved in an and, with the first element being the
        /// initial and'ed condition, and every other condition being the array of conditions that were and'ed upon this.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public static List<ICondition> DeAnd(this ICondition cond)
        {
            Condition.Requires(cond).IsNotNull();
            if (!(cond is And))
                throw new InvalidOperationException("Not an And");

            And and = cond as And;
            return and.Conditions.ToList();
        }
    }
}
