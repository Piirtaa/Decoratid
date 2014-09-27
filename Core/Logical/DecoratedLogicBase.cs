using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Logical
{
    public interface ILogicDecoration : ILogic, IDecorationOf<ILogic> { }

    /// <summary>
    /// base class implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DecoratedLogicBase : DecorationOfBase<ILogic>, ILogicDecoration
    {
        #region Ctor
        public DecoratedLogicBase(ILogic decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected DecoratedLogicBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

        #region Methods
        public virtual void Perform()
        {
            base.Decorated.Perform();
        }

        public override ILogic This
        {
            get { return this; }
        }
        #endregion
    }
}
