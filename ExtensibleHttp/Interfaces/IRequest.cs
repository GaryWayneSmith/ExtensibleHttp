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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Interfaces
{
	public interface IRequest
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "It's a partial Uri and as such would throw an error if used as an Uri")]
		string EndpointUri { get; }
		HttpRequestMessage HttpRequest { get; }
		HttpMethod Method { get; set; }
		string BuildQueryParams();
		IRequestConfig Config { get; }
		string CorrelationId { get; }

		Task AddHeaders(CancellationToken cancellationToken);
		Task AddUserAgentHeader(CancellationToken cancellationToken);
		Task BuildUri(CancellationToken cancellationToken);
		Task FinalizeRequest(CancellationToken cancellationToken);
		Task SignRequest(CancellationToken cancellationToken);
		Task ValidateAccessToken(CancellationToken cancellationToken);

		Task ValidateResponse(IResponse response, CancellationToken cancellationToken);


	}
}
