using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Decoratid.Extensions;
using Decoratid.Core.Decorating;
using Decoratid.Core;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// is just special case of compound value manager
    /// </summary>
    public sealed class DecorationValueManager : INodeValueManager
    {
        public const string ID = "Decoration";

        #region Ctor
        public DecorationValueManager()
        {
            this.DoNotTraverseFilter = new Func<object, GraphPath, bool>((obj, path) =>
            {
                //skip core types
                if (path.CurrentSegment.Path.Contains("_Core "))
                    return true;
                if (path.CurrentSegment.Path.Contains("_isDisposed "))
                    return true;
                if (path.CurrentSegment.Path.Contains("_stateLock "))
                    return true;

                return false;
            });
        }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public Func<object, GraphPath, bool> DoNotTraverseFilter { get; private set; }
        #endregion

        #region INodeValueManager
        //public void RewriteNodePath(GraphPath path, object obj)
        //{
        //    GraphingUtil.RewriteBackingFieldNodePath(path);

        //    //hack the path here to change "_Decorated " to the type name
        //    if (path.CurrentSegment.Path.Contains("_Decorated "))
        //    {
        //        path.ChangeCurrentSegmentPath(obj.GetType().Name);
        //    }
        //}
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object nodeValue, GraphPath nodePath)
        {
            var rv = GraphingUtil.GetChildTraversalNodes(nodeValue, nodePath, DoNotTraverseFilter);

            return rv;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            //if it's not a decoration we can't handle it
            var genTypeDef = typeof(DecorationOfBase<>);
            if (!genTypeDef.IsInstanceOfGenericType(obj))
                return false;
            return true;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            //we return the full type of the compound type
            return LengthEncoder.LengthEncode(obj.GetType().AssemblyQualifiedName);

        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var realData = LengthEncoder.LengthDecode(nodeText);

            Type cType = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(realData);
            Condition.Requires(cType).IsNotNull();
            var obj = ReflectionUtil.CreateUninitializedObject(cType);

            //invoke ctors from the bottom up, manually
            var disposableBaseConstructor = typeof(DisposableBase).GetConstructor(Type.EmptyTypes);
            disposableBaseConstructor.Invoke(obj, null);

            return obj;
        }

        #endregion
    }
}
