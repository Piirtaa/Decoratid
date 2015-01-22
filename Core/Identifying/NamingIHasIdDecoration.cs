using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Identifying
{
    public interface IHasName
    {
        string Name { get; }
    }

    /// <summary>
    /// decorates with a name
    /// </summary>
    [Serializable]
    public class NamingIHasIdDecoration : DecoratedHasIdBase, IHasName
    {
        #region Ctor
        public NamingIHasIdDecoration(IHasId decorated, string name)
            : base(decorated)
        {
            Condition.Requires(name).IsNotNullOrEmpty();
            this.Name = name;
        }
        #endregion

        #region ISerializable
        protected NamingIHasIdDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Name = info.GetString("_name");
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
            info.AddValue("_name", this.Name);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasName
        public string Name { get; private set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new NamingIHasIdDecoration(thing, this.Name);
        }
        #endregion
    }

    public static partial class NameIHasIdDecorationExtensions
    {
        /// <summary>
        /// adds a name decoration to the IHasId
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NamingIHasIdDecoration AddName(this IHasId decorated, string name)
        {
            Condition.Requires(decorated).IsNotNull();
            Condition.Requires(name).IsNotNullOrEmpty();
            return new NamingIHasIdDecoration(decorated, name);
        }
    }
}
