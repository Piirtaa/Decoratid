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
    /// An encoded list will look like this {RS}item 0 length{GS}item 1 length{GS}item 2 length..{US}Data{RS}. 
    /// </summary>
    public interface ILengthFormattedList : IStringableList
    {
        bool IsListLengthFormatted(string text);
    }

    /// <summary>
    /// encodes a list of strings with item lengths prefixed as delimited list.
    /// An encoded list will look like this {RS}item 0 length,item 1 length,item 2 length..{US}Data{RS}. 
    /// </summary>
    /// <remarks>
    /// This class is the list version of the LengthPrefixDecoration, in that it provides header data
    /// of the length of the payload data, but for each item, in a common, easily-parsed header region.
    /// This helps avoid data corruption/injection/encoding issues, and gives us data/process validation.
    /// 
    /// </remarks>
    /// 
    [Serializable]
    public class LengthListDecoration : StringableListDecorationBase, ILengthFormattedList
    {
        public static string PREFIX = Delim.RS.ToString();
        public static string LENGTH_DELIM = ",";
        public static string ITEM_DELIM = Delim.US.ToString();
        public static string SUFFIX = Delim.RS.ToString();

        #region Ctor
        public LengthListDecoration(IStringableList decorated)
            : base(decorated)
        {

        }
        #endregion

        #region ISerializable
        protected LengthListDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public bool IsListLengthFormatted(string text)
        {
            if (!text.StartsWith(PREFIX))
                return false;

            if (!text.EndsWith(SUFFIX))
                return false;

            var lengths= text.GetFrom(PREFIX).GetTo(ITEM_DELIM);
            var lengthArr = lengths.Split(new string[] {LENGTH_DELIM},StringSplitOptions.None );

            if (lengthArr != null && lengthArr.Length > 0)
            {
                //get the payload data
                var data = text.GetFrom(ITEM_DELIM).GetTo(SUFFIX);
                if (string.IsNullOrEmpty(data))
                    return false;

                //validate the length of the data is the same as the sum of the lengths
                int dataLength = lengthArr.Sum(num => num.ConvertToInt());
                if (!data.Length.Equals(dataLength))
                    return false;
            }

            return true;
        }

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
                else
                {
                    lengths.Add(x.Length.ToString());
                }
            });

            StringBuilder rv = new StringBuilder();
            rv.Append(PREFIX);
            rv.Append(string.Join(LENGTH_DELIM, lengths));
            rv.Append(ITEM_DELIM);
            coreVal.WithEach(x =>
            {
                rv.Append(x);
            });
            rv.Append(SUFFIX);

            return rv.ToString();
        }
        public override void Parse(string text)
        {
            if (!IsListLengthFormatted(text))
                throw new InvalidOperationException("bad format");

            //clear first
            this.Decorated.Clear();

            var lengths = text.GetFrom(PREFIX).GetTo(ITEM_DELIM);
            var lengthArr = lengths.Split(new string[] { LENGTH_DELIM }, StringSplitOptions.None);

            if (lengthArr != null && lengthArr.Length > 0)
            {
                //get the payload data
                var data = text.GetFrom(ITEM_DELIM).GetTo(SUFFIX);

                foreach (var each in lengthArr)
                {
                    int length = each.ConvertToInt();
                    var item = data.Substring(0, length);
                    this.Decorated.Add(item);
                    
                    data = data.Substring(length);
                }
            }

        }
        public override IDecorationOf<IStringableList> ApplyThisDecorationTo(IStringableList thing)
        {
            return new LengthListDecoration(thing);
        }
        #endregion
    }

    public static class LengthPrefixListDecorationExtensions
    {
        public static LengthListDecoration DecorateListWithLength(this IStringableList thing)
        {
            Condition.Requires(thing).IsNotNull();
            return new LengthListDecoration(thing);
        }
        public static bool IsListLengthFormatted(this string text)
        {
            var stringable = NaturalStringableList.New().DecorateListWithLength();
            return stringable.IsListLengthFormatted(text);
        }
    }
}
