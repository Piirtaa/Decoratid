using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// defines a stringable decoration.
    /// NOTE: the decoration hydration mechanism SHOULD enable conversion to and from the raw value.
    /// For example, Hydrate() should take in the encoded value, and the raw value would 
    /// Dehydrate() should output the encoded value.
    /// </summary>
    public interface IStringableDecoration : IStringable, IDecorationOf<IStringable> { }

    /// <summary>
    /// base class implementation of a IStringable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class StringableDecorationBase : DecorationOfBase<IStringable>, IStringable
    {
        #region Ctor
        public StringableDecorationBase(IStringable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected StringableDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var data = info.GetString("data");
            this.Parse(data);
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", this.GetValue());
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public virtual string GetValue()
        {
            return base.Decorated.GetValue();
        }
        public virtual void Parse(string text)
        {
            base.Decorated.Parse(text);
        }
        public override IStringable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IStringable> ApplyThisDecorationTo(IStringable thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
