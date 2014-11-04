using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Smellable
{
    /// <summary>
    /// marker interface to faciliate fast ISmellException classification
    /// </summary>
    public interface ISmellException
    {
    }

    /// <summary>
    /// base smell exception.  i know.  this shit cracks me up all the live long day
    /// </summary>
    [Serializable]
    public class SmellException : ApplicationException, ISmellException
    {
        #region Ctor
        public SmellException(string message)
            : base(message)
        {
        }
        public SmellException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected internal SmellException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Overrides
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // call base
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
