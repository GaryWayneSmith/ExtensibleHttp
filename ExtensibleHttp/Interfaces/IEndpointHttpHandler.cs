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

using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Interfaces
{
	public interface IEndpointHttpHandler
	{
		IFetcher Fetcher { get; }
		Task<IResponse> GetAsync(IRequest request, CancellationToken cancellationToken);
		Task<IResponse> PostAsync(IRequest request, CancellationToken cancellationToken);
		Task<IResponse> PutAsync(IRequest request, CancellationToken cancellationToken);
		Task<IResponse> DeleteAsync(IRequest request, CancellationToken cancellationToken);
		Task<IResponse> ExecuteAsync(IRequest request, CancellationToken cancellationToken);

		Task<IResponse> GetAsync(IRequest request, IRetryPolicy retryPolicy, CancellationToken cancellationToken);
		Task<IResponse> PostAsync(IRequest request, IRetryPolicy retryPolicy, CancellationToken cancellationToken);
		Task<IResponse> PutAsync(IRequest request, IRetryPolicy retryPolicy, CancellationToken cancellationToken);
		Task<IResponse> DeleteAsync(IRequest request, IRetryPolicy retryPolicy, CancellationToken cancellationToken);
		Task<IResponse> ExecuteAsync(IRequest request, IRetryPolicy retryPolicy, CancellationToken cancellationToken);

	}
}
