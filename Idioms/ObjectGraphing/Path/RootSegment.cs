using CuttingEdge.Conditions;

namespace Decoratid.Idioms.ObjectGraphing.Path
{



    /// <summary>
    /// Describes the root node.
    /// </summary>
    /// <remarks>
    /// dehydrates to Root
    /// </remarks>
    public sealed class RootSegment : IGraphSegment
    {
        public const string PREFIX = "Root";

        #region Ctor
        private RootSegment()
        {
        }
        #endregion

        #region IGraphPath
        public string Path
        {
            get
            {
                return PREFIX;
            }
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator RootSegment(string text)
        {
            Condition.Requires(text).IsNotNullOrEmpty();
            Condition.Requires(text).StartsWith(PREFIX);

            return new RootSegment();
        }
        public static implicit operator string(RootSegment obj)
        {
            if (obj == null)
                return null;

            return obj.Path;
        }
        #endregion

        #region Fluent Static Builders
        public static RootSegment New()
        {
            return new RootSegment();
        }
        #endregion
    }
}
