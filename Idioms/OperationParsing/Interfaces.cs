﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.OperationParsing
{
    public interface IOperationParsable<T>
    {
        string Idiom { get; }
        Dictionary<string, Func<T, string[], bool>> OperationMap { get; }
    }
}
