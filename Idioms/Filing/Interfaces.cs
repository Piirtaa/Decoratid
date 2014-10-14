using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Filing
{

    public interface IFileable
    {
        string Read();
        void Write(string text);
    }

}
