using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;
using Decoratid.Idioms.Stringing;

namespace Decoratid.Idioms.Filing
{
    /// <summary>
    /// decorates with fileable, with the fileable implementation wrapping stringable GetValue() and Parse().
    /// basically allows a stringable to convert as fileable.
    /// </summary>
    [Serializable]
    public class FileableStringableDecoration : StringableDecorationBase, IFileable
    {
        #region Ctor
        public FileableStringableDecoration(IStringable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected FileableStringableDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IFileable
        public void Read()
        {
            //do nothing since this decoration doesn't have a backing file yet
        }
        public void Write()
        {
            //do nothing since this decoration doesn't have a backing file yet
        }
        #endregion

        #region Overrides
        public override string GetValue()
        {
            var val = this.Decorated.GetValue();
            return val;
        }
        public override void Parse(string text)
        {
            this.Decorated.Parse(text);
        }
        public override IDecorationOf<IStringable> ApplyThisDecorationTo(IStringable thing)
        {
            return new FileableStringableDecoration(thing);
        }
        #endregion
    }

    public static class FileableStringableDecorationExtensions
    {
        public static FileableStringableDecoration Fileable(this IStringable thing)
        {
            Condition.Requires(thing).IsNotNull();
            return new FileableStringableDecoration(thing);
        }
    }
}
