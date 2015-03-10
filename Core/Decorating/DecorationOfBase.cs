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
    /// marker interface. If present on a decoration it prevents other decorations from decorating it
    /// </summary>
    public interface ISealedDecoration
    {

    }

    /// <summary>
    /// abstract class that provides templated implementation of a Decorator/Wrapper
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// Implements ISerializable so that derivations from this class will have hooks to implement
    /// native serialization
    /// </remarks>
    public abstract class DecorationOfBase<T> : DisposableBase, IDecorationOf<T>, ISerializable, IFaceted, IDecoratorAwareDecoration
    {
        #region Declarations
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T _Decorated;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T _Core;
        #endregion

        #region Ctor
        /// <summary>
        /// the base ctor for a decoration.  it MUST decorate something!!  
        /// </summary>
        /// <param name="decorated">kacks on null</param>
        public DecorationOfBase(T decorated)
        {
            this.SetDecorated(decorated);
        }
        #endregion

        #region ISerializable
        protected DecorationOfBase(SerializationInfo info, StreamingContext context)
        {
            this._Decorated = (T)info.GetValue("_Decorated", typeof(T));
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
        }
        #endregion

        #region IFaceted
        /// <summary>
        /// by default does a loose search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        object IFaceted.GetFace(Type type)
        {
            var rv = this.As(type, false);
            return rv;
        }
        List<object> IFaceted.GetFaces()
        {
            var rv = this.GetAllDecorations();
            return rv;
        }
        #endregion

        #region IDecoration
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Decorated { get { return this._Decorated; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IDecoration.Decorated
        {
            get { return this.Decorated; }
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Core { get { return this._Core; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IDecoration.Core { get { return this.Core; } }
        #endregion

        #region IDecorationOf
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public abstract T This { get; }
        #endregion

        #region IDecoratorAwareDecoration
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        /// <summary>
        /// the thing decorating this
        /// </summary>
        public object Decorator { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        /// <summary>
        /// the outermost decoration in the stack
        /// </summary>
        public object Outer { get { return this.GetOuterDecorator(); } }
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
        public List<object> Cake { get { return this.GetAllDecorations(); } }
        #endregion


        #region Methods
        /// <summary>
        /// sets the Decorated property.  If null, kacks
        /// </summary>
        /// <param name="decorated"></param>
        protected void SetDecorated(T decorated)
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

            if (decorated is IDecorationOf<T>)
            {
                IDecorationOf<T> dec = decorated as IDecorationOf<T>;
                this._Core = dec.Core;
            }
            else
            {
                this._Core = decorated;
            }

            //set the decorator backreference
            if (decorated.IsADecoratorAwareDecoration())
            {
                (decorated as IDecoratorAwareDecoration).Decorator = this;
            }

            //if a cake constraint (aka IHasDecoration) is declared anywhere in the stack
            //we validate the current cake supports the constraint.  this is a topdown walk
            var cake = this.Cake;
            cake.WithEach(layer =>
            {
                if (layer is IHasDecoration)
                {
                    //get all the interfaces it has, that derive from IHasDecoration
                    var layerType = layer.GetType();

                    var interfaces = layerType.GetInterfaces();
                    foreach (var interfaceType in interfaces)
                    {
                        if (!interfaceType.Name.Contains("IHasDecoration`"))
                            continue;

                        var requiredDecorations = interfaceType.GetGenericArguments();

                        foreach (Type each in requiredDecorations)
                        {
                            if (this.As(each, false) == null)
                                throw new InvalidOperationException(string.Format("required decoration {0} not found in cake", each.Name));
                        }
                    }
                }
            });
        }
        /// <summary>
        /// replaces the Decorated member.  In effect, we are injecting a decoration immediately below the "surface"
        /// or outermost decoration.
        /// </summary>
        /// <param name="decorationStrategy"></param>
        public void ReplaceDecorated(Func<T, T> decorationStrategy)
        {
            Condition.Requires(decorationStrategy).IsNotNull();
            var newDec = decorationStrategy(this.Decorated);
            this.SetDecorated(newDec);
        }

        //public T Undecorate(Type decType, bool exactTypeMatch)
        //{
        //    //find the decoration we want to remove
        //    T decorationToRemove = this.AsBelow.FindDecorationOf(decType, exactTypeMatch);
        //    if (decorationToRemove == null)
        //        throw new InvalidOperationException("decoration not found");

        //    //get the decorations from the inside out
        //    List<T> decorations = this.GetAllDecorationsOf();
        //    decorations.Reverse();

        //    //iterate to the decoration to remove
        //    T wrappee = default(T);
        //    foreach (var each in decorations)
        //    {
        //        IDecorationOf<T> dec = each as IDecorationOf<T>;
        //        if (dec == null)
        //            continue;

        //        //set the flag when we're at the right index
        //        if (object.ReferenceEquals(each, decorationToRemove))
        //        {
        //            wrappee = dec.Decorated;
        //            continue;
        //        }

        //        //skip if the flag isn't set
        //        if (wrappee == null)
        //            continue;

        //        //we're at the decoration above the decoration to remove
        //        //we want to add all the remaining layers on
        //        //NOTE: this should kack if invalid
        //        var decWrappee = dec.ApplyThisDecorationTo(wrappee);
        //        wrappee = decWrappee.This;
        //    }

        //    return wrappee;
        //}
        ///// <summary>
        ///// looks for the provided decoration layer and tries to build the same store without this decoration.
        ///// </summary>
        ///// <remarks>
        ///// if there is a dependency of one layer upon the other, and the dependency is removed, the ctor 
        ///// chain should kack - we provide the same checks as a ctor does, in this method. 
        ///// </remarks>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public T Undecorate<Tdec>(bool exactTypeMatch)
        //    where Tdec : T
        //{
        //    return Undecorate(typeof(Tdec), exactTypeMatch);
        //}
        #endregion

        #region IDecoration
        public abstract IDecorationOf<T> ApplyThisDecorationTo(T thing);
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

        //#region IIsIdiomatic
        ///// <summary>
        ///// quick check flag to determine if something is idiomatic
        ///// </summary>
        //public abstract bool IsIdiomatic { get; }

        //public abstract string IdiomName { get; } //should come from an attribute, just putting it here as a reminder
        ///// <summary>
        ///// returns the textual representation of state according to the template Has {Idiom}.HasProperty({PropertyName},{Value}).HasProperty({PropertyName},{Value})
        ///// </summary>
        ///// <returns></returns>
        //public string GetIdiomaticRepresentation();
        ///// <summary>
        ///// sets current state by parsing the command
        ///// </summary>
        //public void ParseIdiomaticRepresentation();
        ///// <summary>
        ///// sets the property to the value.  should only work on idiomatic properties
        ///// </summary>
        ///// <param name="propertyName"></param>
        ///// <param name="val"></param>
        //void HasProperty(string propertyName, object val); 
        //#endregion
    }


}
