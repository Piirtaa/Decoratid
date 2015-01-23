using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.ValueOfing
{
    /// <summary>
    /// extends logic with HasId
    /// </summary>
    [Serializable]
    public class HasIdDecoration<T, TId> : DecoratedValueOfBase<T>, IHasId<TId>
    {
        #region Ctor
        public HasIdDecoration(IValueOf<T> decorated, TId id)
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
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new HasIdDecoration<T, TId>(thing, this.Id);
        }
        #endregion
    }

    public static partial class HasIdDecorationExtensions
    {
        /// <summary>
        /// gives a valueof an id decoration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="decorated"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static HasIdDecoration<T, TId> HasId<T, TId>(this IValueOf<T> decorated, TId id)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasIdDecoration<T, TId>(decorated, id);
        }
    }
}
