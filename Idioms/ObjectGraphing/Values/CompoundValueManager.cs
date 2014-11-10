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
        public CompoundValueManager(params string[] doNotRecurseFields) 
        {
            this.DoNotRecurseFields = doNotRecurseFields;
        }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        /// <summary>
        /// list of fields to ignore when recursing
        /// </summary>
        public string[] DoNotRecurseFields { get; private set; }
        #endregion

        #region INodeValueManager
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object nodeValue, GraphPath nodePath)
        {
            List<Tuple<object, GraphPath>> rv = new List<Tuple<object, GraphPath>>();

            //if the node is IEnumerable, recurse here
            if (nodeValue is IEnumerable && (nodeValue is string) == false)
            {
                IEnumerable objEnumerable = nodeValue as IEnumerable;

                EnumeratedSegmentType segType = EnumeratedSegmentType.None;
                if (nodeValue is IDictionary)
                {
                    segType = EnumeratedSegmentType.IDictionary;
                }
                else if (nodeValue is Stack)
                {
                    segType = EnumeratedSegmentType.Stack;
                }
                else if (nodeValue is Queue)
                {
                    segType = EnumeratedSegmentType.Queue;
                }
                else if (nodeValue is IList)
                {
                    segType = EnumeratedSegmentType.IList;
                }
                int index = 0;
                foreach (var each in objEnumerable)
                {
                    //build the path
                    var path = GraphPath.New(nodePath);
                    path.AddSegment(EnumeratedItemSegment.New(index, segType));

                    rv.Add(new Tuple<object, GraphPath>(each, path));
                    index++;
                }
            }
            else
            {
                //recurse the fields           
                var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(nodeValue.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (FieldInfo field in fields)
                {
                    //get field value
                    var obj = field.GetValue(nodeValue);

                    var path = GraphPath.New(nodePath);
                    path.AddSegment(GraphSegment.New(field.DeclaringType, field.Name));

                    //build the node and recurse
                    rv.Add(new Tuple<object,GraphPath>(obj, path));

                }
            }
            return rv;
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
