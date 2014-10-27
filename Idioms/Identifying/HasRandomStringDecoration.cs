using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Communicating;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Identifying
{

    public interface IHasRandomString
    {
        string RandomString { get; set; }
    }

    /// <summary>
    /// extends IHasId with RandomString
    /// </summary>
    [Serializable]
    public class HasRandomStringDecoration : DecoratedHasIdBase, IHasRandomString
    {


        #region Ctor
        public HasRandomStringDecoration(IHasId decorated, string randomString)
            : base(decorated)
        {
            Condition.Requires(randomString).IsNotNull();
            this.RandomString = randomString;
        }
        #endregion

        #region ISerializable
        protected HasRandomStringDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.RandomString = info.GetString("RandomString");
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RandomString", this.RandomString);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasRandomString
        public String RandomString { get; set; }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasRandomStringDecoration(thing, this.RandomString);
        }
        #endregion
    }

    public static partial class HasRandomStringDecorationExtensions
    {
        private static IRandomStringGenerator _gen = new StringGenerator();

        public static HasRandomStringDecoration HasRandomString(this IHasId decorated, string RandomString)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasRandomStringDecoration(decorated, RandomString);
        }
        public static HasRandomStringDecoration HasNewRandomString(this IHasId decorated, int length)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasRandomStringDecoration(decorated, _gen.Generate(length));
        }
    }
}
