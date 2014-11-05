using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.ValueOfing
{
    /// <summary>
    /// decorate logic as a ValueOf
    /// </summary>
    [Serializable]
    public class ValueOfLogicDecoration<T> : DecoratedLogicBase, IValueOf<T>
    {
        #region Ctor
        public ValueOfLogicDecoration(ILogicTo<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ValueOfLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IValueOf
        public T GetValue()
        {
            ILogicTo<T> logic = (ILogicTo<T>)this.Decorated;
            Condition.Requires(logic).IsNotNull();

            ICloneableLogic cloneableLogic = logic as ICloneableLogic;
            Condition.Requires(cloneableLogic).IsNotNull();

            var newLogic = cloneableLogic.Clone();
            newLogic.Perform();
            ILogicTo<T> newLogicTo = newLogic as ILogicTo<T>;
            Condition.Requires(newLogicTo).IsNotNull();

            return newLogicTo.Result;
        }
        #endregion

        #region Methods
        public override void Perform()
        {
            Decorated.Perform();
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ValueOfLogicDecoration<T>((ILogicTo<T>)thing);
        }
        #endregion
    }

    public static class ValueOfLogicDecorationExtensions
    {
        public static ValueOfLogicDecoration<T> IsValueOf<T>(this ILogicTo<T> decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ValueOfLogicDecoration<T>(decorated);
        }
    }
}
