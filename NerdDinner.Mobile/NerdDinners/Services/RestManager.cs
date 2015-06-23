using System;
using Newtonsoft.Json;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace Dinners.Services
{
    public static class HttpMethods
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";
    }
    public static class RestManager
    {
        /// <summary>
        /// Calls the rest service.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static async Task<string> CallRestService(string url, string method, Dictionary<string, string> parameters, List<KeyValuePair<string, string>> headers = null, string contentType = "application/json")
        {
            try
            {
                // Create an HTTP web request using the URL:
                string formattedParams = string.Empty;

                if ((string.Equals(HttpMethods.GET.ToString(), method) || string.Equals(HttpMethods.DELETE.ToString(), method)) && parameters != null && parameters.Count() > 0)
                {
                    formattedParams = string.Join("&",
                        parameters.Select(x => x.Key + "=" + System.Net.WebUtility.UrlEncode(x.Value)));
                    url = string.Format("{0}?{1}", url, formattedParams);
                }
                else if((string.Equals(HttpMethods.POST.ToString(), method) || string.Equals(HttpMethods.PUT.ToString(), method)) && parameters != null && parameters.Count() > 0)
                {
                    if (parameters != null && parameters.Count() > 0)
                    {
                        if ("application/json".Equals(contentType))
                        {
                            formattedParams = JsonConvert.SerializeObject(parameters);
                        }
                        else
                        {
                            formattedParams = string.Join("&",
                                parameters.Select(x => x.Key + "=" + x.Value));
                        }
                    }
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));

                request.Accept = "application/json";
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> kvp in headers)
                    {
                        request.Headers[kvp.Key] = kvp.Value;
                    }
                }
                request.Method = method;

                if (string.Equals(HttpMethods.POST.ToString(), method) || string.Equals(HttpMethods.PUT.ToString(), method))
                {
                    if (parameters.Count > 0)
                    {
                        request.ContentType = contentType;
                        Stream requestStream =
                await
                    Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream,
                        request);

                        using (StreamWriter writer = new StreamWriter(requestStream))
                        {
                            writer.Write(formattedParams);
                            writer.Flush();
                        }
                    }
                }

                return await GetResponseString(request);

            }
            catch (WebException ex)
            {
                throw new Exception(string.Format("Unable to make a request to URL {0}. Error Message = {1}", url, ex.Message), ex);
            }

        }

        /// <summary>
        /// Gets the response string.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private static async Task<string> GetResponseString(HttpWebRequest request)
        {
            WebResponse response =
                await
                    Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse,
                        request);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

