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
using Decoratid.Idioms.TokenParsing.CommandLine.Compiling;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Evaluating
{

    /// <summary>
    /// decorates a token with a value
    /// </summary>
    public interface IHasValueToken : IStringToken, ICanEval
    {
        object Value { get; }
    }

    /// <summary>
    /// decorates with a value
    /// </summary>
    [Serializable]
    public class HasValueTokenDecoration : TokenDecorationBase, IHasValueToken
    {
        #region Ctor
        public HasValueTokenDecoration(IStringToken decorated, object value)
            : base(decorated)
        {
            this.Value = value;
        }
        #endregion

        #region Fluent Static
        public static HasValueTokenDecoration New(IStringToken decorated, object value)
        {
            return new HasValueTokenDecoration(decorated, value);
        }
        #endregion

        #region ISerializable
        protected HasValueTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public object Value { get; private set; }
        public object Evaluate() { return this.Value; }

        public override IDecorationOf<IStringToken> ApplyThisDecorationTo(IStringToken thing)
        {
            return new HasValueTokenDecoration(thing, this.Value);
        }
        #endregion
    }

    public static class HasValueTokenDecorationExtensions
    {
        public static HasValueTokenDecoration HasValue(this IStringToken thing, object value)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasValueTokenDecoration(thing, value);
        }
    }
}
