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
            Thread.Sleep(1000);
            Debug.WriteLine(string.Format("-------- Thread {0} starting test {1}", Thread.CurrentThread.ManagedThreadId, this.GetType()));
            this.TestLogic.CloneAndPerform(arg.AsNaturalValue());
            Debug.WriteLine(string.Format("-------- Thread {0} ending test {1}", Thread.CurrentThread.ManagedThreadId, this.GetType()));
        }
        #endregion
    }
}
