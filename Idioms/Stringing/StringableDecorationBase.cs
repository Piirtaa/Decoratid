using Decoratid.Idioms.Core;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// defines a stringable decoration.
    /// NOTE: the decoration hydration mechanism SHOULD enable conversion to and from the raw value.
    /// For example, Hydrate() should take in the encoded value, and the raw value would 
    /// Dehydrate() should output the encoded value.
    /// </summary>
    public interface IStringableDecoration : IStringable, IDecorationOf<IStringable> { }

    /// <summary>
    /// base class implementation of a IStringable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class StringableDecorationBase : DecorationOfBase<IStringable>, IStringable
    {
        #region Ctor
        public StringableDecorationBase(IStringable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Properties
        public virtual string GetValue()
        {
            return base.Decorated.GetValue();
        }
        public virtual void Parse(string text)
        {
            base.Decorated.Parse(text);
        }
        public override IStringable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IStringable> ApplyThisDecorationTo(IStringable thing)
        {
            throw new NotImplementedException();
        }
        #endregion

        //#region IDecorationHydrateable
        //public override string DehydrateDecoration(IGraph uow = null)
        //{
        //    return this.GetValue();
        //}
        //public override void HydrateDecoration(string text, IGraph uow = null)
        //{
        //    this.Parse(text);
        //}
        //#endregion
    }
}
