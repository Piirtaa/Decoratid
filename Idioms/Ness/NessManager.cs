using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core;
using Decoratid.Extensions;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Ness
{
    /// <summary>
    /// container of all the Nesses
    /// </summary>
    public class NessManager
    {
        #region Ctor
        public NessManager(IStoreOf<INess> store = null)
        {
            if (store == null)
            {
                this.Store = NaturalInMemoryStore.New().IsOf<INess>();
            }
            else
            {
                this.Store = store;
            }
        }
        #endregion

        #region Static Fluent
        public static NessManager New(IStoreOf<INess> store = null)
        {
            return new NessManager(store);
        }
        #endregion

        #region Properties
        public IStoreOf<INess> Store { get; private set; }
        #endregion

        #region AutoComplete Methods
        /// <summary>
        /// for an instance, get all its faces, and aggregate all the possible ness it has
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<INess> GetPossibleNess(object obj)
        {
            List<INess> rv = new List<INess>();

            var types = obj.GetFaceTypes();
            types.WithEach(x =>
            {
                rv.AddRange(this.GetPossibleDecorations(x));
            });

            return rv;
        }
        /// <summary>
        /// for a given type to be decorated, get all the registered, applicable nesses
        /// </summary>
        /// <param name="decoratedType"></param>
        /// <returns></returns>
        private List<INess> GetPossibleDecorations(Type decoratedType)
        {
            List<INess> rv = new List<INess>();

            rv = this.Store.SearchOf<INess>(LogicOfTo<INess, bool>.New(x =>
            {
                return x.DecoratedType.IsAssignableFrom(decoratedType);
            }));

            return rv;
        }

        #endregion

        #region Registration
        public void AddNess(INess ness)
        {
            this.Store.SaveItem(ness);
        }
        #endregion

        #region Ness Building
        public INess GetNess(object obj, string nessName)
        {
            var possibleness = this.GetPossibleNess(obj);
            var rv = possibleness.Find(x => x.Name.Equals(nessName));
            return rv;
        }
        public object DecorateWith(object obj, string nessName, object[] args)
        {
            var ness = this.GetNess(obj, nessName);
            Condition.Requires(ness).IsNotNull();
            var rv = ness.DecorateWith(obj, args);
            return rv;
        }
        #endregion

        #region Ness Operations
        public object PerformOperation(object obj, string nessName, string operationName, object[] args)
        {
            var ness = this.GetNess(obj, nessName);
            Condition.Requires(ness).IsNotNull();
            var rv = ness.PerformOperation(operationName, obj, args);
            return rv;
        }
        #endregion

        #region Ness Conditions
        public bool? IsConditionTrue(object obj, string nessName, string condition, object[] args)
        {
            var ness = this.GetNess(obj, nessName);
            Condition.Requires(ness).IsNotNull();
            var rv = ness.IsConditionTrue(condition, obj, args);
            return rv;
        }
        #endregion
    }
}
