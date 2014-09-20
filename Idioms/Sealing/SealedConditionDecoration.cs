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

namespace Decoratid.Idioms.Validating
{
    public interface IHasValidator
    {
        ICondition IsValidCondition {get;}
    }

    public class ValidatingConditionDecoration : DecoratedConditionBase, IHasValidator
    {
        #region Ctor
        public ValidatingConditionDecoration(ICondition decorated, ICondition isValidCondition)
            : base(decorated)
        {
            Condition.Requires(isValidCondition).IsNotNull();
        }
        #endregion

        #region Properties
        public ICondition IsValidCondition {get; private set;}
        #endregion

        #region Methods
        public override bool? Evaluate()
        {
            if(this.IsValidCondition
                return base.Evaluate();
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new LoggingConditionDecoration(thing);
        }
        #endregion
    }


}
