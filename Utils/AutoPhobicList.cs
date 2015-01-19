using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    /// <summary>
    /// takes a source list of T and enqueues each item.  Arranges the order of enqueuing such that
    /// successive items are as far apart from each other on the source list, positionally, as possible.  
    /// Has facility to remove items by initial list index and the queue will rearrange itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoPhobicQueue<T>: Queue<T>
    {
        #region Declarations
        private readonly object _stateLock = new object();

        #endregion

        #region Ctor
        public AutoPhobicQueue(IEnumerable<T> list)
        {
            this.Decorated = new List<T>();
            this.Decorated.AddRange(list);

            //now arrange the items so they are as far away from each other as possible, index wise
            //For example, in a list of 9 items, the original order is : 1,2,3,4,5,6,7,8,9
            //the modified order follows this pattern: start at the same initial index, then add the 
            //last item, then the 2nd item, then the 2nd last, etc.
            //the modified order becomes: 1,9,2,8,3,7,4,6,5
            //the characteristic of this arrangement is that as we iterate thru the list, we're maintaining
            //the widest item distance possible between successive items. the distance between items decreases
            //as we progress thru the iteration.  this aligns well with the type of algs such an arrangement would
            //used on - like string matching.
            //
            //what's the use of such a thing?
            //if we're parsing a string and using a skipahead approach, we can parallelize the character selection
            //such that each character has the best chance to move the cursor ahead without collision during the skiphead portion.
            //so we're covering more distance, faster.
        }
        #endregion

        #region Properties
        public List<T> Decorated { get; private set; }
        #endregion

        #region Methods
        public void RemoveItem(int index)
        {
            this.Decorated.RemoveAt(index);
        }
        public void AddItem(T item)
        {
            this.Decorated.Add(item);
        }
        #endregion
    }
}
