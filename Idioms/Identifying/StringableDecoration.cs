using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.ObjectGraphing;
using Decoratid.Idioms.ObjectGraphing.Values;
using System.IO;

namespace Decoratid.Idioms.Identifying
{
    /// <summary>
    /// serializes an IHasId using ObjectGraph. 
    /// </summary>
    /// <remarks>
    /// Provides us a gateway for serializing state of the object and using the stringable decoration tree. For example
    /// we can provide a backing file to write state to.
    /// </remarks>
    [Serializable]
    public class StringableDecoration : DecoratedHasIdBase, IStringable
    {
        #region Ctor
        public StringableDecoration(IHasId decorated)
            : base(decorated)
        {

        }
        #endregion

        #region ISerializable
        protected StringableDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

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
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IStringable
        public string GetValue()
        {
            var rv= this.GraphSerializeWithDefaults();
            var data = LengthEncoder.MakeReadable(rv, "\t");
            File.AppendAllText("stringabledump.txt", data + Environment.NewLine);

            return rv;
        }

        public void Parse(string text)
        {
            var obj = text.GraphDeserializeWithDefaults();
            Condition.Requires(obj).IsNotNull().IsOfType(typeof(DecoratedHasIdBase));
            DecoratedHasIdBase decObj = obj as DecoratedHasIdBase;

            //replace the decorated instance with the decObj
            this.ReplaceDecorated((x) => { return decObj; });
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new StringableDecoration(thing);
        }
        #endregion
    }

    public static partial class StringableDecorationExtensions
    {
        public static StringableDecoration Stringable(this IHasId decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new StringableDecoration(decorated);
        }
    }
}
