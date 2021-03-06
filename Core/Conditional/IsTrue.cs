﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Logical;

namespace Decoratid.Core.Conditional
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
