using Decoratid.Core.Logical;
using System;

namespace Decoratid.Core.ValueOfing
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
