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
    /// Implementation of HasBits that uses a BitArray as the backing store.  Also the default
    /// HasBits type for use in Decorations
    /// </summary>
    public class NaturalBitArrayHasBits : IHasBits
    {
        #region Declarations
        private BitArray _bits = null;
        #endregion

        #region Ctor
        public NaturalBitArrayHasBits(BitArray bits)
        {
            Condition.Requires(bits).IsNotNull();
            this._bits = bits;
        }
        #endregion

        #region Fluent Static
        public static NaturalBitArrayHasBits New(BitArray bits)
        {
            return new NaturalBitArrayHasBits(bits);
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

            var clone = item.Bits.Clone() as BitArray; //we have to clone because the BitArray.And() implementation changes instance state of the BitArray
            var andBits = clone.And(item.Bits);

            return new NaturalBitArrayHasBits(andBits);
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
