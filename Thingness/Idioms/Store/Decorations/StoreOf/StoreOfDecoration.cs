﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Extensions;
using Decoratid.Thingness.Idioms.Store.Decorations.Validating;
using Decoratid.Thingness;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Thingness.Idioms.Store.Decorations.StoreOf
{
    #region  IStore Of Constructs
    /// <summary>
    /// Only allows commits of items that are of T. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteableStoreOf<T> : IValidatingStore where T : IHasId
    {
        ///// <summary>
        ///// hides base/forces implementation of validator to be IsOfValidator
        ///// </summary>
        //new IsOfValidator<T> ItemValidator { get; }
    }

    public interface IGetAllableStoreOf<T> : IGetAllableStore where T : IHasId
    {
        new List<T> GetAll();
    }

    public interface ISearchableStoreOf<T> where T : IHasId
    {
        List<T> Search(SearchFilterOf<T> filter);
    }

    /// <summary>
    /// a store restricted to items that are of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOf<T> : IStore, IWriteableStoreOf<T>, IGetAllableStoreOf<T>, ISearchableStoreOf<T> where T : IHasId
    {

    }
    #endregion

    /// <summary>
    /// Turns a store into a "storeOf".  decorates a store such that the only items that can be stored within the store are of type TItem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoreOfDecoration<T> : ValidatingDecoration, IStoreOf<T> where T : IHasId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StoreOfDecoration(IStore decorated)
            : base(decorated, new IsOfValidator<T>())
        {
        }
        #endregion

        //#region Properties
        //public new IsOfValidator<T> ItemValidator
        //{
        //    get { return base.ItemValidator as IsOfValidator<T>; }
        //    set { base.ItemValidator = value; }
        //}
        //#endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreOfDecoration<T>(store);
        }
        #endregion

        #region Overrides
        public new List<T> GetAll()
        {
            return base.GetAll().ConvertListTo<T, IHasId>();
        }
        public List<T> Search(SearchFilterOf<T> filter)
        {
            var list = base.Search<T>(filter);
            return list;
        }
        #endregion
    }
}