using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Communicating
{
    public interface IDecoratedEndPointClient : IEndPointClient, IDecorationOf<IEndPointClient>
    {
    }

    public abstract class DecoratedEndPointClientBase : DecorationOfBase<IEndPointClient>, IDecoratedEndPointClient
    {
        #region Ctor
        public DecoratedEndPointClientBase(IEndPointClient decorated)
            : base(decorated)
        {
        }
        #endregion

        #region IEndPointClient
        public EndPoint EndPoint
        {
            get { return this.Decorated.EndPoint; }
        }
        public string Send(string request)
        {
            return this.Decorated.Send(request);
        }
        public override IEndPointClient This
        {
            get { return this; }
        }
        #endregion


    }
}
