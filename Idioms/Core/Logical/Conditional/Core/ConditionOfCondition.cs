using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Core.Conditional.Of;

namespace Decoratid.Idioms.Core.Conditional.Core
{
    /// <summary>
    /// Takes an IConditionOf and converts it to an ICondition by keeping context IValueOf as state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class ConditionOfCondition<T> : ICondition, IHasContext<IValueOf<T>>, ICloneableCondition, ISerializable
    {
        #region Ctor
        public ConditionOfCondition(IValueOf<T> context, IConditionOf<T> conditionOf)
        {
            Condition.Requires(context).IsNotNull();
            Condition.Requires(conditionOf).IsNotNull();
            this.Context = context;
            this.ConditionOf = conditionOf;
        }
        #endregion

        #region Static Fluent Methods
        public static ConditionOfCondition<T> New(IValueOf<T> context, IConditionOf<T> conditionOf)
        {
            return new ConditionOfCondition<T>(context, conditionOf);
        }
        #endregion

        #region ISerializable
        protected ConditionOfCondition(SerializationInfo info, StreamingContext context)
        {
            this.ConditionOf = (IConditionOf<T>)info.GetValue("_ConditionOf", typeof(IConditionOf<T>));
            this.Context = (IValueOf<T>)info.GetValue("_Context", typeof(IValueOf<T>));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_ConditionOf", this.ConditionOf);
            info.AddValue("_Context", this.Context);
        }
        #endregion

        #region Properties
        public IValueOf<T> Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { this.Context = (IValueOf<T>)value; } }
        public IConditionOf<T> ConditionOf { get; protected set; }
        #endregion

        #region Methods
        public bool? Evaluate()
        {
            var val = this.Context.GetValue();

            //clone the condition if we can
            if (this.ConditionOf is ICloneableCondition)
            {
                ICloneableCondition cloneCond = (ICloneableCondition)this.ConditionOf;
                IConditionOf<T> clone = (IConditionOf<T>)cloneCond.Clone();
                return clone.Evaluate(val);
            }
            return this.ConditionOf.Evaluate(val);
        }
        public ICondition Clone()
        {
            return new ConditionOfCondition<T>(this.Context, this.ConditionOf);
        }
        #endregion
    }

    public static class ConditionOfConditionExtensions
    {
        /// <summary>
        /// converts an IConditionOf to an ICondition by 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionOf"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ConditionOfCondition<T> ConvertToCondition<T>(this IConditionOf<T> conditionOf, IValueOf<T> context)
        {
            return new ConditionOfCondition<T>(context, conditionOf);
        }
    }

}
