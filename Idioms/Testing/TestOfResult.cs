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
        public Type TestType { get; set; }
        public Exception TestError { get; set; }
        public bool IsTestSuccess { get; set; }

        public string Id
        {
            get { return this.TestType.Name; }
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
    }
}
