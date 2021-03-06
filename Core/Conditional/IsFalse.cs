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
    /// always false condition
    /// </summary>
    [Serializable]
    public sealed class IsFalse : ICondition
    {
        public bool? Evaluate()
        {
            return false;
        }
        public static IsFalse New()
        {
            return new IsFalse();
        }
    }
}
