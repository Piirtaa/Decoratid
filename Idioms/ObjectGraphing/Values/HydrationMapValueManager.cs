using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraph.Values.Decorations;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using Decoratid.Core.Storing;
using Decoratid.Reflection;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// has a hydration map. 
    /// </summary>
    public interface IHasHydrationMap 
    {
        IHydrationMap GetHydrationMap();
    }


    #region Member Map
    /// <summary>
    /// defines a mapping of a member to a value manager
    /// </summary>
    public interface IHydrationMemberMapping : IHasId<string>
    {
        string MemberName { get; }
        string ValueManagerId { get; set; }
    }

    /// <summary>
    /// Describes Get/Set strategies, and the ValueManager for a member
    /// </summary>
    public class MemberMapping<T> : IHydrationMemberMapping
    {
        #region Ctor
        public MemberMapping(string memberName, Func<T, object> getter, Action<T, object> setter, string valueManagerId)
        {
            Condition.Requires(memberName).IsNotNullOrEmpty();
            Condition.Requires(getter).IsNotNull();
            Condition.Requires(setter).IsNotNull();
            Condition.Requires(valueManagerId).IsNotNullOrEmpty();
            this.Id = memberName;
            this.Getter = getter;
            this.Setter = setter;
            this.ValueManagerId = valueManagerId;
        }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public string MemberName { get { return this.Id; } }
        public Func<T, object> Getter { get; set; }
        public Action<T, object> Setter { get; set; }
        public string ValueManagerId { get; set; }
        #endregion

        #region Methods
        public object Get(T obj)
        {
            Condition.Requires(this.Getter).IsNotNull();
            var val = this.Getter(obj);
            return val;
        }
        public void Set(T obj, object val)
        {
            Condition.Requires(this.Setter).IsNotNull();
            this.Setter(obj, val);
        }
        #endregion
    }
    #endregion
    /// <summary>
    /// defines the mapping of value managers to getters/setters for something, implying fulfillment of 
    /// ValueManagement thru delegation to the maps.  TODO: remove implication 
    /// </summary>
    public interface IHydrationMap : INodeValueManager
    {
        List<IHydrationMemberMapping> Maps { get; }

        /// <summary>
        /// hydration that works on a provided object, instead of returning one
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <param name="uow"></param>
        void HydrateValue(object obj, string text, IGraph uow);
    }

    /// <summary>
    /// A value manager that uses a mapping that defines the members to serialize and the ValueManager that handles each, respectively.
    /// </summary>
    /// <remarks>
    /// If type implements IHasHydrationMap, it is indicating it has a MappedValueManager for itself.
    /// </remarks>
    public class HydrationMapValueManager<T> : IHydrationMap
    {
        public const string ID = "HydrationMap_{0}";

        #region Ctor
        public HydrationMapValueManager()
        {
            this.Maps = new List<IHydrationMemberMapping>();
        }
        #endregion

        #region IHasId
        public string Id { get { return string.Format(ID, typeof(T).Name); } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public List<IHydrationMemberMapping> Maps { get; private set; }
        #endregion

        #region Registration Methods
        /// <summary>
        /// registers a mapping with the Undeclared value manager
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        public void RegisterDefault(string memberName, Func<T, object> getter, Action<T, object> setter)
        {
            this.Register(memberName, getter, setter, UndeclaredValueManager.ID);
        }
        /// <summary>
        /// registers a mapping with a specific value manager
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <param name="valueMgrId"></param>
        public void Register(string memberName, Func<T, object> getter, Action<T, object> setter, string valueManagerId)
        {
            Condition.Requires(getter).IsNotNull();
            Condition.Requires(setter).IsNotNull();
            Condition.Requires(valueManagerId).IsNotNullOrEmpty();

            MemberMapping<T> map = new MemberMapping<T>(memberName, getter, setter, valueManagerId);
            this.Maps.Add(map);
        }
        #endregion

        #region IHydrationMap
        public void HydrateValue(object obj, string text, IGraph uow)
        {
            IGraph actualUow = null;
            //if we don't have a uow we use the default one
            if (uow == null)
            {
                actualUow = Graph.NewDefault();
            }

            var arr = TextDecorator.LengthDecodeList(text);
            Condition.Requires(arr.Count).IsEqualTo(this.Maps.Count);

            //iterate thru the Mappings and lines in parallel. 
            for (int i = 0; i < arr.Count; i++)
            {
                var map = this.Maps[i];
                MemberMapping<T> eachMap = (MemberMapping<T>)map;

                var line = arr[i];
                //note we inject a null check decoration below        
                var mgr = actualUow.ChainOfResponsibility.GetValueManagerById(map.ValueManagerId).DecorateWithNullCheck();
                var val = mgr.HydrateValue(line, actualUow);
                eachMap.Set((T)obj, val);
            }
        }
        #endregion

        #region INodeValueManager
        /// <summary>
        /// can only handle the exact same type a T
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        public bool CanHandle(object obj, IGraph uow)
        {
            return obj is T;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            T tObj = (T)obj;
            IGraph actualUow = null;
            //if we don't have a uow we use the default one
            if (uow == null)
            {
                actualUow = Graph.NewDefault();
            }

            List<string> lines = new List<string>();
            foreach (var each in Maps)
            {
                MemberMapping<T> eachMap = (MemberMapping<T>)each;
                var val = eachMap.Getter(tObj);

                //note we inject a null check decoration below        
                var mgr = actualUow.ChainOfResponsibility.GetValueManagerById(each.ValueManagerId).DecorateWithNullCheck();
                var stringVal = mgr.DehydrateValue(val, actualUow);
                lines.Add(stringVal);
            }
            return TextDecorator.LengthEncodeList(lines.ToArray());
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var obj = ReflectionUtil.CreateUninitializedObject(typeof(T));

            HydrateValue(obj, nodeText, uow);

            return obj;
        }
   
        #endregion

    }
}
