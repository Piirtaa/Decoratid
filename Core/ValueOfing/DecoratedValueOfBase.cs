﻿using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.ValueOfing
{
    /// <summary>
    /// define the value of decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDecoratedValueOf<T> : IValueOf<T>, IDecorationOf<IValueOf<T>> { }

    /// <summary>
    /// base class implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class DecoratedValueOfBase<T> : DecorationOfBase<IValueOf<T>>, IDecoratedValueOf<T>
    {
        #region Ctor
        public DecoratedValueOfBase(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion
        
        #region ISerializable
        protected DecoratedValueOfBase(SerializationInfo info, StreamingContext context) : base(info, context)
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
