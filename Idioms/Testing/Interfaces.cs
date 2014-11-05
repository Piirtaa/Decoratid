using Decoratid.Core.Storing;
using Decoratid.Idioms.OperationProtocoling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Testing
{
    /*
        Testing is just another use of the OperationProtocol where tests are implemented as operations.
     
     
    */


    public interface ITestable
    {
        OperationManager GetTests();
        IStore GenerateTestInput();
    }

    /// <summary>
    /// marker interface
    /// </summary>
    public interface ITest { }

    public interface ITestOf<T>: ITest
    {
        void Test(T arg);
    }
}
