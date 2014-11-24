using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;
using Decoratid.Idioms.ObjectGraphing;

namespace Decoratid.Idioms.Smellable
{

    [Serializable]
    public class SmellableLogicDecoration : DecoratedLogicBase, ISmellable
    {
        #region Ctor
        public SmellableLogicDecoration(ILogic decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected SmellableLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region ISmellable
        public bool? SmellsGood()
        {
            //if it can't be serialized back to itself it's a problem
            try
            {
                string dat = this.GraphSerializeWithDefaults();
                var obj = dat.GraphDeserializeWithDefaults();
                string dat2 = obj.GraphSerializeWithDefaults();
                return dat.Equals(dat2);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Methods
        public override ILogic Perform(object context = null)
        {
            this.SmellCheck();

            var rv = Decorated.Perform(context);
            return rv;
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new SmellableLogicDecoration(thing);
        }
        #endregion
    }

    public static class SmellableLogicDecorationExtensions
    {
        public static SmellableLogicDecoration IsSmellable(this ILogic decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new SmellableLogicDecoration(decorated);
        }
    }
}
