﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Logical;
using Decoratid.Core.Conditional;
using Decoratid.Core.ValueOfing;
using Decoratid.Core.Identifying;

namespace Decoratid.Idioms.Testing
{
    public static class MockTester
    {
        public static void AutoTestMocks()
        {

            var results1 = TestOfTester.AutomaticTest<Nothing>(Nothing.VOID);
            TestOfTester.LogTestResults(results1, "VoidTests.txt");

            var results2 = TestOfTester.AutomaticTest<ILogic>(MockBuilder.BuildMockLogic());
            TestOfTester.LogTestResults(results2, "LogicTests.txt");

            var results3 = TestOfTester.AutomaticTest<ICondition>(MockBuilder.BuildMockCondition());
            TestOfTester.LogTestResults(results3, "ConditionTests.txt");

            var results4 = TestOfTester.AutomaticTest<IValueOf<IHasId>>(MockBuilder.BuildMockValueOf("id"));
            TestOfTester.LogTestResults(results4, "ValueOfTests.txt");
        }
    }
}