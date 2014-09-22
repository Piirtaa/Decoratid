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
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// 
    [Serializable]
    public class ValidatingLogicDecoration : DecoratedLogicBase, IHasValidator
    {
        #region Ctor
        public ValidatingLogicDecoration(ILogic decorated, ICondition isValidCondition)
            : base(decorated)
        {
            Condition.Requires(isValidCondition).IsNotNull();
            this.IsValidCondition = isValidCondition;
        }
        #endregion

        #region ISerializable
        protected ValidatingLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public override void Perform()
        {
            var condVal = this.IsValidCondition.Evaluate();
            if (!condVal.GetValueOrDefault())
                throw new InvalidOperationException("Condition not ready");

            Decorated.Perform();
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ValidatingLogicDecoration(thing, this.IsValidCondition);
        }
        #endregion
    }

    public static class ValidatingLogicDecorationExtensions
    {
        public static ValidatingLogicDecoration DecorateWithValidation(ILogic decorated, ICondition validatingCondition)
        {
            Condition.Requires(decorated).IsNotNull();
            if (decorated is ValidatingLogicDecoration)
            {
                var rv = decorated as ValidatingLogicDecoration;
                return rv;
            }
            return new ValidatingLogicDecoration(decorated, validatingCondition);
        }
    }
}
