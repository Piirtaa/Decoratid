using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Decoratid.Core.Conditional.Of
{
    /// <summary>
    /// produces an And condition based on the passed in conditions
    /// </summary>
    [Serializable]
    public sealed class AndOf<T> : IConditionOf<T>, ISerializable
    {
        #region Ctor
        public AndOf(params IConditionOf<T>[] conditions)
        {
            this.Conditions = conditions;
        }
        #endregion

        #region Fluent Static
        public static AndOf<T> New(params IConditionOf<T>[] conditions)
        {
            return new AndOf<T>(conditions);
        }
        #endregion

        #region ISerializable
        protected AndOf(SerializationInfo info, StreamingContext context)
        {
            this.Conditions = (IConditionOf<T>[])info.GetValue("Conditions", typeof(IConditionOf<T>[]));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Conditions", this.Conditions);
        }
        #endregion

        #region Properties
        public IConditionOf<T>[] Conditions { get; protected set; }
        #endregion

        #region Methods
        public bool? Evaluate(T context)
        {
            if (this.Conditions == null) { return false; }
            if (this.Conditions.Length == 0) { return false; }

            //in an AND, if any of the terms are false the expression is false
            foreach (IConditionOf<T> each in this.Conditions)
            {
                if (!each.Evaluate(context).GetValueOrDefault())
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }

    public static class AndOfExtensions
    {
        /// <summary>
        /// fluent And 
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="conds"></param>
        /// <returns></returns>
        public static IConditionOf<T> And<T>(this IConditionOf<T> cond, params IConditionOf<T>[] conds)
        {
            if (conds == null || conds.Length == 0)
                return cond;

            List<IConditionOf<T>> newConds = new List<IConditionOf<T>>();

            //if THIS is null, don't include it in the return condition
            if (cond != null)
                newConds.Add(cond);

            newConds.AddRange(conds);

            return new AndOf<T>(newConds.ToArray());

        }
    }
}
