using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Adjusting
{
    public interface IAdjustment<T>
    {
        LogicOfTo<T, T> AdjustmentLogic { get; }
        T AdjustedValue { get; }
    }

    /// <summary>
    /// composites IAdjustment
    /// </summary>
    public interface IHasAdjustment<T> : IAdjustment<T>
    {
        IAdjustment<T> Adjustment { get; set; }
    }
}
