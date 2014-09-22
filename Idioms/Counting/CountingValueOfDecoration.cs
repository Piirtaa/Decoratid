﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Counting
{
    /// <summary>
    /// Adds a Counter to track number of times evaluation has been invoked
    /// </summary>
    public class CountingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasCounter
    {
        #region Ctor
        public CountingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
            Counter = new Counter();
        }
        #endregion

        #region Properties
        public Counter Counter { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            this.Counter.Increment();
            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new CountingValueOfDecoration<T>(thing);
        }
        #endregion
    }


}
