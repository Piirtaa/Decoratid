
    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Serialization
{
    public class GrapherSerializationUtil  : ISerializationUtil
    {
        public const string ID = "GrapherSerializationUtil";

        public GrapherSerializationUtil() { }

        #region IHasId
        public string Id { get { return this.GetType().Name; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        public string Serialize(object obj)
        {
            string returnValue = string.Empty;
            if (obj == null) { return returnValue; }

            try
            {
                var graph = Graph.Build(obj);
                returnValue = graph.Dehydrate();
            }
            catch
            {
                throw;
            }
            return returnValue;
        }
        public object Deserialize(Type type, string s)
        {
            object returnValue = null;
            if (string.IsNullOrEmpty(s)) { return returnValue; }

            try
            {
                var graph = Graph.Parse(s);
                returnValue = graph.RootNode.NodeValue;
            }
            catch
            {
                throw;
            }
            return returnValue;
        }


    }
}

