/*
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using ExtensibleHttp.Exceptions;
using ExtensibleHttp.Interfaces;
using ExtensibleHttp.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp
{
    public abstract class BaseRequest : IRequest
    {
        public IRequestConfig Config { get; private set; }
        public string EndpointUri { get; set; }
        public HttpRequestMessage HttpRequest { get; }
        public Dictionary<string, IEnumerable<string>> QueryParams { get; set; } = new Dictionary<string, IEnumerable<string>>();
        public string CorrelationId { get; private set; }
        protected static Dictionary<string, IEnumerable<string>> EMPTYHEADERS = new Dictionary<string, IEnumerable<string>>();

        public ApiFormat ApiFormat { get; set; }


        public HttpMethod Method
        {
            get { return HttpRequest.Method; }
            set { HttpRequest.Method = value; }
        }

        public BaseRequest(IRequestConfig cfg)
        {
            Config = cfg;
            HttpRequest = new HttpRequestMessage();
            CorrelationId = cfg.NewCorrelationId();
            ApiFormat = cfg.ApiFormat;
        }

        public virtual void AddMultipartContent(byte[] content)
        {
            var multipartContent = new MultipartFormDataContent
            {
                new ByteArrayContent(content)
            };
            HttpRequest.Content = multipartContent;
        }

        public virtual void AddMultipartContent(System.IO.Stream contentStream)
        {
            var multipartContent = new MultipartFormDataContent
            {
                new StreamContent(contentStream)
            };
            HttpRequest.Content = multipartContent;
        }

        public virtual void AddPayload<T>(T payload)
        {
            var data = new SerializerFactory().GetSerializer(ApiFormat).Serialize(payload);
            Debug.WriteLine(data);
            AddPayload(data);
        }

        public virtual void AddPayload(string payload)
        {
            HttpRequest.Content = new StringContent(payload, Encoding.UTF8, Config.GetContentType(ApiFormat));
            HttpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                Config.GetContentType(Config.ApiFormat));
            //HttpRequest.Content.Headers.ContentType.CharSet = "";

        }

        public virtual string BuildQueryParams()
        {
            var list = new List<string>();
            foreach (var param in QueryParams)
            {
                if (param.Value != null && param.Value.Count() > 0)
                {
                    string value = string.Empty;

                    if (param.Value.Count() > 1)
                        value = string.Join(",", param.Value.Select(s => Uri.EscapeDataString(s)));
                    else
                        value = param.Value.First();


                    list.Add(Uri.EscapeDataString(param.Key) + "=" + Uri.EscapeDataString(value));

                    //foreach (var value in param.Value)
                    //{
                    //	list.Add(Uri.EscapeDataString(param.Key) + "=" + Uri.EscapeDataString(value));
                    //}
                }
            }
            if (list.Count > 0)
            {
                return string.Join("&", list);
            }
            return "";
        }

        public virtual Task AddHeaders(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task AddUserAgentHeader(CancellationToken cancellationToken)
        {
            //HttpRequest.Headers.Add(Headers.USER_AGENT, Config.UserAgent.Replace(" ", "_"));
            return Task.CompletedTask;
        }

        public virtual Task BuildUri(CancellationToken cancellationToken)
        {
            string queryString = BuildQueryParams();

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                if (!EndpointUri.Contains("?"))
                {
                    queryString = "?" + queryString;
                }
                else
                {
                    queryString = "&" + queryString;
                }
            }

            HttpRequest.RequestUri = new Uri(Config.BaseUrl + EndpointUri + queryString);
            return Task.CompletedTask;
        }

        public virtual Task FinalizeRequest(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task SignRequest(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task ValidateAccessToken(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        public virtual Task ValidateResponse(IResponse response, CancellationToken cancellationToken)
        {
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                // 503 Service Unavailable
                throw new GatewayException("Service is unavailable, gateway connection error");
            }

            if (response.StatusCode == (HttpStatusCode)429)
            {
                // 429 Too many requests
                throw new ThrottleException("HTTP request was throttled");
            }
            return Task.CompletedTask;
        }
    }
}
