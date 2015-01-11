using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.StringSearch
{
        
    /// <summary>
    /// ur std decoration interface
    /// </summary>
    public interface IStringSearcherDecoration : IStringSearcher, IDecorationOf<IStringSearcher> { }


    [Serializable]
    public abstract class StringSearcherDecorationBase : DecorationOfBase<IStringSearcher>, IStringSearcherDecoration
    {
        #region Ctor
        public StringSearcherDecorationBase(IStringSearcher decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected StringSearcherDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public override IStringSearcher This
        {
            get { return this; }
        }
        #endregion

        #region IStringSearcher

        public virtual void Add(string word, object value)
        {
            this.Decorated.Add(word, value);
        }

        public virtual List<StringSearchMatch> FindMatches(string text)
        {
            return this.Decorated.FindMatches(text);
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IStringSearcher> ApplyThisDecorationTo(IStringSearcher thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
