using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Testing
{
    public class TestOfResult : IHasId<string>
    {
        public TestOfResult(Type type, Exception ex)
        {
            this.Id = type.Name;
            this.TestError = ex;

        }
        public Exception TestError { get; set; }
        public bool IsTestSuccess { get { return this.TestError == null; } }

        public string Id
        {
            get;
            private set;
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }

    }
}
