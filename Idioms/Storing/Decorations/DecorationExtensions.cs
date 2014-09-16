using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.Core.Conditional.Common;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Storing.Decorations
{

    public static class DecorationExtensions
    {
        /// <summary>
        /// if the store is decorated, walks the decorations to find one of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="exactTypeMatch"></param>
        /// <returns></returns>
        public static T FindDecoratorOf<T>(this IStore store, bool exactTypeMatch)
            where T:IDecoratedStore
        {
            Condition.Requires(store).IsNotNull();

            if (!(store is DecoratedStoreBase))
                return default(T);

            DecoratedStoreBase dec = store as DecoratedStoreBase;
            return dec.FindDecoratorOf<T>(exactTypeMatch);
        }

        ///// <summary>
        ///// provide the interfaces to decorate with
        ///// </summary>
        ///// <param name="store"></param>
        ///// <returns></returns>
        //public static IStore DecorateWith(this IStore store, List<Type> decorations)
        //{
        //    IStore returnValue = null;

        //    decorations.WithEach((type) =>
        //    {
        //        if (type.IsInterface && typeof(IDecoratedStore).IsAssignableFrom(type))
        //        {
        //            //find the decoration
        //        }
        //    });

        //    return returnValue;
        //}
    }
}
