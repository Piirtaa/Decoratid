using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;


namespace Decoratid.Idioms.TokenParsing.HasComment
{
    public interface IHasCommentToken<T> : IToken<T>
    {
        string Comment { get; }
    }

    /// <summary>
    /// decorates with a value
    /// </summary>
    [Serializable]
    public class HasCommentTokenDecoration<T> : TokenDecorationBase<T>, IHasCommentToken<T>
    {
        #region Ctor
        public HasCommentTokenDecoration(IToken<T> decorated, string comment)
            : base(decorated)
        {
            Condition.Requires(comment).IsNotNullOrEmpty();
            this.Comment = comment;
        }
        #endregion

        #region Fluent Static
        public static HasCommentTokenDecoration<T> New(IToken<T> decorated, string comment)
        {
            return new HasCommentTokenDecoration<T>(decorated, comment);
        }
        #endregion

        #region ISerializable
        protected HasCommentTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public string Comment { get; private set; }

        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasCommentTokenDecoration<T>(thing, this.Comment);
        }
        #endregion
    }

    public static class HasCommentTokenDecorationExtensions
    {
        public static HasCommentTokenDecoration<T> HasComment<T>(this IToken<T> thing, string comment)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasCommentTokenDecoration<T>(thing, comment);
        }
    }
}
