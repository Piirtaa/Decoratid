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
    public interface IPolyfacingLogic : ILogicDecoration, IPolyfacing
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
            this.RootFace = rootFace;
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
        public static PolyfacingLogicDecoration Polyfacing<T>(this ILogic logic, Polyface rootFace = null)
        {
            Condition.Requires(logic).IsNotNull();

            if (logic is PolyfacingLogicDecoration)
            {
                var pf = logic as PolyfacingLogicDecoration;
                return pf;
            }

            return new PolyfacingLogicDecoration(logic, rootFace);
        }
    }

}
