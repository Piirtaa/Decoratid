//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CuttingEdge.Conditions;
//using Sandbox.Thingness;
//using Sandbox.Extensions;

//namespace SandboXX.Whatever
//{
//    /// <summary>
//    /// a XX decoration
//    /// </summary>
//    public interface IDecoratedXX : IXX
//    {
//        /// <summary>
//        /// the thing we're decorating
//        /// </summary>
//        IXX Decorated { get; }

//        /// <summary>
//        /// Essentially is a clone mechanism.  Allow the current decoration to recreate an instance like itself when
//        /// provided with a XX to decorate - think of this as a ctor with only one arg (the XX) and all other args
//        /// coming from the current instance.
//        /// </summary>
//        /// <param name="XX"></param>
//        /// <returns></returns>
//        IDecoratedXX ApplyDecoration(IXX thing);
//    }

//    /// <summary>
//    /// abstract class that provides templated implementation of a Decorator/Wrapper of IXX
//    /// </summary>
//    public abstract class DecoratedXXBase : DisposableBase, IDecoratedXX
//    {
//        #region Declarations
//        private readonly IXX _Decorated;
//        #endregion

//        #region Ctor
//        /// <summary>
//        /// ctor.  requires IXX to wrap
//        /// </summary>
//        /// <param name="decorated"></param>
//        public DecoratedXXBase(IXX decorated)
//        {
//            Condition.Requires(decorated).IsNotNull();
//            this._Decorated = decorated;

//            //set the core registry.  events are wired to this
//            this._core = GetCore();
//        }
//        #endregion

//        #region Properties
//        public IXX Decorated { get { return this._Decorated; } }
//        #endregion

//        #region Core Registry
//        /// <summary>
//        /// a reference to the actual registry being decorated (eg. the centre-most, core layer)
//        /// </summary>
//        protected IXX _core = null;
//        /// <summary>
//        /// walks the decorators and returns the core registry (the first one that's not an IDecoratedRegistry)
//        /// </summary>
//        /// <returns></returns>
//        protected IXX GetCore()
//        {
//            IXX currentLayer = this;
//            while (currentLayer != null)
//            {
//                //recurse if it's decorated
//                if (currentLayer is IDecoratedXX)
//                {
//                    IDecoratedXX layer = (IDecoratedXX)currentLayer;
//                    currentLayer = layer.Decorated;
//                }
//                else
//                {
//                    break;
//                }
//            }
//            return currentLayer;
//        }
//        #endregion

//        #region Helper Methods
//        /// <summary>
//        /// returns the first decorated XX that matches the filter. 
//        /// </summary>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        protected IXX WalkDecorated(Func<IXX, bool> filter)
//        {
//            IXX currentLayer = this;

//            //iterate down
//            while (currentLayer != null)
//            {
//                //check filter.  break/return
//                if (filter(currentLayer))
//                {
//                    return currentLayer;
//                }

//                //recurse if it's decorated
//                if (currentLayer is IDecoratedXX)
//                {
//                    IDecoratedXX layer = (IDecoratedXX)currentLayer;
//                    currentLayer = layer.Decorated;
//                }
//                else
//                {
//                    //not decorated, and fails the filter?  stop here
//                    return null;
//                }
//            }

//            return null;
//        }
//        /// <summary>
//        /// walks the decorator hierarchy to find the one of the provided type
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        protected T FindDecoratorOf<T>(bool exactTypeMatch, Func<IXX, bool> filter = null) where T : IDecoratedXX
//        {
//            var match = this.WalkDecorated((dec) =>
//            {
//                if (exactTypeMatch)
//                {
//                    if (typeof(T).Equals(dec.GetType()))
//                    {
//                        if (filter == null)
//                        {
//                            return true;
//                        }
//                        else
//                        {
//                            return filter(dec);
//                        }
//                    }
//                    return false;
//                }
//                else
//                {
//                    if (dec is T)
//                    {
//                        if (filter == null)
//                        {
//                            return true;
//                        }
//                        else
//                        {
//                            return filter(dec);
//                        }
//                    }
//                    return false;
//                }
//            });

//            if (match == null)
//            {
//                return default(T);
//            }

//            return (T)match;
//        }
//        /// <summary>
//        /// walks the decorations from outermost to core
//        /// </summary>
//        /// <returns></returns>
//        protected List<IDecoratedXX> GetDecorations()
//        {
//            List<IDecoratedXX> returnValue = new List<IDecoratedXX>();

//            var match = this.WalkDecorated((reg) =>
//            {
//                if (reg != null && reg is IDecoratedXX)
//                    returnValue.Add(reg as IDecoratedXX);
//                return false;
//            });

//            return returnValue;
//        }
//        #endregion

//        #region Methods

//        /// <summary>
//        /// looks for the provided decoration layer and tries to build the same XX without this decoration.
//        /// </summary>
//        /// <remarks>
//        /// if there is a dependency of one layer upon the other, and the dependency is removed, the ctor 
//        /// chain should kack - we provide the same checks as a ctor does, in this method. 
//        /// </remarks>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public IDecoratedXX Undecorate<T>(bool exactTypeMatch, Func<IXX, bool> filter = null) where T : IDecoratedXX
//        {
//            //find the decoration we want to remove
//            IDecoratedXX decorationToRemove = this.FindDecoratorOf<T>(exactTypeMatch, filter);
//            Condition.Requires(decorationToRemove).IsNotNull("decoration not found");

//            //get the decorations from the inside out
//            List<IDecoratedXX> decorations = this.GetDecorations();
//            decorations.Reverse();

//            //iterate to the decoration to remove
//            IXX wrappee = null;
//            foreach (var each in decorations)
//            {
//                //set the flag when we're at the right indeXX
//                if (each == decorationToRemove)
//                {
//                    wrappee = each.Decorated;
//                    continue;
//                }

//                //skip if the flag isn't set
//                if (wrappee == null)
//                    continue;

//                //we're at the decoration above the decoration to remove
//                //we want to add all the remaining layers on
//                //NOTE: this should kack if invalid
//                wrappee = each.ApplyDecoration(wrappee);
//            }

//            return wrappee as IDecoratedXX;
//        }
//        #endregion

//        #region IDecoratedXX
//        public abstract IDecoratedXX ApplyDecoration(IXX XX);
//        #endregion

//        #region Disposable
//        protected override void DisposeManaged()
//        {
//            //dispose the wrapper
//            if (this.Decorated != null && this.Decorated is IDisposable)
//            {
//                ((IDisposable)(this.Decorated)).Dispose();
//            }
//            base.DisposeManaged();
//        }
//        #endregion
//    }
//}
