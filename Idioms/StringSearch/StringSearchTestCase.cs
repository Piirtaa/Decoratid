using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <summary>
        /// generates a bunch of items to be searched for within a larger text
        /// </summary>
        /// <param name="numberOfItems"></param>
        /// <returns></returns>
        public static List<string> GenerateTestDictionary(int numberOfItems)
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
        public static List<string> GenerateSearchText(List<string> dictionary, int percentMatch, int numberOfLines)
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

        public static List<string> GenerateDecorationySearchText(List<string> dictionary, int percentMatch, int numberOfLines)
        {
            List<string> rv = new List<string>();

            //define the flag to write out a dictionary item
            Random rnd = new Random();
            int dictSize = dictionary.Count;

            StringGenerator generator = new StringGenerator();
            Func<string> generateLine = () =>
            {
                string line = string.Empty;

                dictionary.ForEach(word =>
                {
                    var fakeLinePrefix = generator.GenerateAlpha(10, 50);
                    var fakeLineSuffix = generator.GenerateAlpha(10, 50);

                    line += fakeLinePrefix;

                    //get random number from 0 to 100
                    var rndVal = rnd.Next(0, 100);
                    //run random filter
                    if (rndVal < percentMatch)
                    {
                        line += word;
                    }
                    line += fakeLineSuffix;
                });

                return line;
            };

            for (int i = 0; i < numberOfLines; i++)
            {
                rv.Add(generateLine());
            }

            return rv;

        }
    }
}
