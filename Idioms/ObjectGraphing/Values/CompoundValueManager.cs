using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations.StoreOf;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraph.Path;
using System.Reflection;
using Decoratid.TypeLocation;
using System.Collections;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using Decoratid.Reflection;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// is always the LAST manager in the Chain of Responsibility
    /// </summary>
    public sealed class CompoundValueManager : INodeValueManager
    {
        public const string ID = "Compound";

        #region Ctor
        public CompoundValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            return true;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            //we return the full type of the compound type
            return TextDecorator.LengthEncode(obj.GetType().AssemblyQualifiedName);

        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var realData = TextDecorator.LengthDecode(nodeText);

            Type cType = TypeFinder.FindAssemblyQualifiedType(realData);
            Condition.Requires(cType).IsNotNull();
            var obj = ReflectionUtil.CreateUninitializedObject(cType);

            return obj;
        }

        #endregion
    }
}
