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

    public interface IHasGUID
    {
        Guid GUID { get; set; }
    }

    /// <summary>
    /// extends IHasId with GUID
    /// </summary>
    [Serializable]
    public class HasGUIDDecoration : DecoratedHasIdBase, IHasGUID
    {
        #region Ctor
        public HasGUIDDecoration(IHasId decorated, Guid guid)
            : base(decorated)
        {
            this.GUID = guid;
        }
        #endregion

        #region ISerializable
        protected HasGUIDDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.GUID = Guid.Parse(info.GetString("GUID"));
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
            info.AddValue("GUID", this.GUID.ToString());
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasGUID
        public Guid GUID { get; set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasGUIDDecoration(thing, this.GUID);
        }
        #endregion
    }

    public static partial class HasGUIDDecorationExtensions
    {
        public static HasGUIDDecoration HasGUID(this IHasId decorated, Guid GUID)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasGUIDDecoration(decorated, GUID);
        }
        public static HasGUIDDecoration HasAutomaticGUID(this IHasId decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasGUIDDecoration(decorated, Guid.NewGuid());
        }
    }
}
