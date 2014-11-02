using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.OperationProtocoling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;

namespace Decoratid.Idioms.Messaging.OperationProtocoling
{
    public static class Extensions
    {
        public static OperationResponse GetOperationResult(this IOperationProtocolClient client, string operationName)
        {
            return client.ResponseStore.Get<OperationResponse>(operationName);
        }
        public static OperationError GetOperationError(this IOperationProtocolClient client, string operationName)
        {
            return client.ResponseStore.Get<OperationError>(operationName);
        }
        public static void AppendOperation<TArg>(this IOperationProtocolClient client, string operationName, TArg arg)
        {
            client.RequestStore.SaveItem(OperationRequest.New(operationName, arg));
        }
    }
}
