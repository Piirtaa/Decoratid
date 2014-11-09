using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Storidioms.StoreOf;
using System;

namespace Decoratid.Idioms.ObjectGraphing
{
    public interface IGraphDecoration : IGraph, IDecorationOf<IGraph> { }

    [Serializable]
    public abstract class DecoratedGraphBase : DecorationOfBase<IGraph>, IGraphDecoration
    {
        public const string ID = "";

        #region Ctor
        public DecoratedGraphBase(IGraph decorated)
            : base(decorated)
        {
        }
        #endregion

        #region IGraph
        public virtual IStoreOf<GraphNode> NodeStore { get { return this.Decorated.NodeStore; } }
        public virtual ValueManagerChainOfResponsibility ChainOfResponsibility { get { return this.Decorated.ChainOfResponsibility; } }
        #endregion

        #region Methods
        public override IGraph This
        {
            get { return this; }
        }
        #endregion
    }
}
