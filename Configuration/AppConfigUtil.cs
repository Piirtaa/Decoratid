using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Configuration
{
    public static class AppConfigUtil
    {
        #region Normal Entries
        /// <summary>
        /// returns all config entries where the key is suffixed by the provided suffix.  Used in the "convention config" approach.
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public static List<Tuple<string, string>> FindConfigEntriesWithKeySuffix(string keySuffix)
        {
            return FindMatchingConfigEntries((key, val) =>
            {
                return key.EndsWith(keySuffix);
            });
        }
        /// <summary>
        /// returns all config entries where the key is prefixed by the provided prefix.  Used in the "convention config" approach.
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public static List<Tuple<string, string>> FindConfigEntriesWithKeyPrefix(string keyPrefix)
        {
            return FindMatchingConfigEntries((key, val) =>
            {
                return key.StartsWith(keyPrefix);
            });
        }
        /// <summary>
        /// gets all key value pairs from config that match the filter
        /// </summary>
        /// <param name="keyValueFilter"></param>
        /// <returns></returns>
        public static List<Tuple<string, string>> FindMatchingConfigEntries(Func<string, string, bool> keyValueFilter)
        {
            List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>();

            if (keyValueFilter == null) { return returnValue; }

            var keys = ConfigurationManager.AppSettings.AllKeys;

            keys.WithEach(x =>
            {
                var val = ConfigurationManager.AppSettings[x];
                if (keyValueFilter(x, val))
                {
                    returnValue.Add(new Tuple<string, string>(x, val));
                }
            });

            return returnValue;
        }
        #endregion

        #region connection strings
        /// <summary>
        /// returns all config entries where the key is suffixed by the provided suffix.  Used in the "convention config" approach.
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public static List<ConnectionStringSettings> FindConnectionStringEntriesWithKeySuffix(string keySuffix)
        {
            return FindConnectionStringEntries((key, val) =>
            {
                return key.EndsWith(keySuffix);
            });
        }
        /// <summary>
        /// returns all config entries where the key is prefixed by the provided prefix.  Used in the "convention config" approach.
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public static List<ConnectionStringSettings> FindConnectionStringEntriesWithKeyPrefix(string keyPrefix)
        {
            return FindConnectionStringEntries((key, val) =>
            {
                return key.StartsWith(keyPrefix);
            });
        }
        public static List<ConnectionStringSettings> FindConnectionStringEntries(Func<string, string, bool> nameValueFilter)
        {
            List<ConnectionStringSettings> returnValue = new List<ConnectionStringSettings>();

            if (nameValueFilter == null) { return returnValue; }

            foreach(ConnectionStringSettings  each in ConfigurationManager.ConnectionStrings)
            {
                if (nameValueFilter(each.Name,each.ConnectionString))
                {
                    returnValue.Add(each);
                }
            }

            return returnValue;
        }
        #endregion
    }
}
