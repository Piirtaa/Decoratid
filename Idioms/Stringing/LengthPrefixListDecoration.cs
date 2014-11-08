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

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// marker interface indicating the length prefix list decoration is applied
    /// </summary>
    public interface ILengthPrefixStringableList : IStringableList
    {

    }

    /// <summary>
    /// encodes a list of strings with item lengths prefixed as delimited list.
    /// An encoded string will look like this {RS}Delimited Lengths{US}Data{RS}
    /// </summary>
    /// <remarks>
    /// This class is the list version of the LengthPrefixDecoration, in that it provides header data
    /// of the length of the payload data, but for each item, in a common, easily-parsed header region.
    /// This helps avoid data corruption/injection/encoding issues, and gives us data/process validation.
    /// 
    /// </remarks>
    /// 
    [Serializable]
    public class LengthPrefixListDecoration : StringableListDecorationBase, ILengthPrefixStringableList
    {
        public static string DELIM_MID = Delim.US.ToString();
        public static string DELIM_END = Delim.RS.ToString();
        public static string DELIM_LENGTHS = Delim.GS.ToString();

        #region Ctor
        public LengthPrefixListDecoration(IStringableList decorated)
            : base(decorated)
        {

        }
        #endregion

        #region ISerializable
        protected LengthPrefixListDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// converts the list to a length-prefixed string of the format:
        /// {RS}l0{GS}l1{GS},l2..{US}Data{RS}
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <returns></returns>
        public override string GetValue()
        {
            var coreVal = this.Decorated;
            //get the lengths of each item
            List<string> lengths = new List<string>();
            coreVal.WithEach(x =>
            {
                if (x == null)
                {
                    lengths.Add("0");
                }
                {
                    lengths.Add(x.Length.ToString());
                }
            });

            StringBuilder rv = new StringBuilder();
            rv.Append(DELIM_END);
            rv.Append(string.Join(DELIM_LENGTHS, lengths));
            rv.Append(DELIM_MID);
            coreVal.WithEach(x =>
            {
                rv.Append(x);
            });
            rv.Append(DELIM_END);

            return rv.ToString();
        }
        public override void Parse(string text)
        {
            //clear first
            this.Decorated.Clear();

            //validate
            Condition.Requires(text).StartsWith(DELIM_END).EndsWith(DELIM_END);
            var lengths = text.MustGetFrom(DELIM_END).MustGetTo(DELIM_MID);
            var split = new string[] { DELIM_LENGTHS };
            var lengthArr = lengths.Split(split, StringSplitOptions.None);

            if (lengthArr != null && lengthArr.Length > 0)
            {
                var data = text.MustGetFrom(DELIM_MID);

                //slice up the rest of the data
                lengthArr.WithEach(x =>
                {
                    var length = x.ConvertToInt();
                    
                    var itemData = data.Substring(0, length);
                    this.Decorated.Add(itemData);

                    data = data.Substring(length);
                });

                //validate the end delim
                Condition.Requires(data).IsEqualTo(DELIM_END);
            }
        }
        public override IDecorationOf<IStringableList> ApplyThisDecorationTo(IStringableList thing)
        {
            return new LengthPrefixListDecoration(thing);
        }
        #endregion
    }

    public static class LengthPrefixListDecorationExtensions
    {
        public static LengthPrefixListDecoration DecorateWithLengthPrefixList(this IStringableList thing)
        {
            Condition.Requires(thing).IsNotNull();
            return new LengthPrefixListDecoration(thing);
        }
    }
}
