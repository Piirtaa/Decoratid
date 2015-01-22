using Decoratid.Core.Decorating;
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
    /// a decoration of IHasBits
    /// </summary>
    public interface IHasBitsDecoration : IHasBits, IDecorationOf<IHasBits> { }

    /// <summary>
    /// base class implementation for hasbits decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DecoratedHasBitsBase : DecorationOfBase<IHasBits>, IHasBitsDecoration
    {
        #region Ctor
        public DecoratedHasBitsBase(IHasBits decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected DecoratedHasBitsBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region IDecorationOf
        public override IHasBits This
        {
            get { return this; }
        }
        #endregion

        #region IHasBits
        public virtual BitArray Bits { get { return this.Decorated.Bits; } }
        public virtual int BitCount { get { return this.Decorated.BitCount; } }
        public virtual void SetBit(int i, bool val) { this.Decorated.SetBit(i, val); }
        public virtual bool? GetBit(int i) { return this.Decorated.GetBit(i); }
        public virtual IHasBits AND(IHasBits bits) { return this.Decorated.AND(bits); }
        #endregion
    }
}
