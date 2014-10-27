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

    public interface IHasIP
    {
        IPAddress IPAddress { get; set; }
    }

    /// <summary>
    /// extends IHasId with ip
    /// </summary>
    [Serializable]
    public class HasIPDecoration : DecoratedHasIdBase, IHasIP
    {
        #region Ctor
        public HasIPDecoration(IHasId decorated, IPAddress addr)
            : base(decorated)
        {
            Condition.Requires(addr).IsNotNull();
            this.IPAddress = addr;
        }
        #endregion

        #region ISerializable
        protected HasIPDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IPAddress = (IPAddress)info.GetValue("IPAddress", typeof(IPAddress));
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
            info.AddValue("IPAddress", this.IPAddress);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasIP
        public IPAddress IPAddress { get; set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasIPDecoration(thing, this.IPAddress);
        }
        #endregion
    }

    public static partial class HasIPDecorationExtensions
    {
        public static HasIPDecoration HasIP(this IHasId decorated, IPAddress addr)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasIPDecoration(decorated, addr);
        }
        public static HasIPDecoration HasLocalIP(this IHasId decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasIPDecoration(decorated, NetUtil.GetLocalIPAddresses().FirstOrDefault());
        }
    }
}
