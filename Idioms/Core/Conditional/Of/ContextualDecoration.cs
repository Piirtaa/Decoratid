using CuttingEdge.Conditions;

using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core.ValueOfing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core.Conditional.Of
{

    /// <summary>
    /// decorates as ICondition by providing context data 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class ContextualDecoration<T> : DecoratedConditionOfBase<T>, IHasContext<IValueOf<T>>, ICondition
    {
        #region Ctor
        public ContextualDecoration(IConditionOf<T> decorated, IValueOf<T> context)
            : base(decorated)
        {
            Condition.Requires(context).IsNotNull();
            this.Context = context;
        }
        #endregion

        #region Fluent Static
        public static ContextualDecoration<T> New(IConditionOf<T> decorated, IValueOf<T> context)
        {
            return new ContextualDecoration<T>(decorated, context);
        }
        #endregion

        #region ISerializable
        protected ContextualDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Context = (IValueOf<T>)info.GetValue("Context", typeof(IValueOf<T>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Context", this.Context);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasContext
        public IValueOf<T> Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { this.Context = (IValueOf<T>)value; } }
        #endregion

        #region ICondition
        public bool? Evaluate()
        {
            bool? rv = null;

            //get the value to apply
            var val = this.Context.GetValue();

            //clone the condition if we can
            if (this.Decorated is ICloneableCondition)
            {
                ICloneableCondition cloneCond = (ICloneableCondition)this.Decorated;
                IConditionOf<T> clone = (IConditionOf<T>)cloneCond.Clone();
                rv = clone.Evaluate(val);
            }
            rv = this.Decorated.Evaluate(val);
            return rv;
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion
    }

    public static class ContextualExtensions
    {
        /// <summary>
        /// Converts a ConditionOf into an ICondition by providing an argument valueOf 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condOf"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ICondition DefineCondition<T>(this IConditionOf<T> condOf, IValueOf<T> val)
        {
            return ContextualDecoration<T>.New(condOf, val);
        }
    }
}
