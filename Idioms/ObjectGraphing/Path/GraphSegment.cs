using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using System;
using System.Collections.Generic;
using System.Linq;
using Decoratid.Extensions;

namespace Decoratid.Idioms.ObjectGraphing.Path
{
    public sealed class GraphSegment : IGraphSegment
    {
        #region Ctor
        private GraphSegment(string path)
        {
            Condition.Requires(path).IsNotNullOrEmpty();
            this.Path = path;
        }
        #endregion

        #region IGraphPath
        public string Path{ get; private set; }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator GraphSegment(string text)
        {
            GraphSegment rv = new GraphSegment(text);
            return rv;
        }
        public static implicit operator string(GraphSegment obj)
        {
            if (obj == null)
                return null;

            return obj.Path;
        }
        #endregion

        #region Fluent Static
        public static GraphSegment New(string segmentName)
        {
            return new GraphSegment(segmentName);
        }
        #endregion
    }
}
