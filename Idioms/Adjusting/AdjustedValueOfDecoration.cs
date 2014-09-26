using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Adjusting
{
    /// <summary>
    /// is a ValueOf decoration that evaluates the decorated ValueOf and then scrubs it according to some strategy
    /// </summary>
    /// <remarks>
    /// Hold on!  Why would we do this? So we can chain adjustments on a value. 
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IAdjustedValueOf<T> : IValueOf<T>
    {
        LogicOfTo<T,T> ScrubbingStrategy {get;}
    }

    /// <summary>
    /// is a ValueOf decoration that evaluates the decorated ValueOf and then scrubs it according to some strategy
    /// </summary>
    [Serializable]
    public class AdjustedValueOfDecoration<T> : DecoratedValueOfBase<T>, IAdjustedValueOf<T>
    {
        #region Ctor
        public AdjustedValueOfDecoration(IValueOf<T> decorated, LogicOfTo<T, T> ScrubbingStrategy)
            : base(decorated)
        {
            Condition.Requires(ScrubbingStrategy).IsNotNull();
            this.ScrubbingStrategy = ScrubbingStrategy;
        }
        #endregion
        
        #region ISerializable
        protected AdjustedValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ScrubbingStrategy = (LogicOfTo<T, T>)info.GetValue("ScrubbingStrategy", typeof(LogicOfTo<T, T>));
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
            info.AddValue("ScrubbingStrategy", this.ScrubbingStrategy);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public LogicOfTo<T,T> ScrubbingStrategy { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            var oldValue = Decorated.GetValue();
            var rv = this.ScrubbingStrategy.CloneAndPerform(oldValue.AsNaturalValue());
            return rv;
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new AdjustedValueOfDecoration<T>(thing, this.ScrubbingStrategy);
        }
        #endregion
    }

    public static partial class ScrubbingValueOfExtensions
    {
        /// <summary>
        /// applies an adjustment/mutation to a ValueOf but doesn't change the underlying value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="mutateStrategy"></param>
        /// <returns></returns>
        public static AdjustedValueOfDecoration<T> Adust<T>(this IValueOf<T> valueOf, LogicOfTo<T, T> mutateStrategy)
        {
            Condition.Requires(valueOf).IsNotNull();
            Condition.Requires(mutateStrategy).IsNotNull();
            return new AdjustedValueOfDecoration<T>(valueOf, mutateStrategy);
        }
    }

}
