using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Extensions;

namespace Sandbox.Store.ConcurrencyVersioning
{

    [Serializable]
    public class IncrementalVersionMismatchException : ApplicationException
    {

        public IncrementalVersionMismatchException()
            : base()
        {

        }
        public IncrementalVersionMismatchException(IIncrementalVersion expectedVersion)
            : base(string.Format("The expected version is {0}.", expectedVersion.GetVersionText()))
        {
            this.ExpectedVersion = expectedVersion;
        }
        public IncrementalVersionMismatchException(IIncrementalVersion expectedVersion, IIncrementalVersion actualVersion)
            : base(string.Format("The expected version is {0}.  The actual version is {1}.", 
            expectedVersion.GetVersionText(), actualVersion.GetVersionText()))
        {
            this.ExpectedVersion = expectedVersion;
            this.ActualVersion = actualVersion;
        }

        public IIncrementalVersion ActualVersion { get; set; }
        public IIncrementalVersion ExpectedVersion { get; set; }
    }
}
