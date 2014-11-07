using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Polyfacing
{
    /// <summary>
    /// makes the Condition polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPolyfacingCondition : IDecoratedCondition, IPolyfacing
    {
    }

    /// <summary>
    /// decorates as polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PolyfacingConditionDecoration : DecoratedConditionBase, IPolyfacingCondition
    {
        #region Ctor
        public PolyfacingConditionDecoration(ICondition decorated, Polyface rootFace = null)
            : base(decorated)
        {
            this.RootFace = rootFace;
        }
        #endregion

        #region Fluent Static
        public static PolyfacingConditionDecoration New(ICondition decorated, Polyface rootFace = null)
        {
            return new PolyfacingConditionDecoration(decorated, rootFace);
        }
        #endregion

        #region ISerializable
        protected PolyfacingConditionDecoration(SerializationInfo info, StreamingContext context)
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
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new PolyfacingConditionDecoration(thing, this.RootFace);
        }
        #endregion
    }

    public static partial class Extensions
    {
        /// <summary>
        /// decorates with polyfacingness if it's not already there
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="rootFace"></param>
        /// <returns></returns>
        public static PolyfacingConditionDecoration Polyfacing(this ICondition decorated, Polyface rootFace = null)
        {
            Condition.Requires(decorated).IsNotNull();

            PolyfacingConditionDecoration rv = null;
            /*Summary:
             * if we spec a root we are setting that root
             * if the condition is already polyfacing we use that otherwise build new one
             * if no root is spec'd we create new polyface
             */ 

            //if we have polyface in our chain, we return that
            if (DecorationUtils.HasDecoration<PolyfacingConditionDecoration>(decorated))
            {
                rv = DecorationUtils.GetDecoration<PolyfacingConditionDecoration>(decorated);

                //if we specify a root we are replacing root!!!
                if (rootFace != null)
                {
                    rv.RootFace = rootFace;
                    return rv;
                }
            }

            if (rv == null)
            {
                Polyface poly = rootFace == null ? Polyface.New() : rootFace;
                rv = new PolyfacingConditionDecoration(decorated, poly);
            }

            return rv;
        }
    }

}
