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

namespace Decoratid.Idioms.TokenParsing.HasComposite
{
    /// <summary>
    /// a composite token aggregates a bunch of other tokens
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasCompositeToken<T> : IToken<T>
    {
        IToken<T>[] ChildTokens { get; }
    }

    /// <summary>
    /// decorates with composite
    /// </summary>
    [Serializable]
    public class HasCompositeTokenDecoration<T> : TokenDecorationBase<T>, IHasCompositeToken<T>
    {
        #region Ctor
        public HasCompositeTokenDecoration(IToken<T> decorated, IToken<T>[] childTokens)
            : base(decorated)
        {
            this.ChildTokens = childTokens;
        }
        #endregion

        #region Fluent Static
        public static HasCompositeTokenDecoration<T> New(IToken<T> decorated, IToken<T>[] childTokens)
        {
            return new HasCompositeTokenDecoration<T>(decorated, childTokens);
        }
        #endregion

        #region ISerializable
        protected HasCompositeTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public IToken<T>[] ChildTokens { get; private set;}
        #endregion

        #region Overrides
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasCompositeTokenDecoration<T>(thing, this.ChildTokens);
        }
        #endregion
    }

    public static class HasCompositeTokenDecorationExtensions
    {
        public static HasCompositeTokenDecoration<T> HasComposite<T>(this IToken<T> thing, IToken<T>[] childTokens)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasCompositeTokenDecoration<T>(thing, childTokens);
        }

    }
}
