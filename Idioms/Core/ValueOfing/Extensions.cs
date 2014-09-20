﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core.ValueOfing.Decorations;

namespace Decoratid.Idioms.Core.ValueOfing
{
    public static class ValueOfExtensions
    {
        /// <summary>
        /// converts a thing into a ValueOf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static ValueOf<T> AsNaturalValue<T>(this T thing)
        {
            if (thing == null)
                throw new ArgumentNullException();

            if (thing is ValueOf<T>)
                return thing as ValueOf<T>;

            //TODO: IDEA? if thing is already a ValueOf, to just return self? What "is" valueOf exactly other than a decoration?
            //but for right now, we'll see where this goes
            return new ValueOf<T>(thing);
        }
        /// <summary>
        /// converts a thing factory into a ValueOf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static FactoriedValueOf<T> AsNaturalValueFactory<T>(this LogicTo<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            return new FactoriedValueOf<T>(factory);
        }
    }
}