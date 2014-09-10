using Decoratid.Thingness.Decorations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.ValuesOf.Decorations
{
    /// <summary>
    /// define the value of decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueOfDecoration<T> : IValueOf<T>, IDecorationOf<IValueOf<T>> { }

    /// <summary>
    /// base class implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public abstract class DecoratedValueOfBase<T> : DecorationOfBase<IValueOf<T>>, IValueOfDecoration<T>//, ISerializable
    {
        #region Ctor
        protected DecoratedValueOfBase():base() { }
        public DecoratedValueOfBase(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Methods
        public virtual T GetValue()
        {
            return base.Decorated.GetValue();
        }
        public override IValueOf<T> This
        {
            get { return this; }
        }

        #endregion


    }
}
