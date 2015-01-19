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
    public class IntHasBits : IHasBits
    {
        #region Declarations
        private int _val = 0;
        #endregion

        #region Ctor
        public IntHasBits(BitArray bits)
        {
            Condition.Requires(bits).IsNotNull();
            _val = bits.GetIntFromBitArray();
        }
        public IntHasBits(int val)
        {
            Condition.Requires(val).IsGreaterOrEqual(0);
            _val = val;
        }
        #endregion

        #region IHasBits
        public int BitCount { get { return this.Bits.Length; } }
        public BitArray Bits
        {
            get
            {
                return this._val.GetBitArrayFromInt();
            }
        }
        public void SetBit(int i, bool val)
        {
            if (i < 0 || i > 31)
                throw new ArgumentOutOfRangeException("Invalid bit number");
            
            var x = this._val;

            if (val)
                x |= (int)(1 << i);
            else
                x &= ~(int)(1 << i);

            this._val = x;
        }
        public bool? GetBit(int i)
        {
            if (i < 0 || i > 31)
                return null;

            var x = this._val;

            return (x & (1 << i)) != 0;
        }
        public IHasBits AND(IHasBits item)
        {
            Condition.Requires(item).IsNotNull();
            //var andBits = item.Bits.And(item.Bits);

            if (item is IntHasBits)
            {
                var itemAsInt = item.Bits.GetIntFromBitArray();
                var andVal = this._val & (item as IntHasBits)._val;
                return new IntHasBits(andVal);
            }
            else
            {
                var itemAsInt = item.Bits.GetIntFromBitArray();
                var andVal = this._val & itemAsInt;

                return new IntHasBits(andVal);
            }
        }
        #endregion
    }
}
