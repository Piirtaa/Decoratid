using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Core
{
    /// <summary>
    /// the base interface for something that has faces (eg. it's faceted).  
    /// </summary>
    public interface IFaceted
    {
        object GetFace(Type type);
    }
    /// <summary>
    /// base interface for the IsA paradigm that extends IFaceted
    /// </summary>
    public interface IIsA : IFaceted
    {
        Type[] IsATypes { get; }
        T GetIsA<T>();
    }

    //public interface IIsA<T1> : IIsA { }
    //public interface IIsA<T1, T2> : IIsA<T1> { }
    //public interface IIsA<T1, T2, T3> : IIsA<T1, T2> { }
    //public interface IIsA<T1, T2, T3, T4> : IIsA<T1, T2, T3> { }
    //public interface IIsA<T1, T2, T3, T4, T5> : IIsA<T1, T2, T3, T4> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6> : IIsA<T1, T2, T3, T4, T5> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7> : IIsA<T1, T2, T3, T4, T5, T6> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8> : IIsA<T1, T2, T3, T4, T5, T6, T7> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> { }
    //public interface IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : IIsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> { }


    //NOW the container classes that things will convert to

    /// <summary>
    /// The IsA type hierarchy is a way of normalizing idiomatic data (eg. decorations,IFaceted) to a defined container (the IsA)
    /// that specifies what Faces/Types the idiomatic data MUST have. 
    /// </summary>
    /// <remarks>
    /// IsA doesn't care about the idiomatic implementation just that the IFaceted instance support the specified types.
    /// Is useful as a way of defining type constraints on idiomatic data.  For instance, one could define an interface
    /// method that uses an IsA parameter type to constrain the argument to have the provided faces 
    /// (eg.  void DoSomething(IsA|T1|T2 data)).  Similarly derived types of the IsA (eg. IsA|T1|T2|T3) are always convertible
    /// to the base type (eg. IsA|T1|T2), and this lends itself to signature overloads)
    /// </remarks>
    public class IsA : IIsA
    {
        #region Ctor
        public IsA(IFaceted faceted, params Type[] isATypes)
        {
            Condition.Requires(faceted).IsNotNull();
            this.Faceted = faceted;
            this.IsATypes = isATypes;

            //validate the object supports these types
            isATypes.WithEach(x =>
            {
                var face = faceted.GetFace(x);
                Condition.Requires(face).IsNotNull();
            });
        }
        #endregion

        #region Fluent Static
        public static IsA New(IFaceted faceted)
        {
            return new IsA(faceted);
        }
        #endregion

        #region IFaceted
        public object GetFace(Type type)
        {
            return this.Faceted.GetFace(type);
        }
        #endregion

        #region IIsA
        public Type[] IsATypes { get; private set; }
        public T GetIsA<T>()
        {
            var face = this.GetFace(typeof(T));
            if (face == null)
                return default(T);

            return (T)face;
        }
        #endregion

        #region Properties
        public IFaceted Faceted { get; private set; }
        #endregion
    }

    public class IsA<T1> : IsA
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted) : base(faceted, typeof(T1)) { }

        #region Fluent Static
        public static IsA<T1> New(IFaceted faceted)
        {
            return new IsA<T1>(faceted);
        }
        #endregion
    }
    public class IsA<T1, T2> : IsA<T1>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted) : base(faceted, typeof(T1), typeof(T2)) { }
    }
    public class IsA<T1, T2, T3> : IsA<T1, T2>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted) : base(faceted, typeof(T1), typeof(T2), typeof(T3)) { }
    }
    public class IsA<T1, T2, T3, T4> : IsA<T1, T2, T3>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted) : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4)) { }
    }
    public class IsA<T1, T2, T3, T4, T5> : IsA<T1, T2, T3, T4>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted) : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6> : IsA<T1, T2, T3, T4, T5>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted) : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7> : IsA<T1, T2, T3, T4, T5, T6>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8> : IsA<T1, T2, T3, T4, T5, T6, T7>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IsA<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
        T12>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13)) { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
        T14> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
        T12, T13>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    T14, T15> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
    T12, T13, T14>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    T14, T15, T16> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
    T12, T13, T14, T15>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    T14, T15, T16, T17> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
    T12, T13, T14, T15, T16>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16), typeof(T17))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
   T14, T15, T16, T17, T18> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
   T12, T13, T14, T15, T16, T17>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
   T14, T15, T16, T17, T18, T19> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
   T12, T13, T14, T15, T16, T17, T18>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
   T14, T15, T16, T17, T18, T19, T20> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
   T12, T13, T14, T15, T16, T17, T18, T19>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
   T14, T15, T16, T17, T18, T19, T20, T21> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
   T12, T13, T14, T15, T16, T17, T18, T19, T20>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20),
            typeof(T21))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
   T14, T15, T16, T17, T18, T19, T20, T21, T22> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
   T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20),
            typeof(T21), typeof(T22))
        { }
    }
    public class IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
   T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : IsA<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,
   T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>
    {
        protected IsA(IFaceted faceted, params Type[] isATypes) : base(faceted, isATypes) { }
        public IsA(IFaceted faceted)
            : base(faceted, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
                typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
            typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20),
            typeof(T21), typeof(T22), typeof(T23))
        { }
    }

}
