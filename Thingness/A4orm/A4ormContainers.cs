using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Reflection;
using Sandbox.Extensions;

namespace Sandbox.Thingness.A4orm
{

    public class A4ormObject
    {
        #region Ctor
        public A4ormObject()
        {
            this.Properties = new List<A4ormProperty>();
        }
        #endregion

        #region Properties
        public string Id { get; private set; }
        public List<A4ormProperty> Properties { get; private set; }
        #endregion

        #region Static Methods
        public void Build(object obj)
        {
            A4ormObject returnValue = new A4ormObject();
            if (obj is IHasId)
            {
                IHasId iHasId = obj as IHasId;
                returnValue.Id = iHasId.Id;
            }
            else
            {
                //autogen id
                returnValue.Id = Guid.NewGuid().ToString();
            }

            //walk the object
            var meta = TypeMetaCache.Instance.GetTypeMeta(obj.GetType());

            meta.Properties.WithEach(propMeta =>
            {
                A4ormProperty prop = new A4ormProperty();
                prop.Type = propMeta.Type;

            });
            var inspector = ObjectInspectorCache.Instance.GetInspector(obj.GetType());
            


            return returnValue;
        }
        private static void BuildProperty(A4ormObject context, object propertyValue, string currentPath )
        {
            //walk the object


        }
        #endregion
    }

    [Serializable]
    public class A4ormProperty
    {
        #region Ctor
        public A4ormProperty()
        {
            this.Values = new List<A4ormPropertyValue>();
        }
        #endregion

        #region Properties
        public string Path { get; set; }
        public Type Type { get; set; }
        public List<A4ormPropertyValue> Values { get; set; }
        #endregion

        #region Calculated Properties
        public bool IsEnumerable
        {
            get
            {
                return typeof(IEnumerable).IsAssignableFrom(this.Type);
            }
        }
        #endregion
    }

    [Serializable]
    public class A4ormPropertyValue
    {
        #region Ctor
        public A4ormPropertyValue()
        {

        }
        #endregion

        #region Properties
        public string Tag { get; set; }
        public object Value { get; set; }
        public DateTime DateCreated { get; set; }
        #endregion
    }

}
