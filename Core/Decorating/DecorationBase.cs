using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Decoratid.Extensions;
using Decoratid.Core.Identifying;

namespace Decoratid.Core.Decorating
{
    /// <summary>
    /// abstract class that provides templated implementation of a Decorator/Wrapper
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// Implements ISerializable so that derivations from this class will have hooks to implement
    /// native serialization
    /// </remarks>
    public abstract class DecorationBase : DisposableBase, IDecoration, ISerializable, IFaceted
    {
        #region Declarations
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _Decorated;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object _Core;
        #endregion

        #region Ctor
        /// <summary>
        /// the base ctor for a decoration.  it MUST decorate something!!  
        /// </summary>
        /// <param name="decorated">kacks on null</param>
        public DecorationBase(object decorated)
        {
            this.SetDecorated(decorated);
        }
        #endregion

        #region ISerializable
        protected DecorationBase(SerializationInfo info, StreamingContext context)
        {
            Type type = info.GetValue("_type", typeof(Type)) as Type;
            this._Decorated = info.GetValue("_Decorated", type);
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected virtual void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Decorated", this._Decorated);
            info.AddValue("_type", this.Decorated.GetType());
        }
        #endregion

        #region IFaceted
        /// <summary>
        /// by default does a loose search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public object GetFace(Type type)
        {
            var rv = this.As(type, false);
            return rv;
        }
        #endregion

        #region Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object Decorated { get { return this._Decorated; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object Core { get { return this._Core; } }
        #endregion

        #region Calculated Properties
        /// <summary>
        /// returns whether the decoration is decorating something.  We use this as a check that the
        /// decoration chain is unadulterated, and was constructed in a correct way. The general principle is
        /// to NOT allow decorations to be incorrectly constructed.
        /// </summary>
        public bool IsDecorating
        {
            get { return this.Decorated != null; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// sets the Decorated property.  If null, kacks
        /// </summary>
        /// <param name="decorated"></param>
        protected void SetDecorated(object decorated)
        {
            if (decorated == null)
                throw new InvalidOperationException("null decoration injection");

            if (decorated is ISealedDecoration)
                throw new InvalidOperationException("Cannot decorate a SealedDecoration");

            //if decorated is a decoration, we must ensure that none of the decoration layers are equal to this 
            //or we'll get a circ reference situation
            var decorationList = decorated.GetAllDecorations();
            //remove the first decoration because it is equivalent to "this"

            if (decorationList != null)
            {
                foreach (var each in decorationList)
                {
                    if (object.ReferenceEquals(each, this))
                        throw new InvalidOperationException("circular reference");
                }
            }

            this._Decorated = decorated;

            if (decorated is IDecoration)
            {
                IDecoration dec = decorated as IDecoration;
                this._Core = dec.Core;
            }
            else
            {
                this._Core = decorated;
            }
        }
        #endregion

        #region Disposable
        protected override void DisposeManaged()
        {
            //dispose the wrapper
            if (this.Decorated != null && this.Decorated is IDisposable)
            {
                ((IDisposable)(this.Decorated)).Dispose();
            }
            base.DisposeManaged();
        }
        #endregion

    }


}
