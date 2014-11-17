using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Identifying;
using System;
using Decoratid.Idioms.Filing;
using Decoratid.Idioms.Backgrounding;
using Decoratid.Core.Logical;
using Decoratid.Storidioms;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Idioms.Stringing;
using System.Diagnostics;

namespace Decoratid.Idioms.Logging
{

    public static class FileLoggerUtil
    {
        public static ILogger GetFileLogger(string path)
        {
            var logger = StoreLogger.New(NaturalInMemoryStore.New().Polls());
            logger.Store.GetPoll().SetBackgroundAction(LogicOf<IStore>.New((store) =>
            {
                var dat = StoreSerializer.SerializeStore(store, ValueManagerChainOfResponsibility.NewDefault());
                Debug.WriteLine(dat);
                dat.MakeStringable().Fileable().Filing(path).Write();
                
            }), 100);
            return logger;
        }
    }

}
