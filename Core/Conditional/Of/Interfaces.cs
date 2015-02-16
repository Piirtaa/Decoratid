using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Conditional.Of
{
    /// <summary>
    /// a condition that needs a contextual argument for evaluation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConditionOf<T>
    {
        bool? Evaluate(T context);
    }

}
