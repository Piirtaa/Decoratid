using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.HasBitsing
{
    /// <summary>
    /// implements bit operations on ints
    /// </summary>
    public static class IntBitExtensions
    {

        //inspired by: http://www.catonmat.net/blog/low-level-bit-hacks-you-absolutely-must-know/

        public static bool IsEvenNumber(this int val)
        {
            return ((val & 1) == 0);
        }
        public static bool GetNthBit(this int val, int n)
        {
            return (val & (1 << n)) != 0;
        }
        
        /// <summary>
        /// if null, will toggle that bit
        /// </summary>
        /// <param name="val"></param>
        /// <param name="n"></param>
        /// <param name="bit">if null, will toggle that bit</param>
        public static int SetNthBit(this int val, int n, bool? bit)
        {
            int rv;

            if (bit.HasValue)
            {
                if (bit.Value)
                {
                    rv = val | (int)(1 << n);
                }
                else
                {
                    rv = val & ~(int)(1 << n);
                }
            }
            else
            {
                rv = val ^ (int)(1 << n);
            }
            return rv;
        }
        /// <summary>
        /// finds rightmost ON bit and turns it off
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        ///<remarks>
        /// can translate to turn off first decoration
        /// </remarks>
        public static int TurnOffRightmostOnBit(this int val)
        {
            int rv = val & (int)(val - 1);
            return rv;
        }
        /// <summary>
        /// turns all OFF but the rightmost ON bit
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        ///<remarks>
        ///can translate to find first decoration
        /// </remarks>
        public static int IsolateRightmostOnBit(this int val)
        {
            int rv = val & (int)(-val);
            return rv;
        }
        /// <summary>
        /// Turns all OFF except the rightmost OFF bit, which it flips ON
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        public static int IsolateRightmostOffBit(this int val)
        {
            int rv = ~val & (int)(val + 1);
            return rv;
        }
        /// <summary>
        /// finds the rightmost ON bit and turns everything to the right of this ON
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        /// <remarks>
        /// can translate to enabling/disabling a stack of decorations
        /// </remarks>
        public static int TurnOnAfterRightmostOnBit(this int val)
        {
            int rv = val | (int)(val - 1);
            return rv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int TurnOnRightmostOffBit(this int val)
        {
            int rv = val | (int)(val + 1);
            return rv;
        }
    }
}