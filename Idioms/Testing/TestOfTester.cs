using Decoratid.Idioms.TypeLocating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Identifying;
using Decoratid.Idioms.Filing;

namespace Decoratid.Idioms.Testing
{
    public static class TestOfTester
    {
        /// <summary>
        /// using type location, examines the appdomain for ITestOfs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<ITestOf<T>> LocateTests<T>()
        {
            Type genType = typeof(ITestOf<>);
            var tests = TheTypeLocator.Instance.Locator.Locate((t) =>
            {
                if (!t.HasGenericDefinition(genType))
                    return false;

                var type = t.GetGenericParameterType(genType);
                var ttype = typeof(T);
                if(ttype.Equals(type))
                    return true;
                if (type.IsSubclassOf(type))
                    return true;

                return false;
            });

            List<ITestOf<T>> rv = new List<ITestOf<T>>();

            tests.WithEach((testType) =>
            {

                ITestOf<T> test = (ITestOf<T>)Activator.CreateInstance(testType);
                rv.Add(test);
            });

            return rv;
        }
        /// <summary>
        /// Performs a single test
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public static TestOfResult Test<T>(T item, ITestOf<T> test)
        {
            try
            {
                test.Test(item);
                return new TestOfResult() { IsTestSuccess = true, TestType = test.GetType() };
            }
            catch (Exception ex)
            {
                return new TestOfResult() { TestError=ex, TestType = test.GetType() };
            }
        }

        public static List<TestOfResult> Test<T>(T item, List<ITestOf<T>> tests)
        {
            List<TestOfResult> rv = new List<TestOfResult>();
            tests.WithEach((test) =>
            {
                rv.Add(Test(item, test));

            });
            return rv;
        }
        public static List<TestOfResult> AutomaticTest<T>(T item)
        {
            List<TestOfResult> rv = new List<TestOfResult>();
            var tests = LocateTests<T>();
            tests.WithEach((test) =>
            {
                rv.Add(Test(item, test));
            });
            return rv;
        }
        public static bool CheckTestResults(List<TestOfResult> results)
        {
            if (results == null)
                return false;
            if (results.Count == 0)
                return false;

            foreach (var each in results)
            {
                if (each.TestError != null)
                {
                    return false;
                }
            }

            return true;
        }

        public static void LogTestResults(List<TestOfResult> results, string path)
        {
            if (results == null)
                return ;
            if (results.Count == 0)
                return ;

            //get something we can write to
            var list = NaturalStringableList.New().Fileable().Filing(path);
            IStringableList iList = list.FreeWalkFindDecoratorOf(typeof(IStringableList), false) as IStringableList;

            foreach (var each in results)
            {
                var s = each.Stringable();
                //cos we've changed our decorated type from stringablelist to stringable we have to grab the list decoration
                var eVal = s.GetValue();
                iList.Add(eVal);
            }
            list.Write();

        }
    }


}
