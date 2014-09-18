using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring.Decorations
{
    
    
    public interface IExpireableDecoration : IExpirable, IDecorationOf<IExpirable> { }

    /// <summary>
    /// base class implementation of a IExpireable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ExpirableDecorationBase : DecorationOfBase<IExpirable>, IExpirable
    {
        #region Ctor
        public ExpirableDecorationBase(IExpirable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Properties
        public virtual bool IsExpired()
        {
            return base.Decorated.IsExpired();
        }
        public override IExpirable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
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
