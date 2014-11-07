﻿using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Polyfacing;

namespace Decoratid.Idioms.Sealing
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                bool success = false;
                try
                {
                    x.Seal().Polyfacing();

                }
                catch
                {
                    success = true;
                }
                if (!success)
                    throw new InvalidOperationException();
            }))
        {
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
            {
                bool success = false;
                try
                {
                    x.Seal().Polyfacing();

                }
                catch
                {
                    success = true;
                }
                if (!success)
                    throw new InvalidOperationException();
            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                bool success = false;
                try
                {
                    x.Seal().Polyfacing();

                }
                catch
                {
                    success = true;
                }
                if (!success)
                    throw new InvalidOperationException();
            }))
        {
        }
    }
}
