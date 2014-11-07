using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Testing
{
    public static class MockTester
    {
        public static void AutoTestMocks()
        {

            var results1 = TestOfTester.AutomaticTest<Nothing>(Nothing.VOID);
            //results1.WithEach(
        }
    }
}
