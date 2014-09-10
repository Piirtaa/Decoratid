using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Hydrateable
{
    /// <summary>
    /// serialization marker interface if a type knows how to serialize itself
    /// </summary>
    public interface IHydrateable
    {
        string Dehydrate();
        void Hydrate(string text);
    }

    /// <summary>
    /// Marker interface saying that the hydration conforms to a stringable/lengthprefixlist text decoration
    /// format where the first item is the assembly qualified type name of the type to hydrate.  
    /// </summary>
    public interface IHydratesWithTypePrefix : IHydrateable
    {

    }
}
