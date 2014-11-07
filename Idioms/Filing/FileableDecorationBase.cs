using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Filing
{
    public interface IFileableDecoration : IFileable, IDecorationOf<IFileable> { }

    /// <summary>
    /// base class implementation of a IFileable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class FileableDecorationBase : DecorationOfBase<IFileable>, IFileable
    {
        #region Ctor
        public FileableDecorationBase(IFileable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected FileableDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IFileable
        public virtual void Read()
        {
            base.Decorated.Read();
        }
        public virtual void Write()
        {
            base.Decorated.Write();
        }
        #endregion

        #region IStringable
        public string GetValue()
        {
            return this.Decorated.GetValue();
        }
        public void Parse(string text)
        {
            this.Decorated.Parse(text);
            this.Write();
        }
        #endregion

        #region Overrides
        public override IFileable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IFileable> ApplyThisDecorationTo(IFileable thing)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
