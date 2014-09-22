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
     * To provide decorations of string that assist with (de)hydration, and help avoid data corruption.  
     * -The LengthPrefix decorations specify the exact layout of datasizes of the data we're storing,
     * and prefix the actual data with this, so that special character concerns during string parsing go away.
     *  This also has the characteristic of working regardless of the nesting of dehydrated data 
     *  as when non-trivial object graphing is done.  
     * -Decorations that do CRC or Hashing or some other data integrity check are also possible with this
     * idiom.
     * -Encryption can be introduced here also
     * 
     */ 
}
