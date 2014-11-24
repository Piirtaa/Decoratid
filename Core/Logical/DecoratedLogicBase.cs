using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Logical
{
    public interface IDecoratedLogic : ILogic, IDecorationOf<ILogic> { }

    /// <summary>
    /// base class implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DecoratedLogicBase : DecorationOfBase<ILogic>, IDecoratedLogic
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
        public virtual ILogic Perform(object context = null)
        {
            var rv = base.Decorated.Perform(context);
            return rv;
        }
        /// <summary>
        /// the default implementation of this invokes ApplyThisDecorationTo its own Decorated
        /// </summary>
        /// <returns></returns>
        public ILogic Clone()
        {
            return this.ApplyThisDecorationTo(this.Decorated) as ILogic;
        }        
        public override ILogic This
        {
            get { return this; }
        }
        #endregion



    }
}
