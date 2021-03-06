﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Conditional
{
    /// <summary>
    /// defines a simple condition
    /// </summary>
    public interface ICondition 
    {
        bool? Evaluate();
    }

    /// <summary>
    /// a condition that can clone itself
    /// </summary>
    public interface ICloneableCondition
    {
        ICondition Clone();
    }

    /// <summary>
    /// HasA implementation
    /// </summary>
    public interface IHasCondition
    {
        ICondition Condition { get; set; }
    }
}