using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{
    public interface IHasExpiryCondition
    {
        ICondition ExpiryCondition { get; }
    }
    public interface IHasSettableExpiryCondition
    {
        void SetExpiryCondition(ICondition cond);
    }

    /// <summary>
    /// implements expiry by a condition
    /// </summary>
    [Serializable]
    public class ConditionalExpirableDecoration : ExpirableDecorationBase, IHasExpiryCondition, IHasSettableExpiryCondition
    {
        #region Ctor
        public ConditionalExpirableDecoration(IExpirable decorated, ICondition expiryCondition)
            : base(decorated)
        {
            this.ExpiryCondition = expiryCondition;
        }
        #endregion

        #region ISerializable
        protected ConditionalExpirableDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ExpiryCondition = (ICondition)info.GetValue("ExpiryCondition", typeof(ICondition));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ExpiryCondition", this.ExpiryCondition);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasExpiryDate
        public ICondition ExpiryCondition { get; private set; }
        #endregion

        #region IHasSettableExpiryDate
        public void SetExpiryCondition(ICondition cond)
        {
            this.ExpiryCondition = cond;
        }
        #endregion

        #region Overrides
        public override bool IsExpired()
        {
            var rv= this.ExpiryCondition.Evaluate().GetValueOrDefault();
            return rv;
        }
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
        {
            return new ConditionalExpirableDecoration(thing, this.ExpiryCondition);
        }
        #endregion

        //#region IDecorationHydrateable
        //public override string DehydrateDecoration(IGraph uow = null)
        //{
        //    return this.ExpiryDate.ToUnixTime().ToString();
        //}
        //public override void HydrateDecoration(string text, IGraph uow = null)
        //{
        //    var unixTime = long.Parse(text);
        //    this.ExpiryDate = unixTime.FromUnixTime();
        //}
        //#endregion
    }

    public static class ConditionalExpirableDecorationExtensions
    {
        public static ConditionalExpirableDecoration DecorateWithConditionalExpirable(this IExpirable thing, ICondition condition)
        {
            Condition.Requires(thing).IsNotNull();
            return new ConditionalExpirableDecoration(thing, condition);
        }
        public static IHasExpirable When(this IHasExpirable thing, ICondition condition)
        {
            Condition.Requires(thing).IsNotNull();
            thing.Expirable = thing.Expirable.DecorateWithConditionalExpirable(condition);

            return thing;
        }
    }
}
