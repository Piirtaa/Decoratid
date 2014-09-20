﻿using CuttingEdge.Conditions;
using Decoratid.Idioms.Core;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core.ValueOfing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Mutating
{
    /// <summary>
    /// is a ValueOf decoration that evaluates the decorated ValueOf and then scrubs it according to some strategy
    /// </summary>
    /// <remarks>
    /// Hold on!  Why would we do this? So we can chain mutations/adjustments on a value. 
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IScrubbingValueOf<T> : IValueOf<T>
    {
        LogicOfTo<T,T> MutateStrategy {get;}
    }

    [Serializable]
    public class ScrubbingValueOfDecoration<T> : DecoratedValueOfBase<T>, IScrubbingValueOf<T>
    {
        #region Ctor
        public ScrubbingValueOfDecoration(IValueOf<T> decorated, LogicOfTo<T,T> mutateStrategy)
            : base(decorated)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutateStrategy = mutateStrategy;
        }
        #endregion
        
        #region ISerializable
        protected ScrubbingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
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
        public LogicOfTo<T,T> MutateStrategy { get; private set; }
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
            return new ScrubbingValueOfDecoration<T>(thing, this.MutateStrategy);
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
        public static ScrubbingValueOfDecoration<T> ApplyScrub<T>(this IValueOf<T> valueOf, LogicOfTo<T, T> mutateStrategy)
        {
            Condition.Requires(valueOf).IsNotNull();
            Condition.Requires(mutateStrategy).IsNotNull();
            return new ScrubbingValueOfDecoration<T>(valueOf, mutateStrategy);
        }
    }

}