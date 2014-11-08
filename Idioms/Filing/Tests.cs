using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Filing
{
    public class FileableTest : TestOf<IStringable>
    {
        public FileableTest()
            : base(LogicOf<IStringable>.New((x) =>
            {

                //get the string value
                var val = x.GetValue();

                //back to a file
                var fileable = x.Fileable().Filing("test.test");
                fileable.Write();
                var readVal = fileable.GetValue();
                Condition.Requires(readVal).IsEqualTo(val);

                fileable.Parse("a");
                fileable.Write();
                readVal = fileable.GetValue();
                Condition.Requires(readVal).IsEqualTo("a");

                //back to a locked file
                var lockfileable = x.Fileable().LockingFiling("test.test");
                lockfileable.Read();
                readVal = lockfileable.GetValue();
                Condition.Requires(readVal).IsEqualTo("a");

                //test the locking
                fileable = x.Fileable().Filing("test.test");
                try
                {
                    fileable.Read();
                }
                catch { }
            })) 
        { 
        }
    }

}
