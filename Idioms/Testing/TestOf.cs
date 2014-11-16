using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.ValueOfing;
using System.Diagnostics;

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
            Debug.WriteLine("starting test " + this.GetType());
            this.TestLogic.CloneAndPerform(arg.AsNaturalValue());
        }
        #endregion
    }
}
