﻿/*
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
using ExtensibleHttp.Fetcher;
using ExtensibleHttp.Interfaces;
using ExtensibleHttp.Retry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp
{
	public class Handler : IHandler
	{
		public static IFetcherFactory FetcherFactory { get; private set; } = new FetcherFactory();
		private readonly IHttpConfig _httpConfig;
		public IFetcher Fetcher { get; private set; }
		public IRetryPolicy RetryPolicy { get; set; }

		public Handler(IHttpConfig apiConfig)
		{
			_httpConfig = apiConfig ?? throw new ArgumentNullException(nameof(apiConfig));
			Fetcher = FetcherFactory.CreateFetcher(_httpConfig);
			RetryPolicy = new SingleTryPolicy();
		}

		private Task<IResponse> ExecuteAsync(IRequest request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			return RetryPolicy.GetResponse(Fetcher, request, cancellationToken);
		}

		public Task<IResponse> GetAsync(IRequest request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			request.Method = HttpMethod.Get;
			return ExecuteAsync(request, cancellationToken);
		}

		public Task<IResponse> PostAsync(IRequest request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			request.Method = HttpMethod.Post;
			return ExecuteAsync(request, cancellationToken);
		}

		public Task<IResponse> PutAsync(IRequest request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			request.Method = HttpMethod.Put;
			return ExecuteAsync(request, cancellationToken);
		}

		public Task<IResponse> DeleteAsync(IRequest request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			request.Method = HttpMethod.Delete;
			return ExecuteAsync(request, cancellationToken);
		}
	}
}
