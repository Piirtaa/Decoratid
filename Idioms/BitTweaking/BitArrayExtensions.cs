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
    public static class BitArrayExtensions
    {
        public static string ToHumanReadable(this BitArray ba)
        {
            //null check
            if (ba == null)
                return null;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ba.Length; i++)
                sb.Append(ba[i] ? "1" : "0");

            return sb.ToString();
        }
        public static int GetIntFromBitArray(this BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }

        ///// <summary>
        ///// converts a bitarray to an int.  bit array can't be longer than 32
        ///// </summary>
        ///// <param name="bitArray"></param>
        ///// <returns></returns>
        ///// <remarks>http://stackoverflow.com/questions/5283180/how-i-can-convert-bitarray-to-single-int</remarks>
        //public static int GetIntFromBitArray(this BitArray bitArray)
        //{
        //    int value = 0;

        //    if (bitArray.Length > 32)
        //        throw new ArgumentException("Argument length shall be at most 32 bits.");

        //    for (int i = 0; i < bitArray.Count; i++)
        //    {
        //        if (bitArray[i])
        //            value += Convert.ToInt16(Math.Pow(2, i));
        //    }

        //    return value;
        //}

        public static BitArray GetBitArrayFromInt(this int x)
        {
            int[] arr = new int[1];
            arr[0] = x;
            return new BitArray(arr);
        }

        /// <summary>
        /// tests equivalence by XORing and checking for 1s.  XOR returns true if 2 bits differ
        /// </summary>
        /// <param name="ba1"></param>
        /// <param name="ba2"></param>
        /// <returns></returns>
        public static bool AreEquivalent_XORCompare(this BitArray ba1, BitArray ba2)
        {
            //null check
            if (ba1 == null)
            {
                if (ba2 == null)
                    return true;

                return false;
            }
            if (ba2 == null)
                return false;

            //do an XOR.  if any 1s appear we have a mismatch
            BitArray clone = ba1.Clone() as BitArray;
            BitArray check = clone.Xor(ba2);
            foreach (bool each in check)
                if (each)
                    return false;

            return true;
        }
        /// <summary>
        /// tests equivalence by bitwise comparison
        /// </summary>
        /// <param name="ba1"></param>
        /// <param name="ba2"></param>
        /// <returns></returns>
        public static bool AreEquivalent_BitByBit(this BitArray ba1, BitArray ba2)
        {
            //null check
            if (ba1 == null)
            {
                if (ba2 == null)
                    return true;

                return false;
            }
            if (ba2 == null)
                return false;

            int l1 = ba1.Length;
            int l2 = ba2.Length;

            if (l1 != l2)
                return false;

            for (int i = 0; i < l1; i++)
                if (!ba1[i].Equals(ba2[i]))
                {
                    var s1 = ToHumanReadable(ba1);
                    var s2 = ToHumanReadable(ba2);
                    return false;

                }
            return true;
        }
        /// <summary>
        /// tests equivalence by converting to int and comparing
        /// </summary>
        /// <param name="ba1"></param>
        /// <param name="ba2"></param>
        /// <returns></returns>
        public static bool AreEquivalent_IntCompare(this BitArray ba1, BitArray ba2)
        {
            return ba1.GetIntFromBitArray().Equals(ba2.GetIntFromBitArray());
        }

       
    }
}