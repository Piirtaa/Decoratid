
using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// Handles instances that are stringable by delegating their hydration to themselves
    /// </summary>
    public sealed class StringableValueManager : INodeValueManager
    {
        public const string ID = "Stringable";

        #region Ctor
        public StringableValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public void RewriteNodePath(GraphPath path, object obj)
        {
            GraphingUtil.RewriteBackingFieldNodePath(path);
        }
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return obj is IStringable;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var name = obj.GetType().AssemblyQualifiedName;
            IStringable s = obj as IStringable;
            var data = s.GetValue();
            return LengthEncoder.LengthEncodeList(name, data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = LengthEncoder.LengthDecodeList(nodeText);

            Condition.Requires(list).HasLength(2);
            var typeName = list.ElementAt(0);
            var serData = list.ElementAt(1);
            
            //instantiate the type, uninitialized
            Type type = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(typeName);
            var obj = ReflectionUtil.CreateUninitializedObject(type);
            
            //since it's stringable, we use stringable's parsing to initialize
            IStringable s = obj as IStringable;
            s.Parse(serData);

            return obj;
        }
        #endregion

    }
}
