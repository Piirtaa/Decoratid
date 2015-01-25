using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Contextual
{
    /// <summary>
    /// extends IHasId with context
    /// </summary>
    [Serializable]
    public class ContextualIHasIdDecoration : DecoratedHasIdBase, IHasContext, IContextualHasId
    {
        #region Ctor
        public ContextualIHasIdDecoration(IHasId decorated, object context)
            : base(decorated)
        {
            this.Context = context;
        }
        #endregion

        #region ISerializable
        protected ContextualIHasIdDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bool isNull = info.GetBoolean("_isNull");

            if (!isNull)
            {
                Type type = (Type)info.GetValue("_type", typeof(Type));
                this.Context = info.GetValue("_context", type);
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
        public object Context {get; set;}
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new ContextualIHasIdDecoration(thing, this.Context);
        }
        #endregion
    }

    public static partial class ContextualIHasIdDecorationExtensions
    {
        public static ContextualIHasIdDecoration HasContext(this IHasId decorated, object context)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ContextualIHasIdDecoration(decorated, context);
        }
    }
}
