using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Polyfacing
{
    [Serializable]
    public class PolyfacingIHasIdDecoration : DecoratedHasIdBase, IPolyfacing
    {
        #region Ctor
        public PolyfacingIHasIdDecoration(IHasId decorated,  Polyface rootFace = null)
            : base(decorated)
        {
            this.RootFace = rootFace;
        }
        #endregion

        #region ISerializable
        protected PolyfacingIHasIdDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
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
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IPolyfacing
        public Polyface RootFace { get; set; }
        #endregion


        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new PolyfacingIHasIdDecoration(thing, this.RootFace);
        }
        #endregion
    }

    public static partial class PolyfacingIHasIdDecorationExtensions
    {
        public static PolyfacingIHasIdDecoration Polyfacing(this IHasId decorated, Polyface rootFace = null)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PolyfacingIHasIdDecoration(decorated, rootFace);
        }
    }
}
