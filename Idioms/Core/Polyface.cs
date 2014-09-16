using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core
{
    /// <summary>
    /// indicates something has a polyface/is polyface compliant
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
        public Polyface AddBehaviour(Type type, object behaviour)
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
        public Polyface AddBehaviour<T>(T behaviour)
        {
            return AddBehaviour(typeof(T), behaviour);
        }
        /// <summary>
        /// retrieves behaviour from the Behaver
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
            rv.AddBehaviour(face.GetType(), face);

            return rv;
        }

        
    }

}
