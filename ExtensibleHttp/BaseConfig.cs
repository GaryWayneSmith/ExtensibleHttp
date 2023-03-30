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
using ExtensibleHttp.Interfaces;
using System;
using System.Reflection;

namespace ExtensibleHttp
{
    public abstract class BaseConfig : IRequestConfig, IApiClientConfig
    {
        public string BaseUrl { get; set; } = "";
        public string UserAgent { get; set; }
        public ApiFormat PayloadFormat { get; set; } = ApiFormat.Json;

        // just another name for paylod format we also use it for http request
        // and in this case it helps to determine value of contet-type header, not payload
        public ApiFormat ApiFormat
        {
            get { return PayloadFormat; }
            set { PayloadFormat = value; }
        }
        public int RequestTimeoutMs { get; set; } = 100000; // in milliseconds

        public string NewCorrelationId()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetContentType(ApiFormat apiFormat)
        {
            switch (apiFormat)
            {
                case ApiFormat.Json:
                    return "application/json";
                default:
                case ApiFormat.Xml:
                    return "application/xml";
            }
        }

        public BaseConfig()
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            UserAgent = $".Net_{assembly.GetName().Name}_v{assembly.GetName().Version}";
        }

        public IRequestConfig GetRequestConfig() => this;


    }
}
