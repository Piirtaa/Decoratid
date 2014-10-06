using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.Intercepting.Decorating
{
    /// <summary>
    /// A single layer of decoration.   
    /// </summary>
    public interface IOnionLayer : IHasId<string>
    {
        object Extension { get; }
    }

    /// <summary>
    /// A single layer of decoration - providing additional info around a core value of T.  Immutable.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class OnionLayer<T> : IOnionLayer
    {
        #region Declarations
        private readonly T _value;
        private readonly object _extension;
        private readonly string _id;
        #endregion

        #region Ctor
        /// <summary>
        /// build a decoration with just the thing being decorated
        /// </summary>
        /// <param name="val"></param>
        public OnionLayer(T val, object extension = null, string id = null)
        {
            this._id = id;
            this._extension = extension;
            this._value = val;
        }
        #endregion

        //#region ISerializable
        //protected OnionLayer(SerializationInfo info, StreamingContext context)
        //{
        //    this._id = info.GetString("_Id");
        //    this._extension = info.GetValue("_Extension", typeof(object));
        //    this._core = (T)info.GetValue("_Core", typeof(T));
        //}
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("_Id", this._id);
        //    info.AddValue("_Extension", this._extension);
        //    info.AddValue("_Core", this._core);
        //}
        //#endregion

        #region Properties
        public T Value { get { return this._value; } }
        public object Extension { get { return this._extension; } }
        #endregion

        #region IHasId
        public string Id
        {
            get { return this._id; }
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Fluent Static
        public static OnionLayer<T> New(T val, object extension = null, string id = null)
        {
            var rv = new OnionLayer<T>(val, extension, id);
            return rv;
        }
        #endregion
    }

    /// <summary>
    /// A set of DecorationLayers (eg. an onion) around a core value of T.  Has ability to mutate itself (ie. agglomerate layers).
    /// The purpose of this class is to basically agglomerate data and change a value T via layers.  Kind of like an audit trail
    /// of data over time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Onion<T> 
    {
        #region Declarations
        private readonly LinkedList<OnionLayer<T>> _decorations = new LinkedList<OnionLayer<T>>();
        #endregion

        #region Ctor
        /// <summary>
        /// build a decoration with just the thing being decorated
        /// </summary>
        /// <param name="core"></param>
        public Onion(T core)
        {
            OnionLayer<T> coreItem = new OnionLayer<T>(core);
            this._decorations.AddFirst(coreItem);
        }
        #endregion

        //#region ISerializable
        //protected Onion(SerializationInfo info, StreamingContext context)
        //{
        //    this._decorations = (LinkedList<OnionLayer<T>>)info.GetValue("_Decorations", typeof(LinkedList<OnionLayer<T>>));
        //}
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("_Decorations", this._decorations);
        //}
        //#endregion

        #region Calculated Properties
        /// <summary>
        /// the last, decorated, core value.
        /// </summary>
        public T LastValue
        {
            get
            {
                var node = this._decorations.Last.Value;
                return node.Value;
            }
        }
        /// <summary>
        /// the first value
        /// </summary>
        public T FirstValue
        {
            get
            {
                var node = this._decorations.First.Value;
                return node.Value;
            }
        }
        /// <summary>
        /// decorations from least dependent (core) to most (outer layer)
        /// </summary>
        public List<OnionLayer<T>> Decorations
        {
            get
            {
                List<OnionLayer<T>> returnValue = new List<OnionLayer<T>>();
                this._decorations.WithEach(x =>
                {
                    returnValue.Add(x);
                });
                return returnValue;
            }
        }
        #endregion

        #region Mutate Methods
        public void AddLayer(OnionLayer<T> layer)
        {
            Condition.Requires(layer).IsNotNull();
            this._decorations.AddLast(layer);
        }
        /// <summary>
        /// adds a layer to the decoration with no modification of the core data
        /// </summary>
        /// <param name="core"></param>
        /// <param name="extension"></param>
        /// <param name="id"></param>
        public void AddUnmodifiedLayer(object extension, string id = null)
        {
            this.AddLayer(this.LastValue, extension, id);
        }
        /// <summary>
        /// adds another layer to the decoration, but provides a modification to the core data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public void AddLayer(T core, object extension, string id = null)
        {
            OnionLayer<T> item = new OnionLayer<T>(core, extension, id);
            this._decorations.AddLast(item);
        }
        #endregion

        #region Methods
        /// <summary>
        /// gets the specified layer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OnionLayer<T> GetDecorationById(string id)
        {
            return this.Decorations.First((x) =>
            {
                if (x == null)
                    return false;

                return x.Id.Equals(id);
            });
        }
        /// <summary>
        /// gets the specified layer's extension value 
        /// </summary>
        /// <typeparam name="TExt"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public TExt GetDecorationExtension<TExt>(string id)
        {
            var dec = this.GetDecorationById(id);
            return (TExt)dec.Extension;
        }
        /// <summary>
        /// finds the first decoration (least dep to most) with an extension of type TExt
        /// </summary>
        /// <typeparam name="TExt"></typeparam>
        /// <returns></returns>
        public TExt GetDecorationExtension<TExt>()
        {
            foreach (var each in this.Decorations)
            {
                if (each == null)
                    continue;

                if (each.Extension.GetType().IsAssignableFrom(typeof(TExt)))
                    return (TExt)each.Extension;
            }
            return default(TExt);
        }

        #endregion

        #region Fluent Static 
        public static Onion<T> New(T core)
        {
            var rv = new Onion<T>(core);
            return rv;
        }
        #endregion
    }
}
