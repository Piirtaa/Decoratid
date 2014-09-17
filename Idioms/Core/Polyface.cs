using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core
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
            this.Faces = new Dictionary<Type, object>();
        }
        #endregion

        #region Fluent Static
        public static Polyface New()
        {
            return new Polyface();
        }
        #endregion

        #region Properties
        private Dictionary<Type, object> Faces { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// sets the behaviour
        /// </summary>
        /// <param name="type"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public Polyface Is(Type type, object behaviour)
        {
            //registers the behaviour
            this.Faces[type] = behaviour;

            //if the behaviour is polyface compliant it registers the root
            if (behaviour is IPolyfacing)
            {
                IPolyfacing has = (IPolyfacing)behaviour;
                has.RootFace = this;
            }
            return this;
        }
        /// <summary>
        /// sets the behaviour
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public Polyface Is<T>(T behaviour)
        {
            return Is(typeof(T), behaviour);
        }
        /// <summary>
        /// gets the behaviour
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            Type type = typeof(T);

            object obj = null;
            if (this.Faces.TryGetValue(type, out obj))
                return (T)obj;

            //return null if not found
            return default(T);
            //throw new InvalidOperationException("behaviour not found");
        }
        #endregion
    }


    public static class PolyfaceExtensions
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
    }

}
