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
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Endpoint
{
    public class BaseResourceEndpoint
    {
        protected IEndpointConfig config;
        protected IPayloadFactory payloadFactory;
        protected IEndpointHttpHandler client;
        protected IEndpointClient apiClient;
        public object ApiException { get; private set; }


        public BaseResourceEndpoint(IEndpointClient apiClient)
        {
            this.apiClient = apiClient;
            client = apiClient.GetHttpHandler();
            config = apiClient.GetEndpointConfig();
        }

        public async Task<string> ProcessStringResponse(IResponse response, CancellationToken cancellationToken)
        {
            if (!response.IsSuccessful)
            {
                var rawErrors = await response.GetPayloadAsString(cancellationToken);
                //Debug.WriteLine(rawErrors);
                return rawErrors;
            }
            string content = await response.GetPayloadAsString(cancellationToken);
            //Debug.WriteLine(content);
            return content;
        }


        public async Task<TPayload> ProcessResponse<TPayload>(IResponse response, CancellationToken cancellationToken)
        {
            if (!response.IsSuccessful)
            {
                var rawErrors = await response.GetPayloadAsString(cancellationToken);
                Debug.WriteLine(rawErrors);
                Exception ex;
                try
                {
                    // More work needs to be done to put the Exception up to the Request level
                    // so we can process them there as vendor type specific.
                    //
                    // We can create a custom payload factory per resource, so maybe that's
                    // the final answer
                    ex = payloadFactory.CreateApiException(config.ApiFormat, rawErrors, response);
                }
                catch (Exception e)
                {
                    var innerEx = new Exception("Error response is " + rawErrors, e);
                    throw new InvalidValueException("Invalid error response received. Unable to parse with error!", innerEx);
                }

                throw ex;
            }
            string content = await response.GetPayloadAsString(cancellationToken);

            //Debug.WriteLine(content);

            var serializer = payloadFactory.GetSerializer(config.ApiFormat);
            return serializer.Deserialize<TPayload>(content);
        }

        public ISerializer GetSerializer(ApiFormat apiFormat)
        {
            return payloadFactory.GetSerializer(apiFormat);
        }
    }
}
