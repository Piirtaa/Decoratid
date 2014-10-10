using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Polyfacing
{
    /// <summary>
    /// indicates something has a polyface/is polyface compliant.  Just really means it has a placeholder
    /// for the Root to live at, so that each "face" can fluently reference another face on the "poly face".
    /// </summary>
    public interface IPolyfacing
    {
        /// <summary>
        /// the placeholder for the polyface instance
        /// </summary>
        Polyface RootFace { get; set; }
    }

    /// <summary>
    /// A thing that has several faces
    /// </summary>
    public class Polyface
    {
        #region Ctor
        public Polyface()
        {
            this.NamedFaces = new Dictionary<string, object>();
            this.TypedFaces = new Dictionary<Type, object>();
        }
        #endregion

        #region Fluent Static
        public static Polyface New()
        {
            return new Polyface();
        }
        #endregion

        #region Properties
        private Dictionary<string, object> NamedFaces { get; set; }
        private Dictionary<Type, object> TypedFaces { get; set; }
        #endregion

        #region Named Methods
        /// <summary>
        /// Sets the behaviour for the provided face name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public Polyface Is(string face, object behaviour)
        {
            Condition.Requires(face).IsNotNullOrEmpty();

            //registers the behaviour
            this.NamedFaces[face] = behaviour;

            //if the behaviour is polyface compliant it registers the root
            if (behaviour is IPolyfacing)
            {
                IPolyfacing has = (IPolyfacing)behaviour;
                has.RootFace = this;
            }
            return this;
        }
        /// <summary>
        /// Gets the behaviour with the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object As(string name)
        {
            object obj = null;
            if (this.NamedFaces.TryGetValue(name, out obj))
                return obj;

            //return null if not found
            return null;
        }
        /// <summary>
        /// Gets the behaviour with the provided name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T As<T>(string name)
        {
            var rv = this.As(name);
            if (rv != null)
                return (T)rv;

            return default(T);
        }
        #endregion

        #region Typed Methods
        /// <summary>
        /// sets behaviour
        /// </summary>
        /// <param name="type"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public Polyface Is(Type type, object behaviour)
        {
            Condition.Requires(type).IsNotNull();

            //registers the behaviour
            this.TypedFaces[type] = behaviour;

            //if the behaviour is polyface compliant it registers the root
            if (behaviour is IPolyfacing)
            {
                IPolyfacing has = (IPolyfacing)behaviour;
                has.RootFace = this;
            }
            return this;
        }
        /// <summary>
        /// sets behavior
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public Polyface Is<T>(T behaviour)
        {
            return Is(typeof(T), behaviour);
        }
        /// <summary>
        /// gets behaviour
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            Type type = typeof(T);

            object obj = null;
            if (this.TypedFaces.TryGetValue(type, out obj))
                return (T)obj;

            //return null if not found
            return default(T);
            //throw new InvalidOperationException("behaviour not found");
        }
        #endregion
    }


    public static class PolyfacingExtensions
    {
        /// <summary>
        /// a new Polyface is created and injected.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Polyface NewRoot(this IPolyfacing face)
        {
            Condition.Requires(face).IsNotNull();
            var rv = Polyface.New();
            rv.Is(face.GetType(), face);

            return rv;
        }

        #region Typed Behaviour
        /// <summary>
        /// using the face's Root (if no Root exists, one is created), calls Polyface.Is().  In other words,
        /// it sets behaviour on Root
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Polyface Is(this IPolyfacing face, Type type, object behaviour)
        {
            Condition.Requires(face).IsNotNull();
            Polyface rv = face.RootFace;
            if (rv == null)
            {
                rv = Polyface.New();
                face.RootFace = rv;
            }
            return rv.Is(type, behaviour);
        }
        /// <summary>
        /// using the face's Root (if no Root exists, one is created), calls Polyface.Is().  In other words,
        /// it sets behaviour on Root
        /// </summary>
        public static Polyface Is<T>(this IPolyfacing face, T behaviour)
        {
            Condition.Requires(face).IsNotNull();
            Polyface rv = face.RootFace;
            if (rv == null)
            {
                rv = Polyface.New();
                face.RootFace = rv;
            }
            return rv.Is<T>(behaviour);
        }
        /// <summary>
        /// using the face's Root (if no Root exists, one is created), calls Polyface.As().  In other words,
        /// it sets behaviour on Root
        /// </summary>
        public static T As<T>(this IPolyfacing face)
        {
            Condition.Requires(face).IsNotNull();
            Polyface rv = face.RootFace;
            if (rv == null)
            {
                rv = Polyface.New();
                face.RootFace = rv;
            }
            return rv.As<T>();
        }
        #endregion

        #region Named Behaviour
        /// <summary>
        /// Sets the behaviour for the provided face name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public static Polyface Is(this IPolyfacing face, string faceName, object behaviour)
        {
            Condition.Requires(face).IsNotNull();
            Polyface rv = face.RootFace;
            if (rv == null)
            {
                rv = Polyface.New();
                face.RootFace = rv;
            }
            return rv.Is(faceName, behaviour);
        }
        /// <summary>
        /// Gets the behaviour with the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object As(this IPolyfacing face, string name)
        {
            Condition.Requires(face).IsNotNull();
            Polyface rv = face.RootFace;
            if (rv == null)
            {
                rv = Polyface.New();
                face.RootFace = rv;
            }
            return rv.As(name);
        }
        /// <summary>
        /// Gets the behaviour with the provided name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T As<T>(this IPolyfacing face, string name)
        {
            Condition.Requires(face).IsNotNull();
            Polyface rv = face.RootFace;
            if (rv == null)
            {
                rv = Polyface.New();
                face.RootFace = rv;
            }
            return rv.As<T>(name);
        }
        #endregion
    }

}
