using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Sealing
{

    /// <summary>
    /// prevents further decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SealingValueOfDecoration<T> : DecoratedValueOfBase<T>
    {
        #region Ctor
        public SealingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected SealingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Methods
        public override T GetValue()
        {

            return Decorated.GetValue();

        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new SealingValueOfDecoration<T>(thing);
        }
        #endregion
    }

    public static class SealingValueOfDecorationExtensions
    {
        /// <summary>
        /// prevents further decoration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static SealingValueOfDecoration<T> Seal<T>(IValueOf<T> decorated)
        {
            return new SealingValueOfDecoration<T>(decorated);
        }
    }
}
