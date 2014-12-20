using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Communicating.HTTPing
{
    public static class HTTPClient
    {
        public static string Get(HttpClient client, string url)
        {
            var response = client.GetStringAsync(url);
            response.Wait();
            return response.Result;
        }
        public static bool Delete(HttpClient client, string url)
        {
            var response = client.DeleteAsync(url);
            response.Wait();
            if (response.IsCompleted)
                return true;

            return false;
        }
        public static string Post(HttpClient client, HttpContent content, string url)
        {
            var response = client.PostAsync(url, content);
            response.Wait();
            if (response.IsCompleted)
            {
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                var rv = responseContent.Result;
                return rv;
            }

            return string.Empty;
        }
        public static string Put(HttpClient client, HttpContent content, string url)
        {
            var response = client.PostAsync(url, content);
            response.Wait();
            if (response.IsCompleted)
            {
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                var rv = responseContent.Result;
                return rv;
            }

            return string.Empty;
        }
    }
}
