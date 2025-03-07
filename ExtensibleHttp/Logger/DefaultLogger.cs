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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Logger
{
	public class DefaultLogger : DelegatingHandler
	{
		internal const string LOGGERKEY = "Logger";

		public static ILogger Logger { get; set; } = NullLogger.Instance;

		public DefaultLogger(HttpMessageHandler innerHandler) : base(innerHandler)
		{ }

		private static async Task<string> GenerateRequestMessage(HttpRequestMessage request)
		{
			var sBuilder = new StringBuilder();
			using (var sWriter = new StringWriter(sBuilder))
			{
				await sWriter.WriteLineAsync("Request:").ConfigureAwait(false);
				await sWriter.WriteLineAsync(request.ToString()).ConfigureAwait(false);
				if (request.Content != null)
				{
					await sWriter.WriteLineAsync(await request.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);
				}
				await sWriter.WriteLineAsync().ConfigureAwait(false);
				return sBuilder.ToString();
			}
		}

		private static async Task<string> GenerateResponseLog(HttpResponseMessage response)
		{
			var sBuilder = new StringBuilder();
			using (var sWriter = new StringWriter(sBuilder))
			{

				await sWriter.WriteLineAsync("Response:").ConfigureAwait(false);
				await sWriter.WriteLineAsync(response.ToString()).ConfigureAwait(false);
				if (response.Content != null)
				{
					await sWriter.WriteLineAsync(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);
				}
				await sWriter.WriteLineAsync().ConfigureAwait(false);
				return sBuilder.ToString();
			}
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			if (Logger.IsEnabled(LogLevel.Debug))
			{
				string debugMessage = await GenerateRequestMessage(request).ConfigureAwait(false);
#pragma warning disable CA1848 // Use the LoggerMessage delegates
				Logger.LogDebug("{Message}", debugMessage);

				HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

				debugMessage = await GenerateResponseLog(response).ConfigureAwait(false);
				Logger.LogDebug("{Message}", debugMessage);
#pragma warning restore CA1848 // Use the LoggerMessage delegates

				return response;
			}
			else
			{
				// Logging is off or not configured.  Don't waste time on logging
				return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
			}

		}
	}
}
