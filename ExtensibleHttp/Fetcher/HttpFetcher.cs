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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Fetcher
{
	public class HttpFetcher : BaseFetcher
	{
		public IHttpClient Client { get; private set; }

		public HttpFetcher(IHttpConfig httpConfig, IHttpClient httpClient) 
			: base(httpConfig)
		{
			if (httpConfig == null) throw new ArgumentNullException(nameof(httpConfig));

			Client = httpClient;
			Client.BaseAddress = httpConfig.BaseUri;
			Client.Timeout = TimeSpan.FromMilliseconds(httpConfig.RequestTimeoutMs);
		}

		override public async Task<IResponse> ExecuteAsync(IRequest request, CancellationToken cancellationToken)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}
			if (string.IsNullOrWhiteSpace(request.EndpointUri))
			{
				throw new InvalidValueException("Empty URI for the endpoint!");
			}

			await request.ValidateAccessToken(cancellationToken).ConfigureAwait(false);
			await request.AddUserAgentHeader(cancellationToken).ConfigureAwait(false);
			await request.BuildUri(cancellationToken).ConfigureAwait(false);
			await request.FinalizeRequest(cancellationToken).ConfigureAwait(false);
			await request.AddHeaders(cancellationToken).ConfigureAwait(false);
			await request.SignRequest(cancellationToken).ConfigureAwait(false);

			try
			{
				var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);

				await request.ValidateResponse(response, cancellationToken).ConfigureAwait(false);

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
