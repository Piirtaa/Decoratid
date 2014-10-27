using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Communicating;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Identifying
{

    public interface IHasVersion
    {
        string Version { get; set; }
    }

    /// <summary>
    /// extends IHasId with Version
    /// </summary>
    [Serializable]
    public class HasVersionDecoration : DecoratedHasIdBase, IHasVersion
    {


        #region Ctor
        public HasVersionDecoration(IHasId decorated, string version)
            : base(decorated)
        {
            Condition.Requires(version).IsNotNull();
            this.Version = version;
        }
        #endregion

        #region ISerializable
        protected HasVersionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Version = info.GetString("Version");
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
            info.AddValue("Version", this.Version);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasVersion
        public String Version { get; set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasVersionDecoration(thing, this.Version);
        }
        #endregion
    }

    public static partial class HasVersionDecorationExtensions
    {
        public static HasVersionDecoration HasVersion(this IHasId decorated, string Version)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasVersionDecoration(decorated, Version);
        }
    }
}
