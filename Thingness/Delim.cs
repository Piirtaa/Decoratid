using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness
{
    /// <summary>
    /// container of common delim characters
    /// </summary>
    public static class Delim
    {
        /// <summary>
        /// file separator
        /// </summary>
        public const char FS = (char)0x1C;
        /// <summary>
        /// group separator
        /// </summary>
        public const char GS = (char)0x1D;
        /// <summary>
        /// record separator
        /// </summary>
        public const char RS = (char)0x1E;
        /// <summary>
        /// unit separator
        /// </summary>
        public const char US = (char)0x1F;
    }
}
