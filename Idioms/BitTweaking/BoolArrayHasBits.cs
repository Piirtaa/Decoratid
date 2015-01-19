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
    public class BoolArrayHasBits : IHasBits
    {
        #region Declarations
        private readonly bool[] _data = null;
        #endregion

        #region Ctor
        public BoolArrayHasBits(BitArray bits)
        {
            Condition.Requires(bits).IsNotNull();
            _data = new bool[bits.Length];
            bits.CopyTo(_data, 0);
        }
        public BoolArrayHasBits(bool[] bits)
        {
            Condition.Requires(bits).IsNotNull();
            _data = bits;
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

            var andBits = this.Bits.And(item.Bits);

            return new BoolArrayHasBits(andBits);
        }
        #endregion
    }

}
