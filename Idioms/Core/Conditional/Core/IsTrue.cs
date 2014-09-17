using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Core.Logical;

namespace Decoratid.Idioms.Core.Conditional.Core
{
    /// <summary>
    /// Always true condition
    /// </summary>
    [Serializable]
    public sealed class IsTrue : ICondition
    {
        public bool? Evaluate()
        {
            return true;
        }
        public static IsTrue New()
        {
            return new IsTrue();
        }
    }

}
