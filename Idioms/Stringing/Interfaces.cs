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

}
