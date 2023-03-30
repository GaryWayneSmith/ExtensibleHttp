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
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Fetcher
{
    public class HttpFetcher : BaseFetcher
    {
        public IHttpClient client;

        public HttpFetcher(IHttpConfig config, IHttpClient httpClient) : base(config)
        {
            client = httpClient;
            client.BaseAddress = new Uri(config.BaseUrl);
            client.Timeout = TimeSpan.FromMilliseconds(config.RequestTimeoutMs);
        }

        override public async Task<IResponse> ExecuteAsync(IRequest request, CancellationToken cancellationToken)
        {
            if (request.EndpointUri == "")
            {
                throw new InvalidValueException("Empty URI for the endpoint!");
            }

            await request.ValidateAccessToken(cancellationToken);
            await request.AddUserAgentHeader(cancellationToken);
            await request.BuildUri(cancellationToken);
            await request.FinalizeRequest(cancellationToken);
            await request.AddHeaders(cancellationToken);
            await request.SignRequest(cancellationToken);

            try
            {
                //await Util.LogToFile.WriteLogString(request.CorrelationId, request.HttpRequest.RequestUri.ToString(), "Uri", ".txt");
                //await Util.LogToFile.WriteLogString(request.CorrelationId, request.HttpRequest.Content != null ? await request.HttpRequest.Content.ReadAsStringAsync() : "NO PAYLOAD", "Request", config.ApiFormat.ToString().ToLower());

                var response = await client.SendAsync(request, cancellationToken);

                await request.ValidateResponse(response, cancellationToken);

                return response;
            }
            catch (Exception ex) when (IsNetworkError(ex) || ex is TaskCanceledException)
            {
                // unable to connect to API because of network/timeout
                throw new ConnectionException("Network error while connecting to the API", ex);
            }
        }

        private static bool IsNetworkError(Exception ex)
        {
            if (ex is SocketException)
                return true;
            if (ex.InnerException != null)
                return IsNetworkError(ex.InnerException);
            return false;
        }
    }
}
