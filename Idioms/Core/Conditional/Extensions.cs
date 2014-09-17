using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional.Common;
using Decoratid.Extensions;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Core.Conditional.Core;

namespace Decoratid.Idioms.Core.Conditional
{
    public static class Extensions
    {
        #region Boolean
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

        #endregion

    }
}
