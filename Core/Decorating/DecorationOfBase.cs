using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
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
    public interface IDecorationOf<T> //: IDecorationHydrateable 
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
        private T _Decorated;
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
        public T Decorated { get { return this._Decorated; } }
        public T Core { get { return this._Core; } }
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

        #region Iteration Members
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
        /// <summary>
        /// walks the decorator hierarchy to find the one of the provided type, and matching the filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Tdec FindDecoratorOf<Tdec>(bool exactTypeMatch, Func<T, bool> filter = null)
            where Tdec : T
        {
            var match = this.WalkLayers((dec) =>
            {
                //do type level filtering first

                //if we're exact matching, the decoration has to be the same type
                if (exactTypeMatch && typeof(Tdec).Equals(dec.GetType()) == false)
                    return false;

                //if we're not exact matching, the decoration has to be Of the same type
                if (exactTypeMatch == false && (!(dec is Tdec)) )
                    return false;

                //if we don't have a filter we're matching
                if (filter == null)
                    return true;

                //use the filter to determine the match
                return filter(dec);

            });

            if (match == null)
            {
                return default(Tdec);
            }

            return (Tdec)match;
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
            this.SetDecorated( newDec);
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
        public T Undecorate<Tdec>(bool exactTypeMatch, Func<T, bool> filter = null)
            where Tdec : T
        {
            //find the decoration we want to remove
            T decorationToRemove = this.FindDecoratorOf<Tdec>(exactTypeMatch, filter);
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
        #endregion

        #region IDecoration
        public abstract IDecorationOf<T> ApplyThisDecorationTo(T thing);
        #endregion

        //#region IDecorationHydrateable
        //public virtual string DehydrateDecoration(IGraph uow = null)
        //{
        //    throw new NotImplementedException();
        //}
        //public virtual void HydrateDecoration(string text, IGraph uow = null)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion

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
