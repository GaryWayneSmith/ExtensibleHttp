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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp
{
    public class Response : IResponse
    {
        private readonly HttpResponseMessage originalResponse;
        public string CorrelationId { get; private set; }
        public HttpStatusCode StatusCode
        {
            get { return originalResponse.StatusCode; }
        }
        public Response(HttpResponseMessage response, string correlationId)
        {
            originalResponse = response;
            CorrelationId = correlationId;
        }

        public HttpResponseMessage RawResponse { get { return originalResponse; } }

        public bool IsSuccessful { get { return originalResponse.IsSuccessStatusCode; } }

        public async Task<string> GetPayloadAsString(CancellationToken cancellationToken)
        {
            return await originalResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
