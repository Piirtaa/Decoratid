using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    /*
     * models a fibonacci process as items that can spawn themselves at most 2 times, in a sequence.
     * 
     * Fibonacci is a scaling alg, imo.  Trying to model this as a parent/child problem, wherein
     * each node can spawn itself twice.  
     * 
     * Generation(N) = Generation(N-1) + Generation(N-2)
     * As the list grows, the number of slots needed scales according to fibonacci.
     * 
     * Is just like a list, where as u go farther ahead in the list, more slots need to be created.
     * Generation 0 - array is empty
     * Generation 1 - 
     *  calculate Fib# - last (0) + current (1) = 1 slot needed
     *              resize array to new items
     *              
     *              for (int i= old count, i< old count + fib#)
     *              
     *              array[0].Generation = generation # (1)
     *              array[0].Index = i;
     *              
     * 
     */ 
    public class FibonacciList<T>
    {
        #region Inner Class
        private class FibItem
        {
            
            public int Generation;
            public int Index;
            public T Value;
            public FibItem Spawn1;
            public FibItem Spawn2;

            
        }
        #endregion

        #region Declarations
        private readonly object _stateLock = new object();
        private FibItem _root;
        private int _fibGeneration =0;
        private int _nextIndex = 1;
        #endregion


        #region Linked List walking
        private FibItem Walk(Func<FibItem, bool> stopWhen)
        {
            FibItem rv = _root;

            while (rv != null)
            {
                if (stopWhen(rv))
                    return rv;
            
                rv = rv.Spawn1;
            }
            return rv;
        }
        private FibItem Last()
        {
            var rv = this.Walk((x) =>
            {
                return x.Spawn1 == null;
            });
            return rv;
        }

        private void ExpandArray()
        {
            
            var last = Last();

            FibItem newItem = new FibItem();
            newItem.Index = _nextIndex;

            _nextIndex++;

            

        }
        #endregion


    }
}
