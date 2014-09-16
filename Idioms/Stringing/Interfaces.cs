using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// describes a string that is convertable to another string, and vice versa
    /// </summary>
    public interface IStringable
    {
        string GetValue();
        void Parse(string text);
    }
    /// <summary>
    /// A list of strings that are stringable
    /// </summary>
    public interface IStringableList : IList<string>, IStringable
    {
    }

    /*What's the point of Stringable-ness, really?
     * To make (de)hydration data (typically a list of data), free from parsing problems when combined with a bunch of other 
     * (de)hydration data, at whatever level nesting.  Has to be done fractally such that it will work the same regardless of what
     * level in the serialization process we're at.  In the case, to ensure the data can be easily parsed/stored we
     * specify the exact sizes of the data we're storing with the LengthPrefix decorations.  We may add CRC or Hashing or some 
     * other integrity check decorations upon this.  The core concept, however, of using non-ambiguous parsing instructions, is established.
     * 
     */ 
}
