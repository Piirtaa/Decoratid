using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CuttingEdge.Conditions;
using Storidiom.Thingness.Idioms.Store;
using Storidiom.Thingness.Idioms.Store.CoreStores;
using Storidiom.Extensions;
using System.Reflection;
using Storidiom.Thingness.Decorations;

namespace Storidiom.Thingness
{
    /// <summary>
    /// Does decoration 2 store conversions.  Similar to Object2StoreConverter but specialized to handle 
    /// the decoration layers explicitly.   DOES NOT SERIALIZE - store data serialization can do this easily enough, however.
    /// </summary>
    public static class Decoration2StoreConverter
    {
        private const string GENERIC_TYPE_KEY = "+genericType";
        private const string CORE_KEY = "+core";
        private const string LAST_LAYER_TYPE_KEY = "+lastLayerType";

        #region Conversion
        /// <summary>
        /// serializes the decoration chain (assuming this is the topmost) into a store
        /// </summary>
        /// <returns></returns>
        public static InMemoryStore ConvertToStore<T>(DecorationOfBase<T> decoration)
        {
            /* 1.each layer of the decoration is converted to a store and stored with id equal to the layer number
             * 2.the type of the outermost layer is stored with id LAST_LAYER_TYPE_KEY
             * 3.the generic/core type is stored with id GENERIC_TYPE_KEY
             * 4.the core is stored with id CORE_KEY
             * 5.each layer's type is stored with the id equal to the layer number
             */

            //start with all decorations from core to outer
            InMemoryStore store = new InMemoryStore();

            //Req3. SAVE the type of T
            var genericItemEntry = GENERIC_TYPE_KEY.BuildContextualAsId<string, Type>(typeof(T));
            store.SaveItem(genericItemEntry);

            //with each decoration, dehydrate it/ add it to the return store
            var decs = decoration.AllLayers;
            decs.Reverse();

            int count = 0;
            decs.WithEach(x =>
            {
                string id = count.ToString(); count++;

                //5. SAVE the decoration type by index
                var decTypeEntry = id.BuildContextualAsId<string, Type>(x.GetType());
                store.SaveItem(decTypeEntry);

                if (x is IDecorationOf<T>)
                {
                    //Req 1. convert each decoration layer into a store, and save with the id of the index
                    IDecorationOf<T> dec = (IDecorationOf<T>)x;
                    var decStore = dec.ConvertToStore();
                    var decEntry = id.BuildContextualAsId<string, InMemoryStore>(decStore);
                    store.SaveItem(decEntry);
                }
                else
                {
                    //Req 4. we're at the core, so add it, too
                    var coreEntry = (CORE_KEY).BuildContextualAsId<string, T>(x);
                    store.SaveItem(coreEntry);
                }
            });

            //Req 2. SAVE the type of the last layer
            var lastLayer = (LAST_LAYER_TYPE_KEY).BuildContextualAsId<string, Type>(decs.Last().GetType());
            store.SaveItem(lastLayer);

            return store;
        }

        /// <summary>
        /// hydrates a decoration chain from a store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="obj"></param>
        /// <param name="filter"></param>
        public static void HydrateFromStore<T>(IStore store, IDecorationOf<T> obj)
        {
            //build the decoration from scratch

            //inject 
        }
        /// <summary>
        /// deserializes the results from ConvertToStore and builds the decoration chain
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static IDecorationOf<T> ConvertFromStore<T>(IStore store)
        {
            //get the core
            var coreItem = store.Get<ContextualAsId<string, T>>(CORE_KEY);
            T core = coreItem.Context;

            int count = 1;//start after the core
            IDecorationOf<T> currentDecoration = null;

            while (true)
            {
                string id = count.ToString(); count++;

                //get the decoration type by index, if it doesn't exist we're at the end
                var type = store.Get<ContextualAsId<string, Type>>(id);
                if (type == null)
                    break;

                //create it
                T layer = (T)New.Create(type.Context);
                IDecorationOf<T> dec = layer as IDecorationOf<T>;
                Condition.Requires(dec).IsNotNull();

                //get the store version of the layer
                var decLayerStore = store.Get<ContextualAsId<string, InMemoryStore>>(id);
                //use it to hydrate this newly instantiated layer
                dec.HydrateFromStore(decLayerStore.Context);

                //now apply the new decoration to the current decoration, and build up in this fashion
                if (currentDecoration == null)
                {
                    currentDecoration = dec.ApplyThisDecorationTo(core);
                }
                else
                {
                    currentDecoration = dec.ApplyThisDecorationTo(currentDecoration.This);
                }
            }
            return currentDecoration;
        }
        /// <summary>
        /// deserializes the results from DehydrateDecorationsToStore and builds the decoration chain
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static object NonGeneric_ConvertFromStore(IStore store)
        {
            //get the generic type
            var genericTypeEntry = store.Get<ContextualAsId<string, Type>>(GENERIC_TYPE_KEY);
            Condition.Requires(genericTypeEntry).IsNotNull();

            Type genericType = genericTypeEntry.Context;

            var mi = typeof(Decoration2StoreConverter).GetMethod("ConvertFromStore", BindingFlags.Static | BindingFlags.Public);
            var mi2 = mi.MakeGenericMethod(genericType);
            var res = mi2.Invoke(null, new object[] { store });
            return res;
        }
        #endregion
    }

    public static class DecorationExtensions
    {
        /// <summary>
        /// converts a decoration into its Store Format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static InMemoryStore ConvertToStore<T>(this DecorationOfBase<T> dec)
        {
            return Decoration2StoreConverter.ConvertToStore(dec);
        }
        /// <summary>
        /// converts a store (ie. a decoration in Store Format), back to the decoration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <returns></returns>
        public static IDecorationOf<T> ConvertFromStore<T>(IStore store)
        {
            return Decoration2StoreConverter.ConvertFromStore<T>(store);
        }
    }
}
