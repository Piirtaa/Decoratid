using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core
{

    /// <summary>
    /// the base interface for something that has faces (eg. it's faceted).  
    /// </summary>
    public interface IFaceted
    {
        object GetFace(Type type);
        List<object> GetFaces();
    }

    public static class IFacetedExtensions
    {
        public static T GetFace<T>(this object obj)
        {
            Condition.Requires(obj).IsNotNull();

            if (!(obj is IFaceted))
                return default(T);

            var isa = IsA.New(obj as IFaceted);
            return isa.As<T>();
        }
    }
}
