using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Logical;

using System.Runtime.Serialization;
using CuttingEdge.Conditions;

namespace Decoratid.Core.Conditional.Of
{
    /// <summary>
    /// produces an Or condition based on the passed in conditions
    /// </summary>
    [Serializable]
    public sealed class OrOf<T> : IConditionOf<T>, ISerializable
    {
        #region Ctor
        public OrOf(params IConditionOf<T>[] conditions)
        {
            this.Conditions = conditions;
        }
        #endregion

        #region ISerializable
        protected OrOf(SerializationInfo info, StreamingContext context)
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

            //in an Or, if any of the terms are true the expression is true
            foreach (IConditionOf<T> each in this.Conditions)
            {
                if (each.Evaluate(context).GetValueOrDefault())
                {
                    return true;
                }
            }
            return false;
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
        public static IConditionOf<T> OrOf<T>(this IConditionOf<T> cond, params IConditionOf<T>[] conds)
        {
            if (conds == null || conds.Length == 0)
                return cond;

            List<IConditionOf<T>> newConds = new List<IConditionOf<T>>();

            //if THIS is null, don't include it in the return condition
            if (cond != null)
                newConds.Add(cond);

            newConds.AddRange(conds);

            return new OrOf<T>(newConds.ToArray());
        }
    }
}
