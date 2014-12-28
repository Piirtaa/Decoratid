using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using System;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;

namespace Decoratid.Storidioms.Named
{
    [Serializable]
    public class HasIdDecoration : DecoratedStoreBase, IHasId<string>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public HasIdDecoration(IStore decorated, string id)
            : base(decorated)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
        }
        #endregion

        #region ISerializable
        protected HasIdDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Id = info.GetString("Id");
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", this.Id);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Overrides
        public override Core.Decorating.IDecorationOf<IStore> ApplyThisDecorationTo(IStore thing)
        {
            return new HasIdDecoration(thing, this.Id);
        }
        #endregion
    }

    public static class HasIdDecorationExtensions
    {
        /// <summary>
        /// gets the evicting layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static HasIdDecoration GetHasIdDecoration(this IStore decorated)
        {
            return decorated.FindDecoration<HasIdDecoration>(true);
        }
        public static HasIdDecoration HasId(this IStore decorated,
            string id)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasIdDecoration(decorated, id);
        }
        
    }
}
