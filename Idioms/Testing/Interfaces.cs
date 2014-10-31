using Decoratid.Core.Storing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Testable
{
    /// <summary>
    /// describes something that can test itself.  We isolate the test case generation from the test mechanism so that we
    /// can have more portability with our tests.
    /// </summary>
    public interface ISelfTest
    {
        IStore Test(IStore input);
        IStore GenerateTestInput();
    }
}
