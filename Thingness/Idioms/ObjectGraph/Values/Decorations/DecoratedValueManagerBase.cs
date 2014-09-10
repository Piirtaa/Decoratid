using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.ObjectGraph.Values.Decorations
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
