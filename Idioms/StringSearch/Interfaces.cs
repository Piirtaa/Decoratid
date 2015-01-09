using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// describes the word being matched, and where in the text being searched
    /// </summary>
    public interface ITextMatch
    {
        int PositionInText { get; }
        string Word { get; }
    }
    /// <summary>
    /// Given a set of words, defines something that searches for these words
    /// </summary>
    public interface ITextMatcher
    {
        string[] Words { get; set; }

        // 

        /// <summary>
        /// finds the next instances.  returns multiple because of the chance that 2 or more words share the same prefix and are found coincident
        /// with each other.
        /// </summary>
        /// <param name="startingIndex"></param>
        /// <param name="text"></param>
        /// <param name="grasp">how many characters have been searched.  this can be calculated, sure, but it's annoying
        /// and y not mandate it in the signature</param>
        /// <returns></returns>
        /// <remarks>
        /// Signature is designed so that the function completes when there are no more records to process at the given  
        /// </remarks>
        List<ITextMatch> FindNext(int startingIndex, string text, out int grasp);
        List<ITextMatch> FindAll(string text);
    }

    public interface IChainedTextMatcher : ITextMatcher
    {
        Dictionary<string, ITextMatcher> Word2ChainedMatcherMap { get; }
    }

}
