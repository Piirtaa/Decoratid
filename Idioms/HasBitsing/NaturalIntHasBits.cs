using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.HasBitsing
{
    /// <summary>
    /// HasBits implementation that uses an int32 as the backing store of bits.  Limited to 32 bits.
    /// </summary>
    public class NaturalIntHasBits : IHasBits
    {
        #region Declarations
        private int _val = 0;
        #endregion

        #region Ctor
        public NaturalIntHasBits(BitArray bits)
        {
            Condition.Requires(bits).IsNotNull();
            _val = bits.GetIntFromBitArray();
        }
        public NaturalIntHasBits(int val)
        {
            Condition.Requires(val).IsGreaterOrEqual(0);
            _val = val;
        }
        #endregion

        #region Fluent Static
        public static NaturalIntHasBits New(BitArray bits)
        {
            return new NaturalIntHasBits(bits);
        }
        public static NaturalIntHasBits New(int val)
        {
            return new NaturalIntHasBits(val);
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

            if (item is NaturalIntHasBits)
            {
                var itemAsInt = item.Bits.GetIntFromBitArray();
                var andVal = this._val & (item as NaturalIntHasBits)._val;
                return new NaturalIntHasBits(andVal);
            }
            else
            {
                var itemAsInt = item.Bits.GetIntFromBitArray();
                var andVal = this._val & itemAsInt;

                return new NaturalIntHasBits(andVal);
            }
        }
        #endregion

        #region ToString Overrides
 
        public override string ToString()
        {
            return this.Bits.ToHumanReadable();
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        #endregion
    }
}
