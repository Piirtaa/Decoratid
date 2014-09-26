using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Core.Logical;
using Decoratid.Core;

namespace Decoratid.Idioms.Validating
{

    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// 
    [Serializable]
    public class ValidatingConditionDecoration : DecoratedConditionBase, IHasValidator
    {
        #region Ctor
        public ValidatingConditionDecoration(ICondition decorated, ICondition isValidCondition)
            : base(decorated)
        {
            Condition.Requires(isValidCondition).IsNotNull();
            this.IsValidCondition = isValidCondition;
        }
        #endregion

        #region ISerializable
        protected ValidatingConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsValidCondition = (ICondition)info.GetValue("IsValidCondition", typeof(ICondition));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsValidCondition", this.IsValidCondition); ;
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public ICondition IsValidCondition { get; private set; }
        #endregion

        #region Methods
        public override bool? Evaluate()
        {
            var condVal = this.IsValidCondition.Evaluate();
            if (!condVal.GetValueOrDefault())
                throw new InvalidOperationException("Condition not ready");

            return base.Evaluate();
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new ValidatingConditionDecoration(thing, this.IsValidCondition);
        }
        #endregion
    }

    public static class ValidatingConditionDecorationExtensions
    {
        public static ValidatingConditionDecoration KackUntil(ICondition decorated, ICondition validatingCondition)
        {
            Condition.Requires(decorated).IsNotNull();
            if (decorated is ValidatingConditionDecoration)
            {
                var rv = decorated as ValidatingConditionDecoration;
                return rv;
            }
            return new ValidatingConditionDecoration(decorated, validatingCondition);
        }
    }
}
