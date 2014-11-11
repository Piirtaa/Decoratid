using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Counting;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Idioms.Stringing;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Decoratid.Extensions;
using System.IO;
using Decoratid.Idioms.Depending;

namespace Decoratid.Idioms.ObjectGraphing
{
    public class GraphingUtil
    {
        public const string FieldNameDelim = " ";

        /// <summary>
        /// for a given type, returns all the fieldinfos as a list of tuples of fieldinfo and a string shortform
        /// indicating the declaring type nesting level in the inheritance tree.  ie.  FieldA0, would indicate
        /// FieldA on the most base type of the  topmost type.  FieldB2 would indicate FieldB on the 2nd type up from
        /// the base type. 0 is bottom type (most base class), and it goes up from there. I want to be able to discern
        /// graph similarity by simple string compare.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Tuple<string, FieldInfo>> GetNestingNotatedFieldInfos(Type type)
        {
            List<Tuple<string, FieldInfo>> rv = new List<Tuple<string, FieldInfo>>();

            var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //figure out the height of the type inheritance tree
            int inheritanceHeight = 0;
            Type cType = type;
            while (cType != typeof(object))
            {
                cType = cType.BaseType;
                inheritanceHeight++;
            }

            fields.WithEach(x =>
            {
                //figure out the inheritance nesting level
                int level = 0;
                Type currentType = type;
                while (!x.DeclaringType.Equals(currentType))
                {
                    currentType = currentType.BaseType;
                    level++;
                }
                int inheritanceLevel = inheritanceHeight - level;
                rv.Add(new Tuple<string, FieldInfo>(x.Name + FieldNameDelim + inheritanceLevel, x));
            });
            return rv;
        }
        /// <summary>
        /// for a given node returns all the child nodes
        /// </summary>
        /// <param name="nodeValue"></param>
        /// <param name="parentNodePath"></param>
        /// <returns></returns>
        public static List<Tuple<object, GraphPath>> GetChildTraversalNodes(object nodeValue, GraphPath parentNodePath,
            Func<object, GraphPath, bool> doNotTraverseFilter = null)
        {
            List<Tuple<object, GraphPath>> rv = new List<Tuple<object, GraphPath>>();

            //if the node is IEnumerable, recurse here
            if (nodeValue is IEnumerable && (nodeValue is string) == false)
            {
                IEnumerable objEnumerable = nodeValue as IEnumerable;

                int index = 0;
                foreach (var each in objEnumerable)
                {
                    //build the path
                    var path = GraphPath.New(parentNodePath);
                    path.AddSegment(EnumeratedItemSegment.New(index));

                    if (doNotTraverseFilter != null &&
                        doNotTraverseFilter(each, path))
                        continue;

                    rv.Add(new Tuple<object, GraphPath>(each, path));
                    index++;
                }
            }
            else
            {
                //recurse the fields   
                var fields = GraphingUtil.GetNestingNotatedFieldInfos(nodeValue.GetType());

                foreach (var field in fields)
                {
                    //get field value
                    var obj = field.Item2.GetValue(nodeValue);

                    var path = GraphPath.New(parentNodePath);
                    path.AddSegment(GraphSegment.New(field.Item1));

                    if (doNotTraverseFilter != null &&
                        doNotTraverseFilter(obj, path))
                        continue;

                    //build the node and recurse
                    rv.Add(new Tuple<object, GraphPath>(obj, path));

                }
            }
            return rv;
        }
        /// <summary>
        /// if the current node segment has a backing field naming convention (eg. k__BackingField format), prettify it
        /// by removing the k__BackingField stuff
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void RewriteBackingFieldNodePath(GraphPath path)
        {
            Condition.Requires(path).IsNotNull();

            if (path.CurrentSegment.Path.Contains(">k__BackingField"))
            {
                string fieldName = path.CurrentSegment.Path.Replace(">k__BackingField", "");
                fieldName = fieldName.Replace("<", "");

                path.ChangeCurrentSegmentPath(fieldName);
            }
        }
    }
}
