using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Decoratid.Core.Decorating
{


    /// <summary>
    /// a generic decoration
    /// </summary>
    /// <remarks>
    /// Note that the implementors of this MUST also derive from T - which really is the whole point of a decoration.
    /// Only c# can't have a generic type inherit from the generic arg type, so it can't be declared here.
    /// </remarks>
    public interface IDecorationOf<T>
    {
        /// <summary>
        /// in a chain of decorations, it's the core value being decorated
        /// </summary>
        T Core { get; }
        /// <summary>
        /// the immediate thing we are decorating
        /// </summary>
        T Decorated { get; }
        /// <summary>
        /// MUST return a reference to this but cast as T.  This gets around the c# generic inheritance language constraint
        /// </summary>
        T This { get; }

        /// <summary>
        /// Essentially is a clone mechanism.  Allow the current decoration to recreate an instance like itself when
        /// provided with a thing to decorate - think of this as a ctor with only one arg (the thing) and all other args
        /// coming from the current instance.
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        IDecorationOf<T> ApplyThisDecorationTo(T thing);
    }

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
    public abstract class DecorationOfBase<T> : DisposableBase, IDecorationOf<T>, ISerializable
    {
        #region Declarations
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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

        #region Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Decorated { get { return this._Decorated; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Core { get { return this._Core; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public abstract T This { get; }
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

        #region FreeWalk Iteration
        /// <summary>
        /// Does a walk, but doesn't restrict the walk to Decorations of T.  Will walk all Decorations regardless of type.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <remarks>
        /// If a decoration chain has a change of layer type (ie. we start off decorating T1 and at some point a decoration
        /// converts the thing to a T2, which itself is then decorated) this function lets us do that.  It's semantically 
        /// equivalent to a polyfacing interface search but for decorations.
        /// </remarks>
        public object FreeWalk(Func<object, bool> filter)
        {
            object currentLayer = this.This;

            //iterate down
            while (currentLayer != null)
            {
                //check filter.  break/return
                if (filter(currentLayer))
                {
                    return currentLayer;
                }

                //if it's a decoration get the decorated layer
                if (DecorationUtils.IsDecoration(currentLayer))
                {
                    currentLayer = DecorationUtils.GetDecorated(currentLayer);
                }
                else
                {
                    //not decorated, and fails the filter?  stop here
                    return null;
                }
            }

            return null;
        }
        public object FreeWalkFindDecoratorOf(Type decType, bool exactTypeMatch)
        {
            var match = this.FreeWalk((dec) =>
            {
                //do type level filtering first

                //if we're exact matching, the decoration has to be the same type
                if (exactTypeMatch && decType.Equals(dec.GetType()) == false)
                    return false;

                //if we're not exact matching, the decoration has to be Of the same type
                if (exactTypeMatch == false && (!(decType.IsAssignableFrom(dec.GetType()))))
                    return false;

                return true;

            });

            return match;
        }
        #endregion

        #region Layer Iteration Members
        /// <summary>
        /// returns the first decoration that matches the filter. stops iterating if ever finds a null decorated - 
        /// no chain to traverse!. will never return the core item (layer 0)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T WalkLayers(Func<T, bool> filter)
        {
            T currentLayer = this.This;

            //iterate down
            while (currentLayer != null)
            {
                //check filter.  break/return
                if (filter(currentLayer))
                {
                    return currentLayer;
                }

                //recurse if it's decorated
                if (currentLayer is IDecorationOf<T>)
                {
                    IDecorationOf<T> layer = (IDecorationOf<T>)currentLayer;
                    currentLayer = layer.Decorated;
                }
                else
                {
                    //not decorated, and fails the filter?  stop here
                    return default(T);
                }
            }

            return default(T);
        }
        public Tdec FindDecoratorOf<Tdec>(bool exactTypeMatch)
    where Tdec : T
        {
            var rv = FindDecoratorOf(typeof(Tdec), exactTypeMatch);
            if (rv == null)
                return default(Tdec);

            return (Tdec)rv;
        }
        /// <summary>
        /// walks the decorator hierarchy to find the one of the provided type, and matching the filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindDecoratorOf(Type decType, bool exactTypeMatch)
        {
            var match = this.WalkLayers((dec) =>
            {
                //do type level filtering first

                //if we're exact matching, the decoration has to be the same type
                if (exactTypeMatch && decType.Equals(dec.GetType()) == false)
                    return false;

                //if we're not exact matching, the decoration has to be Of the same type
                if (exactTypeMatch == false && (!(decType.IsAssignableFrom(dec.GetType()))))
                    return false;

                return true;

            });

            if (match == null)
            {
                return default(T);
            }

            return match;
        }
        /// <summary>
        /// walks the decorations from outermost to core
        /// </summary>
        /// <returns></returns>
        [ScriptIgnore]
        public List<T> OutermostToCore
        {
            get
            {
                List<T> returnValue = new List<T>();

                var match = this.WalkLayers((reg) =>
                {
                    returnValue.Add(reg);
                    return false;
                });

                return returnValue;
            }
        }
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

        public T Undecorate(Type decType, bool exactTypeMatch)
        {
            //find the decoration we want to remove
            T decorationToRemove = this.FindDecoratorOf(decType, exactTypeMatch);
            if (decorationToRemove == null)
                throw new InvalidOperationException("decoration not found");

            //get the decorations from the inside out
            List<T> decorations = this.OutermostToCore;
            decorations.Reverse();

            //iterate to the decoration to remove
            T wrappee = default(T);
            foreach (var each in decorations)
            {
                IDecorationOf<T> dec = each as IDecorationOf<T>;
                if (dec == null)
                    continue;

                //set the flag when we're at the right index
                if (object.ReferenceEquals(each, decorationToRemove))
                {
                    wrappee = dec.Decorated;
                    continue;
                }

                //skip if the flag isn't set
                if (wrappee == null)
                    continue;

                //we're at the decoration above the decoration to remove
                //we want to add all the remaining layers on
                //NOTE: this should kack if invalid
                var decWrappee = dec.ApplyThisDecorationTo(wrappee);
                wrappee = decWrappee.This;
            }

            return wrappee;
        }
        /// <summary>
        /// looks for the provided decoration layer and tries to build the same store without this decoration.
        /// </summary>
        /// <remarks>
        /// if there is a dependency of one layer upon the other, and the dependency is removed, the ctor 
        /// chain should kack - we provide the same checks as a ctor does, in this method. 
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Undecorate<Tdec>(bool exactTypeMatch)
            where Tdec : T
        {
            return Undecorate(typeof(Tdec), exactTypeMatch);
        }
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

    }


}
