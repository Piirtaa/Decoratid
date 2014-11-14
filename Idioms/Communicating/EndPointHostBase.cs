using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Serviceable;

namespace Decoratid.Idioms.Communicating
{
    /// <summary>
    /// base class for an end point host
    /// </summary>
    public abstract class EndPointHostBase : ServiceBase, IEndPointHost
    {
        #region Ctor
        /// <summary>
        /// Initializes
        /// </summary>
        /// <param name="ep"></param>
        /// <param name="strategy"></param>
        public EndPointHostBase(EndPoint ep, LogicOfTo<string,string> logic = null)
            : base()
        {
            Condition.Requires(ep).IsNotNull();

            //validate the ep is within the current ip list
            var ips = NetUtil.GetLocalIPAddresses();
            Condition.Requires(ips).Contains(ep.IPAddress);

            this.EndPoint = ep;
            this.Logic = logic;
        }
        #endregion

        #region IEndPointHost
        public EndPoint EndPoint { get; set; }
        public LogicOfTo<string, string> Logic { get; set; }
        #endregion


    }


}