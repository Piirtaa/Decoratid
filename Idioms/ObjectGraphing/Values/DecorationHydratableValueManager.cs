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
//using System.Linq;

//namespace Decoratid.Idioms.ObjectGraphing.Values
//{
//    /// <summary>
//    /// responsibility of a decoration to provide (de)hydration for its own layer, for use by DecorationValueManager
//    /// </summary>
//    /// <remarks>
//    /// DecorationValueManager uses the decoration's Apply facility to build up, layer by layer, a
//    /// decoration.  Each decoration need only be responsible for (de)hydrating its individual layer, and doesn't need to 
//    /// consider the decoration chain itself.
//    /// </remarks>
//    public interface IDecorationHydrateable
//    {
//        string DehydrateDecoration(IGraph uow);
//        void HydrateDecoration(string text, IGraph uow);
//    }

//    public sealed class DecorationHydrateableValueManager : INodeValueManager
//    {
//        public const string ID = "DecorationHydrateable";

//        #region Ctor
//        public DecorationHydrateableValueManager() { }
//        #endregion

//        #region IHasId
//        public string Id { get { return ID; } }
//        object IHasId.Id { get { return this.Id; } }
//        #endregion

//        #region INodeValueManager
//        public bool CanHandle(object obj, IGraph uow)
//        {
//            if (obj == null)
//                return false;

//            return obj is IDecorationHydrateable;
//        }
//        public string DehydrateValue(object obj, IGraph uow)
//        {
//            Condition.Requires(obj).IsNotNull();
//            var name = obj.GetType().AssemblyQualifiedName;
//            IDecorationHydrateable s = obj as IDecorationHydrateable;
//            var data = s.DehydrateDecoration(uow);
//            return LengthEncoder.LengthEncodeList(name, data);
//        }
//        public object HydrateValue(string nodeText, IGraph uow)
//        {
//            var list = LengthEncoder.LengthDecodeList(nodeText);

//            Condition.Requires(list).HasLength(2);
//            var typeName = list.ElementAt(0);
//            var serData = list.ElementAt(1);

//            //instantiate the type, uninitialized
//            Type type = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(typeName);
//            var obj = ReflectionUtil.CreateUninitializedObject(type);

//            //since it's stringable, we use stringable's parsing to initialize
//            IDecorationHydrateable s = obj as IDecorationHydrateable;
//            s.HydrateDecoration(serData, uow);

//            return obj;
//        }
//        #endregion
//    }
//}
