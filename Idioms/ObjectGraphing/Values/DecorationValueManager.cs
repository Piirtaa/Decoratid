using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// handles Decorations (instances of DecorationOfBase)
    /// </summary>
    public sealed class DecorationValueManager : INodeValueManager
    {
        public const string ID = "Decoration";

        #region Ctor
        public DecorationValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        private IEnumerable GetDecorationList(object obj)
        {
            //get the generic type we're decorating
            var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

            //get all the decorations via a reflection call 
            var genTypeDef = typeof(DecorationOfBase<>);
            var decType = genTypeDef.MakeGenericType(genType);
            PropertyInfo pi = decType.GetProperty("OutermostToCore", BindingFlags.Instance | BindingFlags.Public);
            Condition.Requires(pi).IsNotNull();
            var decList = pi.GetValue(obj);
            IEnumerable list = decList as IEnumerable;
            return list;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            //if it's not a decoration we can't handle it
            var genTypeDef = typeof(DecorationOfBase<>);
            if (!genTypeDef.IsInstanceOfGenericType(obj))
                return false;

            //if any of the layers of the decoration don't have value managers, we can't handle it
            var list = this.GetDecorationList(obj);

            //define some managers to assist with serializing
            UndeclaredValueManager undeclaredMgr = new UndeclaredValueManager();

            foreach (var each in list)
                if (!undeclaredMgr.CanHandle(each, uow))
                    return false;

            return true;
        }

        public string DehydrateValue(object obj, IGraph uow)
        {
            var list = this.GetDecorationList(obj);

            //define some managers to assist with serializing
            UndeclaredValueManager undeclaredMgr = new UndeclaredValueManager();

            //init the list of strings to hold each layer's dehydrated value
            var decTextList = new List<string>();

            //for each decoration, dehydrate it
            foreach (var each in list)
            {
                //test if the layer can be handled
                if (!undeclaredMgr.CanHandle(each, uow))
                    throw new InvalidOperationException("cannot locate value manager for decoration layer " + each.GetType());

                var fullType = each.GetType().AssemblyQualifiedName;
                string dehydLayer = undeclaredMgr.DehydrateValue(each, uow);
                decTextList.Add(LengthEncoder.LengthEncodeList(fullType, dehydLayer));
            }
            return LengthEncoder.LengthEncodeList(decTextList.ToArray());
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list0 = LengthEncoder.LengthDecodeList(nodeText);


            //declare the decoration we're returning
            object rv = null;

            //for each decoration starting at the core
            list0.Reverse();

            //define some managers to assist with serializing
            UndeclaredValueManager undeclaredMgr = new UndeclaredValueManager();

            list0.WithEach(line =>
            {
                //hydrate each layer
                var layer = undeclaredMgr.HydrateValue(line, uow);

                //if we have a returnvalue declared we use the decoration's apply facility to wrap
                //if we don't have a returnvalue we set it to the layer
                if (rv == null)
                {
                    rv = layer;
                }
                else
                {
                    //apply the decoration
                    var mi = layer.GetType().GetMethod("ApplyThisDecorationTo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    object[] args = new object[] { rv };
                    rv = mi.Invoke(layer, args);
                }
            });
            return rv;
        }
        #endregion

    }
}
