using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Touching
{   
    public interface ITouchable 
    {
        void Touch();
    }

    public interface IHasTouchable
    {
        ITouchable Touchable { get; }
    }
}
