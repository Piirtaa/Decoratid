using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    public static class NumbersUtil
    {
        /// <summary>
        /// Given a range of items and the number of slices, slices this up into an array of arrays
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="slices"></param>
        /// <returns></returns>
        public static int[][] GetSliceBounds(int startPos, int endPos, int slices)
        {
            Condition.Requires(startPos).IsLessThan(endPos);
            int numItems = endPos - startPos + 1;
            int stepSize = numItems / slices;
            int[][] rv = new int[slices][];

            for (int i = 0; i < slices; i++)
            {
                rv[i] = new int[2];
                rv[i][0] = startPos + i * stepSize;
                rv[i][1] = rv[i][0] + stepSize - 1;
            }
            rv[slices - 1][1] = endPos; //update the length on the last slice to account for any remainders from the initial division

            return rv;
        }
    }
}
