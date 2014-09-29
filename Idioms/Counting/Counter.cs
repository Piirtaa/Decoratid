using CuttingEdge.Conditions;
using Decoratid.Idioms.Polyfacing;
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
    public class Counter : IPolyfacing
    {
        private int _counter = 0;

        public int Increment()
        {
            var rv = Interlocked.Increment(ref _counter);
            return rv;
        }
        public int Current { get { return _counter; } }

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion
    }

    public static class CounterExtensions
    {
        public static Polyface IsCounter(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var counter = new Counter();
            root.Is(counter);
            return root;
        }
        public static Counter AsCounter(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<Counter>();
            return rv;
        }
    }
}
