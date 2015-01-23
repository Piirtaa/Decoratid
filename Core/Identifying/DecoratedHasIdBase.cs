using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Identifying
{
    public interface IHasIdDecoration : IHasId, IDecorationOf<IHasId> { }

    /// <summary>
    /// HasId decoration base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DecoratedHasIdBase : DecorationOfBase<IHasId>, IHasIdDecoration
    {
        #region Ctor
        public DecoratedHasIdBase(IHasId decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected DecoratedHasIdBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Overrides
        public virtual object Id
        {
            get { return this.Decorated.Id; }
        }

        public override IHasId This
        {
            get { return this; }
        }
        #endregion
    }
}
