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
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Validating
{
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class ValidatingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasValidator
    {
        #region Ctor
        public ValidatingValueOfDecoration(IValueOf<T> decorated, ICondition isValidCondition)
            : base(decorated)
        {
            Condition.Requires(isValidCondition).IsNotNull();
            this.IsValidCondition = isValidCondition;
        }
        #endregion

        #region ISerializable
        protected ValidatingValueOfDecoration(SerializationInfo info, StreamingContext context)
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
        public override T GetValue()
        {
            var condVal = this.IsValidCondition.Evaluate();
            if (!condVal.GetValueOrDefault())
                throw new InvalidOperationException("Condition not ready");

            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ValidatingValueOfDecoration<T>(thing, this.IsValidCondition);
        }
        #endregion
    }

    public static class ValidatingValueOfDecorationExtensions
    {
        public static ValidatingValueOfDecoration<T> DecorateWithValidation<T>(IValueOf<T> decorated, ICondition validatingCondition)
        {
            Condition.Requires(decorated).IsNotNull();
            if (decorated is ValidatingValueOfDecoration<T>)
            {
                var rv = decorated as ValidatingValueOfDecoration<T>;
                return rv;
            }
            return new ValidatingValueOfDecoration<T>(decorated, validatingCondition);
        }
    }
}
