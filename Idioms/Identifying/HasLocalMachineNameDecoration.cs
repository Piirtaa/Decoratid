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

    public interface IHasMachineName
    {
        string MachineName { get; set; }
    }

    /// <summary>
    /// extends IHasId with MachineName
    /// </summary>
    [Serializable]
    public class HasMachineNameDecoration : DecoratedHasIdBase, IHasMachineName
    {
        #region Ctor
        public HasMachineNameDecoration(IHasId decorated, string machineName)
            : base(decorated)
        {
            Condition.Requires(machineName).IsNotNull();
            this.MachineName = machineName;
        }
        #endregion

        #region ISerializable
        protected HasMachineNameDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.MachineName = info.GetString("MachineName");
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
            info.AddValue("MachineName", this.MachineName);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasMachineName
        public String MachineName { get; set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasMachineNameDecoration(thing, this.MachineName);
        }
        #endregion
    }

    public static partial class HasMachineNameDecorationExtensions
    {
        public static HasMachineNameDecoration HasMachineName(this IHasId decorated, string machineName)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasMachineNameDecoration(decorated, machineName);
        }
        public static HasMachineNameDecoration HasLocalMachineName(this IHasId decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasMachineNameDecoration(decorated, NetUtil.GetLocalMachineName());
        }
    }
}
