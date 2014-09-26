using Decoratid.Idioms.Core;
using Decoratid.Idioms.ObjectGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// defines a stringablelist decoration.
    /// NOTE: the decoration hydration mechanism SHOULD enable conversion to and from the raw value.
    /// For example, Hydrate() should take in the encoded value, and the raw value would 
    /// Dehydrate() should output the encoded value.
    /// </summary>
    public interface IStringableListDecoration : IStringableList, IDecorationOf<IStringableList> { }

    /// <summary>
    /// base class implementation of a IStringableList decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class StringableListDecorationBase : DecorationOfBase<IStringableList>, IStringableList
    {
        #region Ctor
        public StringableListDecorationBase(IStringableList decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected StringableListDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var data = info.GetString("data");
            this.Parse(data);
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", this.GetValue());
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public virtual string GetValue()
        {
            return base.Decorated.GetValue();
        }
        public virtual void Parse(string text)
        {
            base.Decorated.Parse(text);
        }
        public override IStringableList This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IStringableList> ApplyThisDecorationTo(IStringableList thing)
        {
            throw new NotImplementedException();
        }
        #endregion

        //#region IDecorationHydrateable
        //public override string DehydrateDecoration(IGraph uow = null)
        //{
        //    return this.GetValue();
        //}
        //public override void HydrateDecoration(string text, IGraph uow = null)
        //{
        //    this.Parse(text);
        //}
        //#endregion

        #region IList
        public int IndexOf(string item)
        {
            return this.Decorated.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            this.Decorated.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.Decorated.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return this.Decorated[index];
            }
            set
            {
                this.Decorated[index] = value;
            }
        }

        public void Add(string item)
        {
            this.Decorated.Add(item);
        }

        public void Clear()
        {
            this.Decorated.Clear();
        }

        public bool Contains(string item)
        {
            return this.Decorated.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            this.Decorated.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.Decorated.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.Decorated.IsReadOnly; }
        }

        public bool Remove(string item)
        {
            return this.Decorated.Remove(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.Decorated.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
