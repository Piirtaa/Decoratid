using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Polyfacing
{
    /// <summary>
    /// makes the logic polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPolyfacingLogic : IDecoratedLogic, IPolyfacing
    {
    }

    /// <summary>
    /// decorates as polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PolyfacingLogicDecoration : DecoratedLogicBase, IPolyfacingLogic
    {
        #region Ctor
        public PolyfacingLogicDecoration(ILogic decorated, Polyface rootFace = null)
            : base(decorated)
        {
            //if no polyface is set we create new one
            this.RootFace = (rootFace == null) ? Polyface.New() : rootFace;
            //register the face
            this.RootFace.Is(this);
        }
        #endregion

        #region Fluent Static
        public static PolyfacingLogicDecoration New(ILogic decorated, Polyface rootFace = null)
        {
            return new PolyfacingLogicDecoration(decorated, rootFace);
        }
        #endregion

        #region ISerializable
        protected PolyfacingLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new PolyfacingLogicDecoration(thing, this.RootFace);
        }
        #endregion
    }

    public static partial class Extensions
    {
        /// <summary>
        /// decorates with polyfacingness if it's not already there
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logic"></param>
        /// <param name="rootFace"></param>
        /// <returns></returns>
        public static PolyfacingLogicDecoration Polyfacing(this ILogic decorated, Polyface rootFace = null)
        {
            Condition.Requires(decorated).IsNotNull();

            PolyfacingLogicDecoration rv = null;
            /*Summary:
             * if we spec a root we are setting a face on that root, else we are using a new 
             * if the condition is already polyfacing we use that otherwise build new one
             * if no root is spec'd we create new polyface
             */

            //if we have polyface in our chain, we return that
            if (DecorationUtils.HasDecoration<PolyfacingLogicDecoration>(decorated))
            {
                rv = DecorationUtils.GetDecoration<PolyfacingLogicDecoration>(decorated);
            }
            else
            {
                rv = new PolyfacingLogicDecoration(decorated, rootFace);
            }

            return rv;
        }
    }

}
