using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Conditions;

namespace Sandbox.FluentObjects
{
    public class FluentObjectTest
    {
        public class Arg
        {
            public string S;
        }
        public class Response
        {
            public string S;
        }

        public void Test()
        {
            //builds a fluento that has an echo behaviour
            FluentObject<Void> o = new FluentObject<Void>();
            o.WithBehaviour<string, string>("echo")
            .Does((context, arg) =>
            {
                return arg;
            })
            .TriggeredWhen((context) =>
            {
                //trigger an echo when it's tuesday
                return new FuncCondition(
                    () => { return DateTime.Now.DayOfWeek == DayOfWeek.Tuesday; });

            },
                (context) =>
                {
                    return "hey! it's tuesday";
                });



        }
    }
}
