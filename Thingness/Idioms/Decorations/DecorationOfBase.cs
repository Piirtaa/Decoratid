using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.ObjectGraph;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Decoratid.Thingness.Decorations
{
    /// <summary>
    /// responsibility of a decoration to provide (de)hydration for its own layer, for use by DecorationValueManager
    /// </summary>
    /// <remarks>
    /// DecorationValueManager uses the decoration's Apply facility to build up, layer by layer, a
    /// decoration.  Each decoration need only be responsible for (de)hydrating its individual layer, and doesn't need to 
    /// consider the decoration chain itself.
    /// </remarks>
    public interface IDecorationHydrateable
    {
        string DehydrateDecoration(IGraph uow);
        void HydrateDecoration(string text, IGraph uow);
    }

    /// <summary>
    /// a generic decoration
    /// </summary>
    /// <remarks>
    /// Note that the implementors of this MUST also derive from T - which really is the whole point of a decoration.
    /// Only c# can't have a generic inherit from the generic type, so it can't be declared here.
    /// </remarks>
    public interface IDecorationOf<T> : IDecorationHydrateable 
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
    /// abstract class that provides templated implementation of a Decorator/Wrapper
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// The implementation of IHydrateable need only (de)hydrate the decoration layer itself, and not the
    /// chain of decorations (each of which will know how to (de)hydrate itself). There is no need to do Ctor-like
    /// initializations during the HydrateDecoration, as the Apply.. implementation should do this.
    /// </remarks>
    public abstract class DecorationOfBase<T> : DisposableBase, IDecorationOf<T>
    {
        #region Declarations
        private T _Decorated;
        private T _Core;
        #endregion

        #region Ctor
        protected DecorationOfBase() :base() { }
        public DecorationOfBase(T decorated)
        {
            if (decorated == null)
                throw new ArgumentNullException();

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
        #endregion

        #region Properties
        public T Decorated { get { return this._Decorated; } }
        public T Core { get { return this._Core; } }
        public abstract T This { get; }
        #endregion

        #region Helper Methods
        /// <summary>
        /// returns the first decoration that matches the filter. will never return the core item (layer 0)
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
                if (exactTypeMatch)
                {
                    if (typeof(Tdec).Equals(dec.GetType()))
                    {
                        if (filter == null)
                        {
                            return true;
                        }
                        else
                        {
                            return filter(dec);
                        }
                    }
                    return false;
                }
                else
                {
                    if (dec is Tdec)
                    {
                        if (filter == null)
                        {
                            return true;
                        }
                        else
                        {
                            return filter(dec);
                        }
                    }
                    return false;
                }
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
        public List<T> AllLayers
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
        /// replaces the Decorated member.  In effect, we are injecting a decoration immediately below the "surface"
        /// or outermost decoration.
        /// </summary>
        /// <param name="decorationStrategy"></param>
        public void ReplaceDecorated(Func<T, T> decorationStrategy)
        {
            Condition.Requires(decorationStrategy).IsNotNull();
            var newDec = decorationStrategy(this.Decorated);
            if (newDec == null) { throw new InvalidOperationException("null decoration injection"); }
            this._Decorated = newDec;

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
            List<T> decorations = this.AllLayers;
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

        #region IDecorationHydrateable
        public virtual string DehydrateDecoration(IGraph uow = null)
        {
            throw new NotImplementedException();
        }
        public virtual void HydrateDecoration(string text, IGraph uow = null)
        {
            throw new NotImplementedException();
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
