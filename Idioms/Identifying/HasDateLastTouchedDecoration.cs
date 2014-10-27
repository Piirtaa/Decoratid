using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Communicating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Identifying
{

    public interface IHasDateLastTouched
    {
        DateTime DateLastTouched { get; set; }
    }

    /// <summary>
    /// extends IHasId with DateLastTouched
    /// </summary>
    [Serializable]
    public class HasDateLastTouchedDecoration : DecoratedHasIdBase, IHasDateLastTouched
    {
        #region Ctor
        public HasDateLastTouchedDecoration(IHasId decorated, DateTime DateLastTouched)
            : base(decorated)
        {
            this.DateLastTouched = DateLastTouched.ToUniversalTime();
        }
        #endregion

        #region ISerializable
        protected HasDateLastTouchedDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.DateLastTouched = info.GetDateTime("DateLastTouched");
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
            info.AddValue("DateLastTouched", this.DateLastTouched);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasDateLastTouched
        public DateTime DateLastTouched { get; set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasDateLastTouchedDecoration(thing, this.DateLastTouched);
        }
        #endregion
    }

    public static partial class HasDateLastTouchedDecorationExtensions
    {
        public static HasDateLastTouchedDecoration HasDateLastTouched(this IHasId decorated, DateTime DateLastTouched)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasDateLastTouchedDecoration(decorated, DateLastTouched);
        }
        public static HasDateLastTouchedDecoration HasNewDateLastTouched(this IHasId decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasDateLastTouchedDecoration(decorated, DateTime.UtcNow);
        }
    }
}
