using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Extensions;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations;
using ServiceStack.Text;
using Decoratid.Serialization;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Core.Storing
{
    public static class StoreSerializer
    {
        #region Methods
        /// <summary>
        /// serializes using Graph then Encodes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="managerSet"></param>
        /// <param name="encodingStrategy"></param>
        /// <returns></returns>
        public static string SerializeItem(object obj, ValueManagerChainOfResponsibility managerSet = null, Func<string, string> encodingStrategy = null)
        {
            var graph = Graph.Build(obj, managerSet);
            var text = graph.Dehydrate(); //serialize using graph
            var encodedData = encodingStrategy == null ? text : encodingStrategy(text);//encode
            //var valueEncodedData = ValueEncoder.LengthEncode(encodedData);//value encode
            return encodedData;
        }
        /// <summary>
        /// deserialization complement to SerializeItem
        /// </summary>
        /// <param name="text"></param>
        /// <param name="managerSet"></param>
        /// <param name="decodingStrategy"></param>
        /// <returns></returns>
        public static object DeserializeItem(string text, ValueManagerChainOfResponsibility managerSet = null, Func<string, string> decodingStrategy = null)
        {
            //do the reverse of serialize
            //var valueDecodedData = ValueEncoder.LengthDecode(text); //value decode
            var decodedData = decodingStrategy == null ? text : decodingStrategy(text); //decode
            var graph = Graph.Parse(decodedData, managerSet); //parse graph
            var item = graph.RootNode.NodeValue;
            return item;
        }  
     
        public static string SerializeStore(IGetAllableStore store, ValueManagerChainOfResponsibility managerSet = null, Func<string, string> encodingStrategy = null)
        {
            if (store == null)
                return null;

            var all = store.GetAll();
            List<string> lines = new List<string>();
            all.WithEach(each =>
            {
                var line = SerializeItem(each, managerSet, null);
                lines.Add(line);
            });

            var raw= TextDecorator.LengthEncodeList(lines.ToArray());
            var encodedData = encodingStrategy == null ? raw : encodingStrategy(raw);
            return encodedData;
        }
        public static IStore DeserializeStore(string data, ValueManagerChainOfResponsibility managerSet = null, Func<string,string> decodingStrategy =null)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var decodedData = decodingStrategy == null ? data : decodingStrategy(data);
            var list = TextDecorator.LengthDecodeList(decodedData);
            var store = NaturalInMemoryStore.New();
            list.WithEach(each =>
            {
                var item = DeserializeItem(each, managerSet, null);
                IHasId obj = item as IHasId;
                store.SaveItem(obj);
            });
            return store;
        }
        #endregion

    }
}
