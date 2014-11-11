using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using System;
using System.Collections.Generic;

namespace Decoratid.Idioms.ObjectGraphing.Values.Decorations
{
    public interface INodeValueManagerDecoration : INodeValueManager, IDecorationOf<INodeValueManager> { }

    /// <summary>
    /// base class implementation of a valuemanager decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class DecoratedValueManagerBase : DecorationOfBase<INodeValueManager>, INodeValueManager
    {
        public const string ID = "";

        #region Ctor
        public DecoratedValueManagerBase(INodeValueManager decorated)
            : base(decorated)
        {
        }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public virtual void RewriteNodePath(GraphPath path, object obj)
        {
            base.Decorated.RewriteNodePath(path, obj);
        }
        public virtual List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return base.Decorated.GetChildTraversalNodes(obj, nodePath);
        }
        public virtual bool CanHandle(object obj, IGraph uow)
        {
            return base.Decorated.CanHandle(obj, uow);
        }
        public virtual string DehydrateValue(object obj, IGraph uow)
        {
            return base.Decorated.DehydrateValue(obj, uow);
        }
        public virtual object HydrateValue(string nodeText, IGraph uow)
        {
            return base.Decorated.HydrateValue(nodeText, uow);
        }
        public override INodeValueManager This
        {
            get { return this; }
        }
        #endregion
    }
}
