using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraphing;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Idioms.Stringing;
using System.Collections.Generic;

namespace Decoratid.Storidioms
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
        public static string SerializeItem(object obj, ValueManagerChainOfResponsibility managerSet = null)
        {
            var graph = Graph.Build(obj, managerSet);
            var text = graph.GetValue(); //serialize using graph
            //var valueEncodedData = ValueEncoder.LengthEncode(encodedData);//value encode
            return text;
        }
        /// <summary>
        /// deserialization complement to SerializeItem
        /// </summary>
        /// <param name="text"></param>
        /// <param name="managerSet"></param>
        /// <param name="decodingStrategy"></param>
        /// <returns></returns>
        public static object DeserializeItem(string text, ValueManagerChainOfResponsibility managerSet = null)
        {
            //do the reverse of serialize
            //var valueDecodedData = ValueEncoder.LengthDecode(text); //value decode
            var graph = Graph.Parse(text, managerSet); //parse graph
            var item = graph.RootNode.NodeValue;
            return item;
        }  
     
        public static string SerializeStore(IGetAllableStore store, ValueManagerChainOfResponsibility managerSet = null)
        {
            if (store == null)
                return null;

            var all = store.GetAll();
            List<string> lines = new List<string>();
            all.WithEach(each =>
            {
                var line = SerializeItem(each, managerSet);
                lines.Add(line);
            });

            var raw= LengthEncoder.LengthEncodeList(lines.ToArray());
            return raw;
        }
        public static IStore DeserializeStore(string data, ValueManagerChainOfResponsibility managerSet = null)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var list = LengthEncoder.LengthDecodeList(data);
            var store = NaturalInMemoryStore.New();
            list.WithEach(each =>
            {
                var item = DeserializeItem(each, managerSet);
                IHasId obj = item as IHasId;
                store.SaveItem(obj);
            });
            return store;
        }
        #endregion

    }
}
