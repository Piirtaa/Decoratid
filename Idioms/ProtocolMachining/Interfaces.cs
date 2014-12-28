using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.ProtocolMachining
{
    /// <summary>
    /// to process a given 
    /// </summary>
    public interface IProtocolContext
    {
        Dictionary<string,
        byte[] RequestBuffer;
        byte[] ResponseBuffer;
        int ReqCursorPos;
        bool IsCloseIndicated { get; }//defaults to false
    }

    /// <summary>
    /// the Id is the name of the state
    /// </summary>
    public interface IProtocolStateHandler : IHasId<string>
    {
        /// <summary>
        /// returns the id of the next handler
        /// </summary>
        Func<IProtocolContext, string> HandlingStrategy { get; }
    }
}
