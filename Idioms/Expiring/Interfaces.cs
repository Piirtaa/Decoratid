using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring
{   
    /// <summary>
    /// something that expires
    /// </summary>
    public interface IExpirable
    {
        bool IsExpired();
    }
    /// <summary>
    /// composites Expirable
    /// </summary>
    public interface IHasExpirable : IExpirable
    {
        IExpirable Expirable { get; }
    }

}
