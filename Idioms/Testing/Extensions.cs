using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Storidioms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Testing
{
    public static class Extensions
    {
        public static void RunTest(this ITestable testable)
        {
            Condition.Requires(testable).IsNotNull();

            var tests = testable.GetTests();
            var testStore = testable.GenerateTestInput();
            IStore testResultsStore = NaturalInMemoryStore.New();
            tests.HandleOperations(testStore, testResultsStore);

            var errors = tests.GetErrors(testResultsStore);

            if (errors != null && errors.Count > 0)
            {
                //output the stores
                testResultsStore.JoinStore(testStore);
                var dump = StoreSerializer.SerializeStore(testResultsStore, ValueManagerChainOfResponsibility.NewDefault());
                Debug.WriteLine(dump);
                throw new InvalidOperationException("test failure");

            }
        }
    }
}
