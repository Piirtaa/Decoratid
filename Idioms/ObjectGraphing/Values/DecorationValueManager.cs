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
    /// responsibility of a decoration to provide (de)hydration for its own layer, for use by DecorationValueManager
    /// </summary>
    /// <remarks>
    /// DecorationValueManager uses the decoration's Apply facility to build up, layer by layer, a
    /// decoration.  Each decoration need only be responsible for (de)hydrating its individual layer, and doesn't need to 
    /// consider the decoration chain itself.
    /// </remarks>
    public interface IDecorationHydrateable
    {
        string DehydrateDecoration(IGraph uow);
        void HydrateDecoration(string text, IGraph uow);
    }

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
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            var genTypeDef = typeof(DecorationOfBase<>);

            var rv = genTypeDef.IsInstanceOfGenericType(obj);
            return rv;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            //get the generic type we're decorating
            var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

            //get all the decorations via a reflection call 
            var genTypeDef = typeof(DecorationOfBase<>);
            var decType = genTypeDef.MakeGenericType(genType);
            PropertyInfo pi = decType.GetProperty("AllLayers", BindingFlags.Instance | BindingFlags.Public);
            Condition.Requires(pi).IsNotNull();
            var decList = pi.GetValue(obj);
            IEnumerable list = decList as IEnumerable;

            //init the list of strings to hold each layer's dehydrated value
            var decTextList = new List<string>();

            //for each decoration, dehydrate it
            foreach (var each in list)
            {
                var fullType = each.GetType().AssemblyQualifiedName;

                string dehydLayer = string.Empty;

                //all decorations are IDecorationHydrateable.  The core may not be.  if it isn't we just use a New instance
                if (each is IDecorationHydrateable)
                {
                    IDecorationHydrateable hyd = each as IDecorationHydrateable;
                    dehydLayer = hyd.DehydrateDecoration(uow);
                }
                else
                {
                    //we're at the core, ask the graph to figure out how to dehydrate
                    UndeclaredValueManager mgr = new UndeclaredValueManager();
                    dehydLayer = mgr.DehydrateValue(each, uow);
                }
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

            object core = null;

            list0.WithEach(line =>
            {
                //init the core
                if (core == null)
                {
                    //delegate back to the graph
                    UndeclaredValueManager mgr = new UndeclaredValueManager();
                    core = mgr.HydrateValue(line, uow);
                    rv = core;
                }
                else
                {
                    var list1 = LengthEncoder.LengthDecodeList(line);
                    string typeName = list1[0];
                    Type cType = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(typeName);
                    Condition.Requires(cType).IsNotNull();

                    //create an uninitialized
                    var obj = ReflectionUtil.CreateUninitializedObject(cType);

                    //if the object is IDecorationHydrateable
                    if (!(obj is IDecorationHydrateable))
                        throw new InvalidOperationException("decoration expected");

                    Condition.Requires(list1.Count == 2);
                    IDecorationHydrateable hyd = obj as IDecorationHydrateable;
                    hyd.HydrateDecoration(list1[1], uow);

                    //apply the decoration
                    var mi = obj.GetType().GetMethod("ApplyThisDecorationTo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    object[] args = new object[] { rv };
                    rv = mi.Invoke(obj, args);
                }
            });
            return rv;
        }
        #endregion

    }
}
