using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Conditions
{
    /// <summary>
    /// defines a simple condition
    /// </summary>
    public interface ICondition
    {
        bool? Evaluate();
    }

    /// <summary>
    /// a condition that can mutate.  
    /// </summary>
    public interface IMutableCondition : ICondition
    {
        void Mutate();
    }

    /// <summary>
    /// a condition that can clone itself
    /// </summary>
    public interface ICloneableCondition 
    {
        ICondition Clone();
    }

    /// <summary>
    /// a condition that needs a contextual argument for evaluation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConditionOf<T>
    {
        bool? Evaluate(T context);
    }
}
