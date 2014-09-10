using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Sandbox.Reflection
{
    /// <summary>
    /// captures the type information in an object graph
    /// </summary>
    public class TypeGraph
    {
        public List<TypeGraphNode> Nodes { get; set; }

        public bool RegisterNode(string path, Type type)
        {
            if(this.Nodes.Exists(x=>x.Type == type && x.Path == path))
            {
                return false;
            }
            this.Nodes.Add(new TypeGraphNode(type, path)); 
        }
    }

    public class TypeGraphNode
    {
        public TypeGraphNode(Type parentType, Type type, string path)
        {
            Condition.Requires(parentType).IsNotNull();
            Condition.Requires(type).IsNotNull();
            Condition.Requires(path).IsNotNullOrEmpty();

            this.ParentType = parentType;
            this.Type = type;
            this.Path = path;
        }
        public Type ParentType { get; private set; }
        public Type Type { get; private set; }
        public string Path { get; private set; }
    }
}
