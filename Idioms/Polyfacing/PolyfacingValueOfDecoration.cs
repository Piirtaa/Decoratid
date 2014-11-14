using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Polyfacing
{
    /// <summary>
    /// makes the valueof polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPolyfacingValueOf<T> : IDecoratedValueOf<T>, IPolyfacing
    {
    }

    /// <summary>
    /// decorates as polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PolyfacingValueOfDecoration<T> : DecoratedValueOfBase<T>, IPolyfacingValueOf<T>
    {
        #region Ctor
        public PolyfacingValueOfDecoration(IValueOf<T> decorated, Polyface rootFace = null)
            : base(decorated)
        {
            //if no polyface is set we create new one
            this.RootFace = (rootFace == null) ? Polyface.New() : rootFace;
            //register the face
            this.RootFace.Is(this);
        }
        #endregion

        #region Fluent Static
        public static PolyfacingValueOfDecoration<T> New(IValueOf<T> decorated, Polyface rootFace = null)
        {
            return new PolyfacingValueOfDecoration<T>(decorated, rootFace);
        }
        #endregion

        #region ISerializable
        protected PolyfacingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.RootFace = (Polyface)info.GetValue("RootFace", typeof(Polyface));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RootFace", this.RootFace);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IPolyfacing
        public Polyface RootFace { get; set; }
        #endregion

        #region Methods
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new PolyfacingValueOfDecoration<T>(thing, this.RootFace);
        }
        #endregion
    }

    public static partial class Extensions
    {
        /// <summary>
        /// decorates with polyfacingness if it's not already there
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="rootFace"></param>
        /// <returns></returns>
        public static PolyfacingValueOfDecoration<T> Polyfacing<T>(this IValueOf<T> decorated, Polyface rootFace = null)
        {
            Condition.Requires(decorated).IsNotNull();

            PolyfacingValueOfDecoration<T> rv = null;

            //if we have polyface in our chain, we return that
            if (DecorationUtils.HasDecoration<PolyfacingValueOfDecoration<T>>(decorated))
            {
                rv = DecorationUtils.GetDecoration<PolyfacingValueOfDecoration<T>>(decorated);
            }
            else
            {
                rv = new PolyfacingValueOfDecoration<T>(decorated, rootFace);
            }

            return rv;
        }
    }


}
