//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters;
//using System.Text;
//using System.Threading.Tasks;
//using Decoratid.Idioms.Storing;

//namespace Decoratid.Serialization
//{
//    public class NetDataContractSerializerUtil : ISerializationUtil
//    {
//        #region Declarations
//        private readonly Encoding _Encoding = new UTF8Encoding();
//        private readonly IFormatter _Serializer;
//        #endregion

//        #region Ctor
//        public NetDataContractSerializerUtil()
//        {
//            var serializer = new NetDataContractSerializer();
//            serializer.AssemblyFormat = FormatterAssemblyStyle.Simple;
//            serializer.SurrogateSelector = new UnattributedTypeSurrogateSelector();
//            _Serializer = serializer;
//        }
//        #endregion

//        #region IHasId
//        public string Id { get { return this.GetType().Name; } }
//        object IHasId.Id { get { return this.Id; } }
//        #endregion

//        #region Methods
//        public string Serialize(object obj)
//        {
//            using (var memoryStream = new MemoryStream())
//            {
//                _Serializer.Serialize(memoryStream, obj);
//                return _Encoding.GetString(memoryStream.ToArray());

//                //formatter.Serialize(stream, action);
//                //stream.Position = 0;
//                //return stream.GetBuffer();
//            }
//        }

//        public object Deserialize(Type type, string s)
//        {
//            using (var memoryStream = new MemoryStream(_Encoding.GetBytes(s)))
//            {
//                return _Serializer.Deserialize(memoryStream);
//            }
//        }
//        #endregion
//    }
//}
