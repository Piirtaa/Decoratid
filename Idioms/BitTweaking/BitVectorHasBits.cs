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
    public class BitVectorHasBits : IHasBits
    {
        #region Declarations

        private static readonly int[] _masks = null;
        private BitVector32 _bv;
        #endregion

        #region Ctor
        static BitVectorHasBits()
        {
            _masks = new int[32];
            _masks[0] = BitVector32.CreateMask();

            //generate a mask for each bit
            for (int i = 1; i < 32; i++)
                _masks[i] = BitVector32.CreateMask(_masks[i - 1]);
        }
        public BitVectorHasBits(BitArray bits)
        {
            Condition.Requires(bits).IsNotNull();
            _bv = new BitVector32(bits.GetIntFromBitArray());
        }
        public BitVectorHasBits(int val)
        {
            _bv = new BitVector32(val);
        }
        #endregion

        #region IHasBits
        public int BitCount { get { return 32; } }
        public BitArray Bits
        {
            get
            {
                return this._bv.Data.GetBitArrayFromInt();
            }
        }
        public void SetBit(int i, bool val)
        {
            _bv[_masks[i]] = val;
        }
        public bool? GetBit(int i)
        {
            if (i < 0 || i > 31)
                return null;

            var rv = _bv[_masks[i]];
            return rv;
        }
        public IHasBits AND(IHasBits item)
        {
            Condition.Requires(item).IsNotNull();
            var itemAsInt = item.Bits.GetIntFromBitArray();
            var andVal = this._bv.Data & itemAsInt;

            return new BitVectorHasBits(andVal);
        }
        #endregion
    }

}
