using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    public class StringableTest : TestOf<IStringable>
    {
        public StringableTest()
            : base(LogicOf<IStringable>.New((x) =>
            {
                var oldVal = x.GetValue();
                var lengthstringable = x.DecorateWithLengthPrefix();
                var newVal  = lengthstringable.GetValue();

                Condition.Requires(oldVal).IsNotEqualTo(newVal);
                Condition.Requires(newVal).Contains(oldVal);

                lengthstringable.Parse(newVal);
                var decVal = lengthstringable.Decorated.GetValue();
                Condition.Requires(decVal).IsEqualTo(oldVal);
            })) 
        { 
        }
    }
    public class StringableListTest : TestOf<IStringableList>
    {
        public StringableListTest()
            : base(LogicOf<IStringableList>.New((x) =>
            {
                var oldVal = x.GetValue();
                var lengthstringable = x.DecorateWithLengthPrefixList();
                var newVal = lengthstringable.GetValue();

                Condition.Requires(oldVal).IsNotEqualTo(newVal);
                Condition.Requires(newVal).Contains(oldVal);

                lengthstringable.Parse(newVal);
                var decVal = lengthstringable.Decorated.GetValue();
                Condition.Requires(decVal).IsEqualTo(oldVal);
            }))
        {
        }
    }
}
