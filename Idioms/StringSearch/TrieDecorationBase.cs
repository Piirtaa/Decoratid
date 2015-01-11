using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.StringSearch
{
        
    /// <summary>
    /// ur std decoration interface
    /// </summary>
    public interface ITrieDecoration : ITrie, IDecorationOf<ITrie> { }


    [Serializable]
    public abstract class TrieDecorationBase : DecorationOfBase<ITrie>, ITrieDecoration
    {
        #region Ctor
        public TrieDecorationBase(ITrie decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected TrieDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public override ITrie This
        {
            get { return this; }
        }
        #endregion

        #region ITrie
        public ITrieNode Root { get { return this.Decorated.Root; } }
        public virtual void Add(string word, object value)
        {
            this.Decorated.Add(word, value);
        }
        public ITrieNode this[string path] { get { return this.Decorated[path]; } set { this.Decorated[path] = value; } }
        public virtual List<StringSearchMatch> FindMatches(string text)
        {
            return this.Decorated.FindMatches(text);
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<ITrie> ApplyThisDecorationTo(ITrie thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
