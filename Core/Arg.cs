using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Core
{
    /*What's the deal with Arg?
     * It's a non-decorated container that supports IFaceted, that one can use to build up Args for ILogic.
     * Essentially a tuple.
     * 
     * Unlike normal decorations that only apply to some types (and thus can be "organically" grown, and still maintain
     * a Ness-y integrity), we may have a bunch of disconnected pieces of data that aren't really related in any Ness-y, semantic 
     * way.  This container lets us aggregate these types under one IFaceted umbrella without requiring any of that 
     * hippy organicsauce.
     * 
     */ 

    /// <summary>
    /// a tuple type of container of data that can be grown with more data, dynamically
    /// </summary>
    public class Arg : IFaceted, IHasId
    {
        #region Ctor
        /// <summary>
        /// copy ctor
        /// </summary>
        /// <param name="arg"></param>
        public Arg(Arg arg)
        {
            this.Faces = arg.Faces;
        }

        public Arg(params object[] args)
        {
            this.Faces = new Dictionary<Type, object>();

            args.WithEach(x =>
            {
                if (x != null)
                {
                    if (x is IHasId)
                    {
                        this.Faces[typeof(IHasId)] = x;
                    }
                    else
                    {
                        var type = x.GetType();
                        this.Faces[type] = x;
                    }
                }
            });
        }
        #endregion

        #region Fluent Static
        public static Arg New(params object[] args)
        {
            return new Arg(args);
        }
        #endregion

        #region IHasId
        /// <summary>
        /// this points to the IHasId face, otherwise kacks
        /// </summary>
        public object Id
        {
            get
            {
                IHasId hasId = this.GetFace(typeof(IHasId)) as IHasId;
                Condition.Requires(hasId).IsNotNull();
                return hasId.Id;
            }
        }
        #endregion

        #region IFaceted
        public object GetFace(Type type)
        {
            return this.Faces[type];
        }
        public List<object> GetFaces()
        {
            return this.Faces.Values.ToList();
        }
        #endregion

        #region Properties
        protected Dictionary<Type, object> Faces { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// fluent add method
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public Arg AddValue(object val)
        {
            if (val != null)
                this.Faces[val.GetType()] = val;

            return this;
        }
        public Arg Is<T>(T val)
        {
            if (val != null)
                this.Faces[typeof(T)] = val;

            return this;
        }
        public T As<T>()
        {
            var face = (this as IFaceted).GetFace(typeof(T));
            if (face == null)
                return default(T);

            return (T)face;
        }
        #endregion
    }

    /// <summary>
    /// a tuple type of container of data that can be grown with more data, dynamically
    /// </summary>
    public class Arg<T> : Arg, IHasId
    {
        #region Ctor
        public Arg(T arg)
            : base(arg)
        {
        }
        #endregion

        #region Fluent Static
        public static Arg<T> New(T arg)
        {
            return new Arg<T>(arg);
        }
        #endregion
    }

    public class Arg<T1, T2> : Arg
    {
        public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

        #region Fluent Static
        public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
        {
            return new Arg<T1, T2>(arg1, arg2);
        }
        #endregion
    }
    public class Arg<T1, T2, T3> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3) : base(arg1, arg2, arg3) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3> New(T1 arg1, T2 arg2, T3 arg3)
        {
            return new Arg<T1, T2, T3>(arg1, arg2, arg3);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(arg1, arg2, arg3, arg4) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return new Arg<T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(arg1, arg2, arg3, arg4, arg5) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return new Arg<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5, T6> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) : base(arg1, arg2, arg3, arg4, arg5, arg6) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5, T6> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return new Arg<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5, T6, T7> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5, T6, T7> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7)
        {
            return new Arg<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5, T6, T7, T8> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5, T6, T7, T8> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8)
        {
            return new Arg<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9)
        {
            return new Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9, T10 arg10)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return new Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return new Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
                arg11);
        }
        #endregion
    }
    public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : Arg
    {
        public Arg(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
            : base(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) { }

        #region Fluent Static
        public new static Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> New(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return new Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,
                arg11, arg12);
        }
        #endregion
    }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    //     T14> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    // T14, T15> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    // T14, T15, T16> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    // T14, T15, T16, T17> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    //T14, T15, T16, T17, T18> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    //T14, T15, T16, T17, T18, T19> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    //T14, T15, T16, T17, T18, T19, T20> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    //T14, T15, T16, T17, T18, T19, T20, T21> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    //T14, T15, T16, T17, T18, T19, T20, T21, T22> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
    // public class Arg<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
    //T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : Arg
    // {
    //     public Arg(T1 arg1, T2 arg2) : base(arg1, arg2) { }

    //     #region Fluent Static
    //     public new static Arg<T1, T2> New(T1 arg1, T2 arg2)
    //     {
    //         return new Arg<T1, T2>(arg1, arg2);
    //     }
    //     #endregion
    // }
}
