using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Thingness.Decorations;

namespace Decoratid.Communication.Decorations
{

    /// <summary>
    /// a EndPointLogic decoration
    /// </summary>
    public interface IDecoratedEndPointLogic : IDecorationOf<IEndPointLogic>, IEndPointLogic
    {
    }

    /// <summary>
    /// abstract class that provides templated implementation of a Decorator/Wrapper of IEndPointLogic
    /// </summary>
    /// 
    [Serializable]
    public abstract class DecoratedEndPointLogicBase : DecorationOfBase<IEndPointLogic>, IDecoratedEndPointLogic
    {
        #region Ctor
        /// <summary>
        /// ctor.  requires IEndPointLogic to wrap
        /// </summary>
        /// <param name="decorated"></param>
        public DecoratedEndPointLogicBase(IEndPointLogic decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Properties
        public override IEndPointLogic This { get { return this; } }
        #endregion

        #region Overrides - Virtual Hooks
        public virtual string HandleRequest(string data)
        {
            return this.Decorated.HandleRequest(data);
        }
        #endregion
    }
}