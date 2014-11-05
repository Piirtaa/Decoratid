using CuttingEdge.Conditions;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Contextual
{
    [Serializable]
    public class ContextualCondition<T> : DecoratedConditionOfBase<T>, IHasContext<T>
    {
        #region Ctor
        public ContextualCondition(IConditionOf<T> decorated, T context)
            : base(decorated)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.Context = context;
        }
        #endregion

        #region ISerializable
        protected ContextualCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bool isNull = info.GetBoolean("_isNull");

            if (!isNull)
            {
                Type type = (Type)info.GetValue("_type", typeof(Type));
                IHasContext iCtx =  this;
                iCtx.Context = info.GetValue("_context", type);
            }
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
            bool isNull = this.Context == null;
            info.AddValue("_isNull", isNull);

            if (!isNull)
            {
                info.AddValue("_type", this.Context.GetType());
                info.AddValue("_context", this.Context);
            }
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasContext
        public T Context { get; set; }
        object IHasContext.Context
        {
            get
            {
                return this.Context;
            }
            set
            {
                this.Context = (T)value;
            }
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IConditionOf<T>> ApplyThisDecorationTo(IConditionOf<T> thing)
        {
            return new ContextualCondition<T>(thing, this.Context);
        }
        #endregion


    }

    public static class ContextualConditionExtensions
    {
        public static ContextualCondition<T> AddContext<T>(this IConditionOf<T> thing, T context)
        {
            Condition.Requires(thing).IsNotNull();
            return new ContextualCondition<T>(thing, context);
        }
    }
}
