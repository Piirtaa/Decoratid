using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring.Decorations
{
    
    
    public interface IWindowableDecoration : IWindowable, IDecorationOf<IWindowable> { }

    /// <summary>
    /// base class implementation of a IWindowable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class WindowableDecorationBase : DecorationOfBase<IWindowable>, IWindowable
    {
        #region Ctor
        public WindowableDecorationBase(IWindowable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Properties
        public virtual bool IsInWindow()
        {
            return base.Decorated.IsInWindow();
        }
        public override IWindowable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IWindowable> ApplyThisDecorationTo(IWindowable thing)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return null;
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
        }
        #endregion
    }
}
