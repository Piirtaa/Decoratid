//using CuttingEdge.Conditions;
//using Decoratid.Core.Decorating;
//using Decoratid.Core.Identifying;
//using Decoratid.Extensions;
//using Decoratid.Idioms.Stringing;
//using Decoratid.Idioms.TypeLocating;
//using Decoratid.Utils;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;

//namespace Decoratid.Idioms.ObjectGraphing.Values
//{
//    /// <summary>
//    /// handles Decorations (instances of DecorationOfBase).  
//    /// </summary>
//    public sealed class DecorationValueManager : INodeValueManager
//    {
//        public const string ID = "Decoration";

//        #region Ctor
//        public DecorationValueManager() { }
//        #endregion

//        #region IHasId
//        public string Id { get { return ID; } }
//        object IHasId.Id { get { return this.Id; } }
//        #endregion

//        #region INodeValueManager
//        /// <summary>
//        /// given a decoration of an unknown type, gets all of the layers of the decoration
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        private IEnumerable GetDecorationList(object obj)
//        {
//            //get the generic type we're decorating
//            var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

//            //get all the decorations via a reflection call 
//            var genTypeDef = typeof(DecorationOfBase<>);
//            var decType = genTypeDef.MakeGenericType(genType);
//            PropertyInfo pi = decType.GetProperty("OutermostToCore", BindingFlags.Instance | BindingFlags.Public);
//            Condition.Requires(pi).IsNotNull();
//            var decList = pi.GetValue(obj);
//            IEnumerable list = decList as IEnumerable;
//            return list;
//        }
//        public bool CanHandle(object obj, IGraph uow)
//        {
//            if (obj == null)
//                return false;

//            //if it's not a decoration we can't handle it
//            var genTypeDef = typeof(DecorationOfBase<>);
//            if (!genTypeDef.IsInstanceOfGenericType(obj))
//                return false;

//            //if any of the layers of the decoration don't have value managers, we can't handle it
//            var list = this.GetDecorationList(obj);

//            //we delegate responsibility to classify the value managers of each layer 
//            //but we exclude DecorationValueManager and UndeclaredValueManager to be returned
//            UndeclaredValueManager undeclaredMgr = new UndeclaredValueManager(UndeclaredValueManager.ID, DecorationValueManager.ID);

//            foreach (var each in list)
//                if (!undeclaredMgr.CanHandle(each, uow))
//                    return false;

//            return true;
//        }

//        public string DehydrateValue(object obj, IGraph uow)
//        {
//            var list = this.GetDecorationList(obj);

//            //we delegate responsibility to classify the value managers of each layer 
//            //but we exclude DecorationValueManager and UndeclaredValueManager to be returned
//            UndeclaredValueManager undeclaredMgr = new UndeclaredValueManager(UndeclaredValueManager.ID, DecorationValueManager.ID);

//            //init the list of strings to hold each layer's dehydrated value
//            var decTextList = new List<string>();

//            //for each decoration, dehydrate it
//            foreach (var each in list)
//            {
//                //test if the layer can be handled
//                if (!undeclaredMgr.CanHandle(each, uow))
//                    throw new InvalidOperationException("cannot locate value manager for decoration layer " + each.GetType());

//                var fullType = each.GetType().AssemblyQualifiedName;
//                string dehydLayer = undeclaredMgr.DehydrateValue(each, uow);
//                decTextList.Add(LengthEncoder.LengthEncodeList(fullType, dehydLayer));
//            }
//            return LengthEncoder.LengthEncodeList(decTextList.ToArray());
//        }
//        public object HydrateValue(string nodeText, IGraph uow)
//        {
//            var list0 = LengthEncoder.LengthDecodeList(nodeText);


//            //declare the decoration we're returning
//            object rv = null;

//            //for each decoration starting at the core
//            list0.Reverse();

//            //define some managers to assist with serializing
//            UndeclaredValueManager undeclaredMgr = new UndeclaredValueManager();

//            list0.WithEach(line =>
//            {
//                //hydrate each layer
//                var layer = undeclaredMgr.HydrateValue(line, uow);

//                //if we have a returnvalue declared we use the decoration's apply facility to wrap
//                //if we don't have a returnvalue we set it to the layer
//                if (rv == null)
//                {
//                    rv = layer;
//                }
//                else
//                {
//                    //apply the decoration
//                    var mi = layer.GetType().GetMethod("ApplyThisDecorationTo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//                    object[] args = new object[] { rv };
//                    rv = mi.Invoke(layer, args);
//                }
//            });
//            return rv;
//        }

//        public string DehydrateValue(object obj, IGraph uow)
//        {
//            List<string> ignoreMgrIds = new List<string>();
//            ignoreMgrIds.Add(UndeclaredValueManager.ID);
//            if (this.ManagerIdsToIgnore != null)
//                ignoreMgrIds.AddRange(this.ManagerIdsToIgnore);

//            var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow, ignoreMgrIds.ToArray());

//            if (mgr == null)
//                return null;

//            var data = mgr.DehydrateValue(obj, uow);
//            return LengthEncoder.LengthEncodeList(mgr.Id, data);
//        }
//        public object HydrateValue(string nodeText, IGraph uow)
//        {
//            if (string.IsNullOrEmpty(nodeText))
//                return null;

//            var list = LengthEncoder.LengthDecodeList(nodeText);
//            Condition.Requires(list).HasLength(2);

//            var mgr = uow.ChainOfResponsibility.GetValueManagerById(list.ElementAt(0));

//            if (mgr == null)
//                return null;

//            //if the chain of responsibility produces This as manager, we're in an infinite loop situation and should back out
//            if (mgr != null && mgr is UndeclaredValueManager)
//                return null;

//            var obj = mgr.HydrateValue(list.ElementAt(1), uow);
//            return obj;
//        }
//        #endregion

//    }
//}
