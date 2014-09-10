using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// contains helper functions
    /// </summary>
    public static class StoreUtil
    {
        ///// <summary>
        ///// creates a ContextualAsId container without generics
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //public static IHasId BuildContextualAsId(object id, object context)
        //{
        //    Condition.Requires(id).IsNotNull();
        //    Condition.Requires(context).IsNotNull();

        //    Type idType = id.GetType();


        //    Type contextType = null;
        //    //if we're dealing with an actual type, calling gettype will give System.RuntimeType which is not what we want to lookup with
        //    if (context is Type)
        //    {
        //        contextType = (Type)context;
        //    }
        //    else
        //    {
        //        context.GetType();
        //    }
        //    var genType = typeof(ContextualAsId<,>).MakeGenericType(idType, contextType);
        //    var obj = Activator.CreateInstance(genType, id, context);

        //    IHasId hasId = obj as IHasId;
        //    return hasId;
        //}
    }
}
