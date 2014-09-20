﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.Core.ValueOfing.Decorations;
using Decoratid.Idioms.Core.ValueOfing;
using Decoratid.Idioms.Core;

namespace Decoratid.Idioms.Polyfacing
{
    /// <summary>
    /// makes the valueof polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPolyfacingValueOf<T> : IValueOfDecoration<T>, IPolyfacing
    {
    }

    /// <summary>
    /// decorates as polyfacing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PolyfacingValueOfDecoration<T> : DecoratedValueOfBase<T>, IPolyfacingValueOf<T>
    {
        #region Ctor
        public PolyfacingValueOfDecoration(IValueOf<T> decorated, Polyface rootFace = null)
            : base(decorated)
        {
            this.RootFace = rootFace;
        }
        #endregion

        #region Fluent Static
        public static PolyfacingValueOfDecoration<T> New(IValueOf<T> decorated, Polyface rootFace = null)
        {
            return new PolyfacingValueOfDecoration<T>(decorated, rootFace);
        }
        #endregion

        #region ISerializable
        protected PolyfacingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.RootFace = (Polyface)info.GetValue("RootFace", typeof(Polyface));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RootFace", this.RootFace);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IPolyfacing
        public Polyface RootFace { get; set; }
        #endregion

        #region Methods
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new PolyfacingValueOfDecoration<T>(thing, this.RootFace);
        }
        #endregion
    }

    public static partial class Extensions
    {
        /// <summary>
        /// decorates with polyfacingness if it's not already there
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="rootFace"></param>
        /// <returns></returns>
        public static PolyfacingValueOfDecoration<T> Polyfacing<T>(this IValueOf<T> valueOf, Polyface rootFace = null)
        {
            Condition.Requires(valueOf).IsNotNull();

            if (valueOf is PolyfacingValueOfDecoration<T>)
            {
                var pf = valueOf as PolyfacingValueOfDecoration<T>;
                return pf;
            }

            return new PolyfacingValueOfDecoration<T>(valueOf, rootFace);
        }
    }


}