using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Counting
{
    /// <summary>
    /// a counter that does a threadsafe increment.  
    /// </summary>
    [Serializable]
    public class Counter
    {
        private int _counter = 0;

        public int Increment()
        {
            var rv = Interlocked.Increment(ref _counter);
            return rv;
        }
        public int Current { get { return _counter; } }
    }
}
