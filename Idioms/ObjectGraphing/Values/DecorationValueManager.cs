using CuttingEdge.Conditions;
using Decoratid.Extensions;
using Decoratid.Idioms.Decorating;
using Decoratid.Core.Storing;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using System.Collections.Generic;
using Decoratid.Reflection;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
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
        public bool CanHandle(object obj, IGraph uow)
        {
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
                decTextList.Add(TextDecorator.LengthEncodeList(fullType, dehydLayer));
            }
            return TextDecorator.LengthEncodeList(decTextList.ToArray());
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list0 = TextDecorator.LengthDecodeList(nodeText);

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
                    UndeclaredValueManager mgr = new UndeclaredValueManager();
                    core = mgr.HydrateValue(line, uow);
                    rv = core;
                }
                else
                {
                    var list1 = TextDecorator.LengthDecodeList(line);
                    string typeName = list1[0];
                    Type cType = TypeFinder.FindAssemblyQualifiedType(typeName);
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
