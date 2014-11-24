using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.ValueOfing;
using System.Diagnostics;
using System.Threading;

namespace Decoratid.Idioms.Testing
{
    /// <summary>
    /// container of test logic of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TestOf<T> : ITestOf<T>
    {
        #region Ctor
        public TestOf(LogicOf<T> testLogic)
        {
            Condition.Requires(testLogic).IsNotNull();
            this.TestLogic = testLogic;
        }
        #endregion

        #region Properties
        private LogicOf<T> TestLogic { get; set; }
        #endregion

        #region ITestOf
        public void Test(T arg)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();



            Debug.WriteLine(string.Format("-------- Thread {0} starting test {1}", Thread.CurrentThread.ManagedThreadId, this.GetType()));
            this.TestLogic.Perform(arg);
            Debug.WriteLine(string.Format("-------- Thread {0} ending test {1}", Thread.CurrentThread.ManagedThreadId, this.GetType()));

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Debug.WriteLine("RunTime " + elapsedTime);
        }
        #endregion
    }
}
