
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core.Conditional
{
    public interface IConditionDecoration : ICondition, IDecorationOf<ICondition> { }

    public abstract class DecoratedConditionBase : DecorationOfBase<ICondition>, IConditionDecoration
    {
        #region Ctor
        public DecoratedConditionBase(ICondition decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected DecoratedConditionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Methods
        public virtual bool? Evaluate()
        {
            return base.Decorated.Evaluate();
        }
        public override ICondition This
        {
            get { return this; }
        }
        #endregion
    }
}
