using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Extensions
{
    public static class ActionExtensions
    {
        public static void EnqueueToThreadPool(this Action action)
        {
            System.Threading.WaitCallback callback = state => action();
            System.Threading.ThreadPool.QueueUserWorkItem(callback);
        }
    }
}
