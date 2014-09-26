using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Utils;

namespace Decoratid.Core.Logical
{
    /// <summary>
    /// does some stuff
    /// </summary>
    [Serializable]
    public sealed class Logic : LogicBase
    {
        #region Ctor
        public Logic(Action action)
            : base()
        {
            Condition.Requires(action).IsNotNull();
            this.Action = action;
        }
        #endregion

        #region Static Methods
        public static Logic New(Action action)
        {
            Condition.Requires(action).IsNotNull();
            return new Logic(action);
        }
        #endregion

        #region ISerializable
        protected Logic(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Action = (Action)info.GetValue("_Action", typeof(Action));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Action", this.Action);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        private Action Action { get; set; }
        #endregion

        #region ILogic
        public override void Perform()
        {
            Action();
        }
        #endregion

        #region ICloneableLogic
        public override ILogic Clone()
        {
            return new Logic(this.Action);
        }
        #endregion

        #region Clone and Run
        public void CloneAndPerform()
        {
            Logic clone = (Logic)this.Clone();
            clone.Perform();
        }
        #endregion
    }

    public static class LogicExtensions
    {
        public static Action ToAction(this Logic logic)
        {
            if (logic == null) { return null; }

            return () => { logic.CloneAndPerform(); };
        }

        public static Logic MakeLogic(this Action action)
        {
            Condition.Requires(action).IsNotNull();
            return new Logic(action);
        }
    }

}
