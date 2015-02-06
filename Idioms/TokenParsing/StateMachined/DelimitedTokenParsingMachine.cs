using Decoratid.Idioms.StateMachining;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.StateMachined
{
    public class DelimitedTokenParsingMachine
    {
        #region Declarations

        #endregion

        #region Ctor
        public DelimitedTokenParsingMachine()
        {
            _stateMachine = new StringStateMachineGraph(ServiceStateEnum.Uninitialized);
            _stateMachine.AllowTransition(ServiceStateEnum.Uninitialized, ServiceStateEnum.Initialized, ServiceTriggersEnum.Initialize);
            _stateMachine.AllowTransition(ServiceStateEnum.Initialized, ServiceStateEnum.Started, ServiceTriggersEnum.Start);
            _stateMachine.AllowTransition(ServiceStateEnum.Started, ServiceStateEnum.Stopped, ServiceTriggersEnum.Stop);
            _stateMachine.AllowTransition(ServiceStateEnum.Stopped, ServiceStateEnum.Started, ServiceTriggersEnum.Start);
        }
        #endregion

        #region Methods
        public void ParseFrom(string prefix, string suffix, Func<string,IToken,IToken)
        {

        }
        #endregion
    }
}
