using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.TypeLocating
{
    public interface ITypeLocatorDecoration : ITypeLocator, IDecorationOf<ITypeLocator> { }

    [Serializable]
    public abstract class TypeLocatorDecorationBase : DecorationOfBase<ITypeLocator>, ITypeLocator
    {
        #region Ctor
        public TypeLocatorDecorationBase(ITypeLocator decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected TypeLocatorDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public virtual List<Type> Locate(Func<Type, bool> filter)
        {
            return Decorated.Locate(filter);
        }
        public override ITypeLocator This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<ITypeLocator> ApplyThisDecorationTo(ITypeLocator thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
