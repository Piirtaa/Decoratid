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
    /// <summary>
    /// defines host decorations
    /// </summary>
    public interface IDecoratedEndPointHost : IEndPointHost, IDecorationOf<IEndPointHost>
    {
    }

    /// <summary>
    /// define the base class for end point logic decorations.  
    /// </summary>
    public abstract class DecoratedEndPointHostBase : DecorationOfBase<IEndPointHost>, IDecoratedEndPointHost
    {
        #region Ctor
        public DecoratedEndPointHostBase(IEndPointHost decorated)
            : base(decorated)
        {
        }
        #endregion

        #region IEndPointHost
        public virtual bool Initialize()
        {
            return this.Decorated.Initialize();
        }
        public bool Start()
        {
            return this.Decorated.Start();
        }
        public bool Stop()
        {
            return this.Decorated.Stop();
        }
        public Serviceable.ServiceStateEnum CurrentState
        {
            get { return this.Decorated.CurrentState; }
        }
        public EndPoint EndPoint
        {
            get { return this.Decorated.EndPoint; }
        }
        public LogicOfTo<string, string> Logic
        {
            get
            {
                return this.Decorated.Logic;
            }
            set
            {
                this.Decorated.Logic = value;
            }
        }        

        public override IEndPointHost This
        {
            get { return this; }
        }
        #endregion


    }
}
