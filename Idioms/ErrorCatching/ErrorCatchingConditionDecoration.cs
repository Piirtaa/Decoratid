using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core;

namespace Decoratid.Idioms.ErrorCatching
{
    /// <summary>
    /// Traps all errors that are thrown from the condition evaluation
    /// </summary>
    public class ErrorCatchingConditionDecoration : DecoratedConditionBase
    {
        #region Ctor
        public ErrorCatchingConditionDecoration(ICondition decorated)
            : base(decorated)
        {
        }
        #endregion
        
        #region Methods
        public override bool? Evaluate()
        {
            try
            {
                return Decorated.Evaluate();
            }
            catch
            {
            }
            return false;
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new ErrorCatchingConditionDecoration(thing);
        }
        #endregion
    }
}
