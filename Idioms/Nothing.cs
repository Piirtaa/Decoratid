using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms
{
    /// <summary>
    /// placeholder class that contains nothing of consequence
    /// </summary>
    public sealed class Nothing
    {
        private Nothing() { }

        public static Nothing VOID { get { return new Nothing(); } }
    }
}
