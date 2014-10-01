using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Rehydrating
{

    /// <summary>
    /// marker interface if a type knows how to (de)serialize, AKA (de)hydrate itself
    /// </summary>
    public interface IRehydrating
    {
        string Dehydrate();
        void Hydrate(string text);
    }

    /// <summary>
    /// defines the mechanism of invoking hydration on IRehydratings.  Is an interface in case we want to decorate
    /// even more
    /// </summary>
    public interface IReconstor
    {
        string Dehydrate(IRehydrating obj);
        IRehydrating Hydrate(string text);
    }
}
