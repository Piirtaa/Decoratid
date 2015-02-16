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

namespace Decoratid.Idioms.TokenParsing
{
    public interface IHasPrefixToken : IToken
    {
        string Prefix { get; }
    }

    /// <summary>
    /// decorates with prefix
    /// </summary>
    [Serializable]
    public class HasPrefixTokenDecoration : TokenDecorationBase, IHasPrefixToken
    {
        #region Ctor
        public HasPrefixTokenDecoration(IToken decorated, string prefix)
            : base(decorated)
        {
            this.Prefix = prefix;
        }
        #endregion

        #region Fluent Static
        public static HasPrefixTokenDecoration New(IToken decorated, string prefix)
        {
            return new HasPrefixTokenDecoration(decorated, prefix);
        }
        #endregion

        #region ISerializable
        protected HasPrefixTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public string Prefix { get; private set; }
        public override string TokenString
        {
            get
            {
                //remove the prefix
                var rv = base.TokenString;
                rv = rv.Remove(0, this.Prefix.Length);
                return  rv;
            }
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IToken> ApplyThisDecorationTo(IToken thing)
        {
            return new HasPrefixTokenDecoration(thing, this.Prefix);
        }
        #endregion
    }

    public static class HasPrefixTokenDecorationExtensions
    {
        public static HasPrefixTokenDecoration HasPrefix(this IToken thing, string prefix)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasPrefixTokenDecoration(thing, prefix);
        }
    }
}
