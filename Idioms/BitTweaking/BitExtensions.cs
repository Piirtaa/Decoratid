using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.BitTweaking
{
    public static class BitExtensions
    {


        //public static void SetBits(this IBitTweaking tweaker, bool val, params int[] indices)
        //{
        //    indices.WithEach(i =>
        //    {
        //        tweaker.SetBit(i, val);
        //    });
        //}
        ///// <summary>
        ///// is the bit set
        ///// </summary>
        ///// <param name="bv"></param>
        ///// <param name="i"></param>
        ///// <param name="val"></param>
        ///// <returns></returns>
        //public static bool IsBitSet(this IBitTweaking tweaker, int i, bool val)
        //{
        //    var rv = tweaker.GetBit(i)  == val;
        //    return rv;
        //}
        ///// <summary>
        ///// are all the bits set to the given value
        ///// </summary>
        ///// <param name="bv"></param>
        ///// <param name="val"></param>
        ///// <param name="indices"></param>
        ///// <returns></returns>
        //public static bool AreBitsSet(this IBitTweaking tweaker, bool val, params int[] indices)
        //{
        //    if (indices == null)
        //        return false;

        //    foreach (int i in indices)
        //    {
        //        if (!tweaker.IsBitSet(i, val))
        //            return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// at the provided positions are the bitvectors the same
        /// </summary>
        /// <param name="bv1"></param>
        /// <param name="bv2"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static bool AreEquivalentAtPositions(this IHasBits bits1, IHasBits bits2, params int[] indices)
        {
            if (indices == null)
                return false;

            foreach (int i in indices)
            {
                if (bits1.GetBit(i).Equals(bits2.GetBit(i)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// at the provided positions are the bitvectors the same
        /// </summary>
        /// <param name="bv1"></param>
        /// <param name="bv2"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static bool AreEquivalent(this IHasBits bits1, IHasBits bits2)
        {
            //null check
            if (bits1 == null)
            {
                if (bits2 == null)
                    return true;

                return false;
            }
            if (bits2 == null)
                return false;

            var l1 = bits1.BitCount;
            var l2 = bits2.BitCount;
            if (l1 != l2)
                return false;

            //foreach (int i in indices)
            //{
            //    if (bits1.GetBit(i).Equals(bits2.GetBit(i)))
            //        return false;
            //}
            return true;
        }
        //public static bool AND(this IBitTweaking tweaker, BitArray mask)
        //{
        //    if (tweaker == null)
        //        return false;

        //    var newArr = tweaker.Bits.And(mask);
        //    return newArr.Equals(mask);
        //}

    }
}