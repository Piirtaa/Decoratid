using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Logical
{
    /// <summary>
    /// extends logic with HasId
    /// </summary>
    [Serializable]
    public class HasIdDecoration<TId> : DecoratedLogicBase, IHasId<TId>
    {
        #region Ctor
        public HasIdDecoration(ILogic decorated, TId id)
            : base(decorated)
        {
            this.Id = id;
        }
        #endregion

        #region ISerializable
        protected HasIdDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Id = (TId)info.GetValue("_id", typeof(TId));
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
            info.AddValue("_id", this.Id);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasId
        public TId Id { get; set; }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Overrides
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new HasIdDecoration<TId>(thing, this.Id);
        }
        #endregion
    }

    public static partial class HasIdDecorationExtensions
    {
        public static HasIdDecoration<TId> HasId<TId>(this ILogic decorated, TId id)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasIdDecoration<TId>(decorated, id);
        }
    }
}
