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
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Retry
{
	public abstract class BasePolicy : IRetryPolicy
	{
#pragma warning disable CA1721 // Property names should not match get methods
		public IFetcher Fetcher { get; set; }
		protected IResponse Response { get; private set; }
		protected Exception LatestException { get; private set; }
#pragma warning restore CA1721 // Property names should not match get methods

		protected async Task<bool> ExecuteOnce(IFetcher fetcher, IRequest request, CancellationToken cancellationToken)
		{
			if (fetcher == null) throw new ArgumentNullException(nameof(fetcher));
			if (request == null) throw new ArgumentNullException(nameof(request));

			try
			{
				Response = await fetcher.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
				return true;
			}
			catch (HttpException ex)
			{
				LatestException = ex;
				return false;
			}
		}

		public abstract Task<IResponse> GetResponse(IFetcher fetcher, IRequest request, CancellationToken cancellationToken);
	}
}
