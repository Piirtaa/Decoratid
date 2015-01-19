using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.BitTweaking
{
    /// <summary>
    /// Generates mock bitarray data
    /// </summary>
    public static class BitMocker
    {
        public static List<BitArray> GenerateRandomBitArrays(int count)
        {
            List<BitArray> rv = new List<BitArray>();

            Random rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                var itemVal = rnd.Next(int.MaxValue);
                var ba = itemVal.GetBitArrayFromInt();

                rv.Add(ba);
            }

            return rv;
        }
    }
}
