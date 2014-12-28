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
    //        ServicePointManager.ServerCertificateValidationCallback +=
    //(sender, cert, chain, sslPolicyErrors) => true;

            var response = client.GetStringAsync(url);
            try
            {
                response.Wait();
            }
            catch { }
            return response.Result;
        }
        public static bool Delete(HttpClient client, string url)
        {
            var response = client.DeleteAsync(url);
            try
            {
                response.Wait();
            }
            catch { }
            if (response.IsCompleted)
                return true;

            return false;
        }
        public static string Post(HttpClient client, HttpContent content, string url)
        {
            var response = client.PostAsync(url, content);
            try
            {
                response.Wait();
            }
            catch { }
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
            try
            {
                response.Wait();
            }
            catch { }
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
