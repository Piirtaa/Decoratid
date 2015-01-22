using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.HasBitsing
{
    /// <summary>
    /// puts type label on things that implement both IHasId and IHasBits
    /// </summary>
    public interface IHasBitsHasId : IHasId, IHasBits
    {
    }

    /// <summary>
    /// provides bitarray feature to IHasId instances
    /// </summary>
    [Serializable]
    public sealed class HasBitsIHasIdDecoration : DecoratedHasIdBase, IHasBitsHasId
    {
        #region Declarations
        private IHasBits _hasBits; //uses backing field to wrap
        #endregion

        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="hasBits">if null will default to 32bit BitArrayHasBits instance</param>
        public HasBitsIHasIdDecoration(IHasId decorated, IHasBits hasBits = null)
            : base(decorated)
        {
            if (hasBits != null)
            {
                this._hasBits = hasBits;
            }
            else
            {
                this._hasBits = NaturalBitArrayHasBits.New(new BitArray(32));
            }
        }
        #endregion

        #region ISerializable
        protected HasBitsIHasIdDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._hasBits = NaturalBitArrayHasBits.New(new BitArray(info.GetValue("_hasBits", typeof(bool[])) as bool[]));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_hasBits", this._hasBits.Bits.GetBoolArrayFromBitArray(), typeof(bool[]));
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties

        #endregion

        #region IHasBits
        BitArray IHasBits.Bits { get { return this._hasBits.Bits; } }
        int IHasBits.BitCount { get { return this._hasBits.BitCount; } }
        void IHasBits.SetBit(int i, bool val) { this._hasBits.SetBit(i, val); }
        bool? IHasBits.GetBit(int i) { return this._hasBits.GetBit(i); }
        IHasBits IHasBits.AND(IHasBits bits) { return this._hasBits.AND(bits); }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new HasBitsIHasIdDecoration(thing, this._hasBits);
        }
        #endregion
    }

    public static partial class HasBitsIHasIdDecorationExtensions
    {
        /// <summary>
        /// decorates with bits
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static HasBitsIHasIdDecoration HasBits(this IHasId decorated, IHasBits bits = null)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasBitsIHasIdDecoration(decorated, bits);
        }
    }
}
