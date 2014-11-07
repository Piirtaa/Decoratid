using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Filing
{
    /// <summary>
    /// a stringable that can access an underlying file store
    /// </summary>
    public interface IFileable : IStringable
    {
        /// <summary>
        /// reads the stringable value from the file.  will trigger a Parse()
        /// </summary>
        void Read();
        /// <summary>
        /// writes the stringable value to the file.  will trigger a GetValue()
        /// </summary>
        void Write();
    }

}
