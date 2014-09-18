using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core.ValueOfing.Decorations
{
    /// <summary>
    /// is a ValueOf that mutates its value
    /// </summary>
    /// <remarks>
    /// Hold on!  Why would we do this? So we can chain mutations/adjustments on a value. 
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IMutatingValueOf<T> : IValueOf<T>
    {
        T Mutate(T val);
    }

    [Serializable]
    public class StrategizedMutatingValueOfDecoration<T> : DecoratedValueOfBase<T>, IMutatingValueOf<T>
    {
        #region Ctor
        public StrategizedMutatingValueOfDecoration(IValueOf<T> decorated, LogicOfTo<T,T> mutateStrategy)
            : base(decorated)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutateStrategy = mutateStrategy;
        }
        #endregion
        
        #region ISerializable
        protected StrategizedMutatingValueOfDecoration(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.MutateStrategy = (LogicOfTo<T, T>)info.GetValue("MutateStrategy", typeof(LogicOfTo<T, T>));
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MutateStrategy", this.MutateStrategy);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        private LogicOfTo<T,T> MutateStrategy { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            var oldValue = Decorated.GetValue();
            var rv = this.MutateStrategy.CloneAndPerform(oldValue.AsNaturalValue());
            return rv;
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new StrategizedMutatingValueOfDecoration<T>(thing, this.MutateStrategy);
        }
        #endregion

    }

    public static partial class Extensions
    {
        /// <summary>
        /// applies an adjustment/mutation to a ValueOf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="mutateStrategy"></param>
        /// <returns></returns>
        public static StrategizedMutatingValueOfDecoration<T> ApplyMutation<T>(this IValueOf<T> valueOf, LogicOfTo<T, T> mutateStrategy)
        {
            Condition.Requires(valueOf).IsNotNull();
            Condition.Requires(mutateStrategy).IsNotNull();

            //wrap task
            return new StrategizedMutatingValueOfDecoration<T>(valueOf, mutateStrategy);
        }
    }

}
