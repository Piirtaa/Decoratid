using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Store;

namespace Sandbox.TypeLocation
{

    /// <summary>
    /// config that is used to instantiate typecontainer
    /// </summary>
    [Serializable]
    public class TypeContainerConfig : IHasId<string>
    {
        #region Ctor
        public TypeContainerConfig(string id)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
        }
        #endregion

        #region IHasId
        public string Id
        {
            get;
            private set;
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public List<string> ExcludeFilter { get; set; }
        /// <summary>
        /// if this is non null/non empty it takes precedence over exclude filter
        /// </summary>
        public List<string> IncludeFilter { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// does the type match the plugin config criteria
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsMatch(Type type)
        {
            bool returnValue = false;

            //if we have an include list, it takes priority
            //for each string in the include filter, if the type name contains it, it passes
            if (this.IncludeFilter != null && this.IncludeFilter.Count > 0)
            {
                foreach (string each in this.IncludeFilter)
                {
                    if (type.Name.Contains(each))
                    {
                        returnValue = true;
                        break;
                    }
                }
            }
            else if (this.ExcludeFilter != null && this.ExcludeFilter.Count > 0)
            {
                //if we aren't excluded, we're good
                returnValue = true;

                foreach (string each in this.ExcludeFilter)
                {
                    if (type.Name.Contains(each))
                    {
                        returnValue = false;
                        break;
                    }
                }
            }
            else
            {
                //everything is a match
                return true;
            }
            return returnValue;
        }
        #endregion


    }
}
