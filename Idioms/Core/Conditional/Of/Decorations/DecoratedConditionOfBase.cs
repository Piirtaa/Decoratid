using Decoratid.Idioms.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core.Conditional.Of.Decorations
{
    public interface IConditionOfDecoration<T> : IConditionOf<T>, IDecorationOf<IConditionOf<T>> { }

    public abstract class DecoratedConditionOfBase<T> : DecorationOfBase<IConditionOf<T>>, IConditionOfDecoration<T>
    {
        #region Ctor
        public DecoratedConditionOfBase(IConditionOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected DecoratedConditionOfBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Methods
        public virtual bool? Evaluate(T context)
        {
            return base.Decorated.Evaluate(context);
        }
        public override IConditionOf<T> This
        {
            get { return this; }
        }
        #endregion
    }
}
