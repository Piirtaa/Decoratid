using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing;
using Decoratid.TypeLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// converts the node object/value into string.  
    /// </summary>
    public interface INodeValueManager : IHasId<string>
    {
        bool CanHandle(object obj, IGraph uow);
        string DehydrateValue(object obj, IGraph uow);
        object HydrateValue(string nodeText, IGraph uow);
    }

    /// <summary>
    /// Brokers ValueManagers using a ChainOfResponsibility pattern
    /// </summary>
    public class ValueManagerChainOfResponsibility : IReconstable
    {

        #region Ctor
        /// <summary>
        /// if no valuemanagers are provided, uses the default set
        /// </summary>
        /// <param name="valueManagers"></param>
        private ValueManagerChainOfResponsibility(List<INodeValueManager> valueManagers = null)
        {
            this.ValueManagers = valueManagers == null ? GetDefaultValueManagerList() : valueManagers;
        }
        #endregion

        #region Properties
        private List<INodeValueManager> ValueManagers { get; set; }
        #endregion

        #region Manager Lookup
        /// <summary>
        /// finds the value manager that can handle the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public INodeValueManager FindHandlingValueManager(object obj, IGraph graph)
        {
            INodeValueManager rv = null;

            foreach (var each in this.ValueManagers)
            {
                if (each.Id.Equals(UndeclaredValueManager.ID))
                    continue;

                if (each.CanHandle(obj, graph))
                {
                    rv = each;
                    break;
                }
            }

            return rv;
        }

        public INodeValueManager GetValueManagerById(string id)
        {
            INodeValueManager rv = null;

            foreach (var each in this.ValueManagers)
            {
                if (each.Id.Equals(id))
                {
                    rv = each;
                    break;
                }
            }

            return rv;
        }
        #endregion

        #region IReconstable
        /// <summary>
        /// hydrates to a delimited list of ids
        /// </summary>
        /// <returns></returns>
        public string Dehydrate()
        {
            var ids = this.ValueManagers.Select((x) => { return x.Id; });
            return TextDecorator.LengthEncodeList(ids.ToArray());
        }
        /// <summary>
        /// takes a delimited list of value manager ids, and using TypeContainer plugin loading looks for 
        /// each id
        /// </summary>
        /// <param name="text"></param>
        public void Hydrate(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            var list = TextDecorator.LengthDecodeList(text);

            List<INodeValueManager> plugins = null;
            //strategy to load the managers (via assembly interrogation/plugin loading)
            Action initPlugins = () =>
            {
                TypeContainer<INodeValueManager> types = TypeContainer<INodeValueManager>.New();
                plugins = new List<INodeValueManager>();
                foreach (var each in types.ContainedTypes)
                {
                    try
                    {
                        INodeValueManager mgr = Activator.CreateInstance(each) as INodeValueManager;
                        if (list.Contains(mgr.Id))
                            plugins.Add(mgr);
                    }
                    catch { }
                }
            };

            //hydrate the managers list in the specified order
            var newList = new List<INodeValueManager>();
            foreach (var each in list)
            {
                //get the mgr by id from the current managers (we want to use managers that we've explicitly added, first)
                //  if it can't be found, get it from the plugins
                var mgr = this.ValueManagers.Find(x => x.Id == each);
                if (mgr == null)
                {
                    //we can't find the manager so load the plugins
                    if (plugins == null)
                        initPlugins();

                    mgr = plugins.Find(x => x.Id == each);
                }

                Condition.Requires(mgr).IsNotNull();
                newList.Add(mgr);
            }
            this.ValueManagers = newList;
        }
        #endregion

        #region Static Defaults
        public static List<INodeValueManager> GetDefaultValueManagerList()
        {
            var list = new List<INodeValueManager>();
            /*
             * This list is kept in a particular order, as the first match is brokered when a node value is processed.
             * We keep the list in this order:
             * Nulls 1st
             * Duplicates 2nd - this does a NodeList scan per Node
             * Fixed implementations 3rd (eg. Delegates, Primitives)
             * Interface overrides 4th (eg. Hydrateable, ManagedHydrateable)  Note Hydrateable MUST come first 
             * Custom things 5th (eg. Decorations)
             * Catch-alls 6th (eg. value types (which should serialize).
             * Final 7th (eg. Compounds)
             * 
             * The general concept here is fixed values first, then pluggable values, then special cases (decorations), other cases
             * and compounds.
             */
            list.Add(new NullValueManager());//MUST BE 1ST!!
            list.Add(new DuplicateValueManager());
            list.Add(new DelegateValueManager());
            list.Add(new PrimitiveValueManager());
            list.Add(new HydrateableValueManager());
            list.Add(new ManagedHydrateableValueManager());
            list.Add(new SerializableValueManager());
            list.Add(new DecorationValueManager());
            list.Add(new ValueTypeValueManager());
            //list.Add(new HasHydrationMapValueManager());
            list.Add(new CompoundValueManager());//MUST BE LAST!!

            return list;
        }
        #endregion

        #region Static Methods
        public static ValueManagerChainOfResponsibility New(List<INodeValueManager> valueManagers = null)
        {
            return new ValueManagerChainOfResponsibility(valueManagers);
        }
        public static ValueManagerChainOfResponsibility Default()
        {
            return new ValueManagerChainOfResponsibility(GetDefaultValueManagerList());
        }
        #endregion
    }
}
