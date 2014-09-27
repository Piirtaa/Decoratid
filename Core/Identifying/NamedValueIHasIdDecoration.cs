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
    public interface IHasNameValue
    {
        string Name { get; }
        object Value { get; }
    }

    /// <summary>
    /// extends IHasId with some extra data
    /// </summary>
    [Serializable]
    public class NamedValueIHasIdDecoration : DecoratedHasIdBase, IHasNameValue
    {
        #region Ctor
        public NamedValueIHasIdDecoration(IHasId decorated, string name, object val)
            : base(decorated)
        {
            Condition.Requires(name).IsNotNullOrEmpty();
            this.Name = name;
            this.Value = val;
        }
        #endregion

        #region ISerializable
        protected NamedValueIHasIdDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Name = info.GetString("_name");
            bool isNull = info.GetBoolean("_isNull");

            if (!isNull)
            {
                Type type = (Type)info.GetValue("_type", typeof(Type));
                this.Value = info.GetValue("_value", type);
            }
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

            bool isNull = this.Value == null;
            info.AddValue("_isNull", isNull);

            if (!isNull)
            {
                info.AddValue("_type", this.Value.GetType());
                info.AddValue("_value", this.Value);
            }
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasNameValue
        public string Name { get; private set; }
        public object Value { get; private set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new NamedValueIHasIdDecoration(thing, this.Name, this.Value);
        }
        #endregion
    }

    public static partial class NamedValueDecorationExtensions
    {
        /// <summary>
        /// adds a name value decoration to the IHasId
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NamedValueIHasIdDecoration AddProperty(this IHasId decorated, string name, object value)
        {
            Condition.Requires(decorated).IsNotNull();
            Condition.Requires(name).IsNotNullOrEmpty();
            return new NamedValueIHasIdDecoration(decorated, name, value);
        }
    }
}
