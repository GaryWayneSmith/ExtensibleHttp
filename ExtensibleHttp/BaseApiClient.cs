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
using ExtensibleHttp.Logger;

namespace ExtensibleHttp
{
    public abstract class BaseApiClient : IApiClient, IEndpointClient
    {
        protected IHttpFactory httpFactory = new HttpFactory();

        protected IHandler httpHandler;
        public IEndpointHttpHandler GetHttpHandler() => httpHandler;

        protected IEndpointConfig config;
        public IEndpointConfig GetEndpointConfig() => config;

        public BaseApiClient(IApiClientConfig cfg)
        {
            config = cfg;
            httpHandler = httpFactory.GetHttpHandler(cfg);
        }

        public IRetryPolicy RetryPolicy
        {
            get { return httpHandler.RetryPolicy; }
            set { httpHandler.RetryPolicy = value; }
        }

        public ILoggerAdapter Logger
        {
            get { return LoggerContainer.Logger; }
            set { LoggerContainer.Logger = value; }
        }
    }
}
