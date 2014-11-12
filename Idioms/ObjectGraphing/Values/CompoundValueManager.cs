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

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// is always the LAST manager in the Chain of Responsibility.  Handles objects that haven't been classified yet ->
    /// in other words, it recurses and we drill into the value's constituents, and continue classifying that way.
    /// Has list of "do not recurse" fields when recursing.
    /// </summary>
    public sealed class CompoundValueManager : INodeValueManager
    {
        public const string ID = "Compound";

        #region Ctor
        public CompoundValueManager(Func<object, GraphPath, bool> doNotTraverseFilter = null)
        {
            this.DoNotTraverseFilter = doNotTraverseFilter;
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
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object nodeValue, GraphPath nodePath)
        {
            return GraphingUtil.GetChildTraversalNodes(nodeValue, nodePath, DoNotTraverseFilter);
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
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

            return obj;
        }

        #endregion
    }
}
