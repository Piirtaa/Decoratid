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

    public interface IHasDateCreated
    {
        DateTime DateCreated { get; set; }
    }

    /// <summary>
    /// extends IHasId with DateCreated
    /// </summary>
    [Serializable]
    public class HasDateCreatedDecoration : DecoratedHasIdBase, IHasDateCreated
    {
        #region Ctor
        public HasDateCreatedDecoration(IHasId decorated, DateTime dateCreated)
            : base(decorated)
        {
            this.DateCreated = dateCreated.ToUniversalTime();
        }
        #endregion

        #region ISerializable
        protected HasDateCreatedDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.DateCreated = info.GetDateTime("DateCreated");
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
            info.AddValue("DateCreated", this.DateCreated);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasDateCreated
        public DateTime DateCreated { get; set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasDateCreatedDecoration(thing, this.DateCreated);
        }
        #endregion
    }

    public static partial class HasDateCreatedDecorationExtensions
    {
        public static HasDateCreatedDecoration HasDateCreated(this IHasId decorated, DateTime dateCreated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasDateCreatedDecoration(decorated, dateCreated);
        }
        public static HasDateCreatedDecoration HasNewDateCreated(this IHasId decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasDateCreatedDecoration(decorated, DateTime.UtcNow);
        }
    }
}
