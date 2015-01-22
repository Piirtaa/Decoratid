using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.Indexing
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexMap
    {
        /*
         *Test the different ways to bitwise search large sets
         *Way 1.
         *  Generate BitArrays, key each line to a value (key value lookup)
         *  Generate TestMask
         *  Apply testmask to each line, report matches
         *  
         * 
         */
 
        /*
         *The format of an index line will be like a binary string "10111001" where each
         *position (leftmost is 0th, as with a string) corresponds to an index
         * 
         */

        /// <summary>
        /// is functionally the same as 
        /// this pseudocode: line[tupleN.1] == (bool)tupleN.2 AND line[tupleN+1].1 == (bool)tupleN+1.2 ...
        /// </summary>
        /// <param name="line"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool CreateFilterFunctionFromMask(string line, params Tuple<int, bool>[] filter)
        {
            if (filter == null)
                return false;

            foreach (var each in filter)
            {
                var c = line[each.Item1];
                bool isOn = each.Item2;
                if (isOn == true && c == '1')
                {
                    continue;
                }
                else if (isOn == false && c == '0')
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexLines"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<int> Matches(List<string> indexLines, params Tuple<int, bool>[] filter)
        {

        }
    }
}
