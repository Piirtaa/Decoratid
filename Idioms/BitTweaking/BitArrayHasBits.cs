using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.BitTweaking
{
    public class BitArrayHasBits : IHasBits
    {
        #region Declarations
        private BitArray _bits = null;
        #endregion

        #region Ctor
        public BitArrayHasBits(BitArray bits)
        {
            Condition.Requires(bits).IsNotNull();
            this._bits = bits;
        }
        #endregion

        #region IHasBits
        public int BitCount { get { return this._bits.Count; } }
        public BitArray Bits
        {
            get
            {
                return this._bits;
            }
        }
        public void SetBit(int i, bool val)
        {
            this._bits[i] = val;
        }
        public bool? GetBit(int i)
        {
            if (this._bits.Length <= i)
                return null;

            return this._bits[i];
        }
        public IHasBits AND(IHasBits item)
        {
            Condition.Requires(item).IsNotNull();

            var andBits = this.Bits.And(item.Bits);

            return new BitArrayHasBits(andBits);
        }
        #endregion
    }

}
