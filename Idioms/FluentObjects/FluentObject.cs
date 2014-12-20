using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.FluentObjects
{
    public class FluentObject<TContext> 
    {
        #region Ctor
        public FluentObject()
        {

        }
        #endregion

        #region Properties
        public TContext Context {get;set;}
        private List<IBehaviour> Behaviours { get; set; }
        private Func<TContext> ContextFactory { get; set; }
        #endregion

        #region Fluent Setup Methods
        public Behaviour<TContext,TArg,TResult> WithBehaviour<TArg,TResult>(string name)
        {
            Behaviour<TContext, TArg, TResult> returnValue = null;
            returnValue = this.Behaviours.Single(x => x.Name == name) as Behaviour<TContext, TArg, TResult>;

            if (returnValue == null)
            {
                //build a new one
                returnValue = new Behaviour<TContext, TArg, TResult>(name);
            }

            return returnValue;
        }
        /// <summary>
        /// Sets the context factory, so creation
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public FluentObject<TContext> InitContextFactory(Func<TContext> factory)
        {
            this.ContextFactory = factory;
            return this;
        }
        #endregion
    }
}
