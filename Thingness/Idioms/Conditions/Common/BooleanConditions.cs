using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness;
using System.Runtime.Serialization;

namespace Decoratid.Thingness.Idioms.Conditions.Common
{
    /// <summary>
    /// the boolean "operations" that can be performed on ICondition instances
    /// </summary>
    public enum IConditionBooleanOperator
    {
        And,
        Or,
        DeAnd,
        DeOr
    }

    /// <summary>
    /// produces an Or condition based on the passed in conditions.  mutating, with each mutable condition mutating
    /// </summary>
    /// 
    [Serializable]
    public class OrCondition : MutableStrategizedCondition
    {
        private readonly ICondition[] _conditions;
        public ICondition[] Conditions { get { return _conditions; } }

        //protected OrCondition(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}

        public OrCondition(params ICondition[] conditions)
            : base(
            LogicTo<bool?>.New(
            () =>
            {
                if (conditions == null) { return false; }
                if (conditions.Length == 0) { return false; }

                //in an or, if any of the terms are true the expression is true
                foreach (ICondition each in conditions)
                {
                    if (each.Evaluate().GetValueOrDefault())
                    {
                        return true;
                    }
                }
                return false;
            }),
            Logic.New(() =>
            {
                if (conditions == null) { return; }
                if (conditions.Length == 0) { return; }

                //in an or, if any of the terms are true the expression is true
                foreach (ICondition each in conditions)
                {
                    if (each is IMutableCondition)
                    {
                        ((IMutableCondition)each).Mutate();
                    }
                }
            }))
        {
            this._conditions = conditions;
        }


    }
    /// <summary>
    /// produces an And condition based on the passed in conditions.  mutating, with each mutable condition mutating
    /// </summary>
    /// 
    [Serializable]
    public class AndCondition : MutableStrategizedCondition
    {
        private readonly ICondition[] _conditions;
        public ICondition[] Conditions { get { return _conditions; } }
       
        //protected AndCondition(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}
        
        public AndCondition(params ICondition[] conditions)
            : base(
            () =>
            {
                if (conditions == null) { return false; }
                if (conditions.Length == 0) { return false; }

                //in an and, if any of the terms are false the expression is false
                foreach (ICondition each in conditions)
                {
                    if (!each.Evaluate().GetValueOrDefault())
                    {
                        return false;
                    }
                }
                return true;
            },
            () =>
            {
                if (conditions == null) { return; }
                if (conditions.Length == 0) { return; }

                //in an or, if any of the terms are true the expression is true
                foreach (ICondition each in conditions)
                {
                    if (each is IMutableCondition)
                    {
                        ((IMutableCondition)each).Mutate();
                    }
                }
            })
        {
            this._conditions = conditions;
        }
    }
}
