using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;
using Decoratid.Thingness;

namespace Decoratid.Configuration
{
    /// <summary>
    /// defines a typical key/value of string config entry
    /// </summary>
    public class ConfigEntry : IHasId<string>
    {
        public ConfigEntry() { }

        public string Value { get; set; }

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion
    }
}
