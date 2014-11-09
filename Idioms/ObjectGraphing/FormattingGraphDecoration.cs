using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using Decoratid.Extensions;

namespace Decoratid.Idioms.ObjectGraphing
{
    public interface IFormattingGraph : IGraph
    {
        string GetStoreText();
    }
        
    [Serializable]
    public class FormattingGraphDecoration : DecoratedGraphBase, IFormattingGraph
    {
        #region Ctor
        public FormattingGraphDecoration(IGraph decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Methods
        public string GetStoreText()
        {
            var nodes = this.NodeStore.GetAll();

            List<string> lines = new List<string>();

            nodes.WithEach(node =>
            {
                string nodeLine = string.Join("|", node.TraversalIndex.ToString(), node.ValueManagerId, node.Id, node.Context);
                lines.Add(nodeLine);
            });
            
            return string.Join(Environment.NewLine, lines.ToArray());
        }
        public override IDecorationOf<IGraph> ApplyThisDecorationTo(IGraph thing)
        {
            return new FormattingGraphDecoration(thing);
        }
        #endregion
    }

    public static class FormattingGraphDecorationExtensions
    {
        /// <summary>
        /// adds formatting of the graph, for readability
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static FormattingGraphDecoration Formats(this IGraph decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new FormattingGraphDecoration(decorated);
        }

    }
}
