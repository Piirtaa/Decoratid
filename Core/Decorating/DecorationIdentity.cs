using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Decorating
{
    /// <summary>
    /// metadata about a decoration that is convertible to string.  we use this as the identity container for
    /// decorations.  It helps conform ids so that they can't be spoofed.
    /// </summary>
    [Serializable]
    public class DecorationIdentity : IHasId<string>
    {
        #region Ctor
        public DecorationIdentity(object obj)
        {
            Condition.Requires(obj).IsNotNull();
            if (!obj.IsADecoration())
                throw new ArgumentOutOfRangeException("obj is not a decoration");

            this.DecorationType = obj.GetType();
            this.DecoratedType = obj.GetTypeBeingDecorated();
            this.Id = string.Empty;
        }
        public DecorationIdentity(Type decorationType, Type decoratedType, string id)
        {
            Condition.Requires(decorationType).IsNotNull();
            Condition.Requires(decoratedType).IsNotNull();
            this.DecorationType = decorationType;
            this.DecoratedType = decoratedType;
            this.Id = id;
        }
        #endregion

        #region Fluent Static
        public static DecorationIdentity New(object obj)
        {
            return new DecorationIdentity(obj);
        }
        public static DecorationIdentity New(Type type, Type decoratedType, string id)
        {
            return new DecorationIdentity(type, decoratedType, id);
        }
        #endregion

        #region IHasId
        public string Id { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Properties
        private Type DecorationType { get; set; }
        private Type DecoratedType { get; set; }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.DecorationType.FullName, this.DecoratedType.FullName, this.Id);
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj.GetType().Equals(this.GetType())))
                return false;

            return obj.ToString().Equals(this.ToString());
        }
        #endregion
    }
}
