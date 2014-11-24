using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Logical;
using Decoratid.Core.Conditional;
using Decoratid.Core.ValueOfing;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Communicating.Socketing;
using Decoratid.Idioms.Communicating;
using Decoratid.Core.Storing;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Testing
{
    public static class MockTester
    {
        public static void AutoTestMocks()
        {

            var stringableListResults = TestOfTester.AutomaticTest<IStringableList>(NaturalStringableList.New("one", "two", "three"));
            TestOfTester.LogTestResults(stringableListResults, "StringableListTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(stringableListResults)).IsTrue();

            var stringableResults = TestOfTester.AutomaticTest<IStringable>(NaturalStringable.New("test"));
            TestOfTester.LogTestResults(stringableResults, "StringableTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(stringableResults)).IsTrue();

            var results1 = TestOfTester.AutomaticTest<Nothing>(Nothing.VOID);
            TestOfTester.LogTestResults(results1, "VoidTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(results1)).IsTrue();

            var results2 = TestOfTester.AutomaticTest<ILogic>(MockBuilder.BuildMockLogic());
            TestOfTester.LogTestResults(results2, "LogicTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(results2)).IsTrue();

            var results3 = TestOfTester.AutomaticTest<ICondition>(MockBuilder.BuildMockCondition());
            TestOfTester.LogTestResults(results3, "ConditionTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(results3)).IsTrue();

            var results4 = TestOfTester.AutomaticTest<IValueOf<IHasId>>(MockBuilder.BuildMockValueOf("id"));
            TestOfTester.LogTestResults(results4, "ValueOfTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(results4)).IsTrue();

            var results5 = TestOfTester.AutomaticTest<IValueOf<string>>("derp".AsNaturalValue());
            TestOfTester.LogTestResults(results5, "ValueOfStringTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(results5)).IsTrue();

            var results6 = TestOfTester.AutomaticTest<Host>(Host.New(EndPoint.NewFreeLocalEndPoint()));
            TestOfTester.LogTestResults(results6, "HostTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(results6)).IsTrue();

            var results7 = TestOfTester.AutomaticTest<IStore>(MockBuilder.BuildMockStore());
            TestOfTester.LogTestResults(results7, "StoreTests.txt");
            bool good = TestOfTester.CheckTestResults(results7);
            Condition.Requires(TestOfTester.CheckTestResults(results7)).IsTrue();

            var results8 = TestOfTester.AutomaticTest<Tuple<IStore, IHasId>>(new Tuple<IStore, IHasId>(MockBuilder.BuildMockStore(), MockBuilder.BuildMockIHasId("1")));
            TestOfTester.LogTestResults(results8, "StoreItemTests.txt");
            Condition.Requires(TestOfTester.CheckTestResults(results8)).IsTrue();
        }
    }
}
