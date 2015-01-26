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
    /// HasBits implementation that uses an array of bool as the backing store.  
    /// </summary>
    public class NaturalBoolArrayHasBits : IHasBits
    {
        #region Declarations
        private readonly bool[] _data = null;
        #endregion

        #region Ctor
        public NaturalBoolArrayHasBits(BitArray bits)
        {
            Condition.Requires(bits).IsNotNull();
            _data = new bool[bits.Length];
            bits.CopyTo(_data, 0);
        }
        public NaturalBoolArrayHasBits(bool[] bits)
        {
            Condition.Requires(bits).IsNotNull();
            _data = bits;
        }
        #endregion

        #region Fluent Static
        public static NaturalBoolArrayHasBits New(BitArray bits)
        {
            return new NaturalBoolArrayHasBits(bits);
        }
        public static NaturalBoolArrayHasBits New(bool[] bits)
        {
            return new NaturalBoolArrayHasBits(bits);
        }
        #endregion

        #region IHasBits
        public int BitCount { get { return this._data.Length; } }
        public BitArray Bits
        {
            get
            {
                return new BitArray(this._data);
            }
        }
        public void SetBit(int i, bool val)
        {
            this._data[i] = val;
        }
        public bool? GetBit(int i)
        {
            if (this._data.Length <= i)
                return null;

            return this._data[i];
        }
        public IHasBits AND(IHasBits item)
        {
            Condition.Requires(item).IsNotNull();

            var clone = this.Bits.Clone() as BitArray;

            var andBits = clone.And(item.Bits);

            return new NaturalBoolArrayHasBits(andBits);
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
