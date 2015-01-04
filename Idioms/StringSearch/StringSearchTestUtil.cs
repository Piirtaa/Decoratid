using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Gma.DataStructures.StringSearch;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// Randomly generated test case for string search testing 
    /// </summary>
    public class StringSearchTestCase
    {
        private StringSearchTestCase(int numberOfDictionaryItems, int percentMatch, int numSearchTextLines)
        {
            this.TestDictionary = GenerateTestDictionary(numberOfDictionaryItems);
            this.TestSearchText = string.Join(Environment.NewLine, GenerateSearchText(this.TestDictionary, percentMatch, numSearchTextLines));
            this.ExpectedPercentMatch = percentMatch;
        }
        public List<string> TestDictionary { get; private set; }
        public string TestSearchText { get; private set; }
        public int ExpectedPercentMatch { get; private set; }

        public static StringSearchTestCase New(int numberOfDictionaryItems, int percentMatch, int numSearchTextLines)
        {
            return new StringSearchTestCase(numberOfDictionaryItems, percentMatch, numSearchTextLines);
        }

        private static List<string> GenerateTestDictionary(int numberOfItems)
        {
            List<string> rv = new List<string>();

            StringGenerator generator = new StringGenerator();

            for (int i = 0; i < numberOfItems; i++)
            {
                var line = generator.GenerateAlpha(10, 50);
                rv.Add(line);
            }
            return rv;
        }
        /// <summary>
        /// Generates a list of test strings, the number of items being specified by numberOfLines.
        /// percentMatch is the number of words in the dictionary per line, as dictated by pseudorandom distribution
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="percentMatch"></param>
        /// <param name="numberOfLines"></param>
        /// <returns></returns>
        private static List<string> GenerateSearchText(List<string> dictionary, int percentMatch, int numberOfLines)
        {
            List<string> rv = new List<string>();

            //define the flag to write out a dictionary item
            Random rnd = new Random();
            int dictSize = dictionary.Count;

            StringGenerator generator = new StringGenerator();
            int matchLines = 0;
            Func<string> generateLine = () =>
            {
                //get random number from 0 to 100
                var rndVal = rnd.Next(0, 100);
                //run random filter
                if (rndVal > percentMatch)
                    return string.Empty;

                matchLines++;

                //get random keyword and return it
                rndVal = rnd.Next(0, dictSize - 1);
                var keyWord = dictionary[rndVal];
                var fakeLinePrefix = generator.GenerateAlpha(100, 1000);
                var fakeLineSuffix = generator.GenerateAlpha(100, 1000);
                return fakeLinePrefix + keyWord + fakeLineSuffix; //stick the value in the middle of the fake line
            };

            for (int i = 0; i < numberOfLines; i++)
            {
                rv.Add(generateLine());
            }
            Debug.WriteLine("{0} match lines in {1} total", matchLines, numberOfLines);
            return rv;

        }
    }

    public static class StringSearchTestUtil
    {
        public static void Test()
        {
            var testcase = StringSearchTestCase.New(50, 10, 1000000);
            Stopwatch stopWatch = new Stopwatch();

            //trie
            ITrie<string> gma_trie =  new PatriciaTrie<string>();
            stopWatch.Start();
            testcase.TestDictionary.WithEach(x =>
            {
                gma_trie.Add(x, x);
            });
            stopWatch.Stop();
            Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            var gma_matches = gma_trie.Retrieve(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, gma_matches.Count())
                );
            //eeeksoft 
            stopWatch.Reset();
            stopWatch.Start();
            var eeeksoft_searcher = new Decoratid.Idioms.StringSearch.EeekSoft.StringSearch(testcase.TestDictionary.ToArray());
            stopWatch.Stop();
            Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            
            stopWatch.Reset();
            stopWatch.Start();
            var eeeksoft_matches = eeeksoft_searcher.FindAll(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, eeeksoft_matches.Length)
                );
            
            //pduniho
            stopWatch.Reset();
            stopWatch.Start();
            var pduniho_graph = new Decoratid.Idioms.StringSearch.PDuniho.StateGraph<char, int>();
            for (int istr = 0; istr < testcase.TestDictionary.Count; istr++)
            {
                pduniho_graph.Add(testcase.TestDictionary[istr], istr);
            }
            stopWatch.Stop();
            Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            var pduniho_matches = pduniho_graph.RgobjFromCollectionParallel(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, pduniho_matches.Length)
                );

            //pnikiforovs
            stopWatch.Reset();
            stopWatch.Start();
            var pnikiforovs_trie = new Decoratid.Idioms.StringSearch.PNikiforovs.Trie();
            testcase.TestDictionary.WithEach(item =>
            {
                pnikiforovs_trie.Add(item);
            });
            pnikiforovs_trie.Build();
            stopWatch.Stop();
            Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            string[] pnikiforovs_matches = pnikiforovs_trie.Find(testcase.TestSearchText).ToArray();
            stopWatch.Stop();
            Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, pnikiforovs_matches.Length)
                );
        }

    }
}
