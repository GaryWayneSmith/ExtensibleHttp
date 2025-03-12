using ExtensibleHttp.Endpoint;
using ExtensibleHttp.Interfaces;
using ExtensibleHttp.Payload;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ExtensibleHttp.Sample.Implementation
{
	internal class SampleResource : BaseResourceEndpoint
	{
		public SampleResource(ApiClient apiClient) : base(apiClient)
		{
			PayloadFactory = new PayloadFactory();
		}

		public async Task<SampleResult> PostData<T>(T payload, CancellationToken cancellationToken)
		{
			await new ContextRemover();
			SampleRequest request = new((SampleConfig)Config)
			{
				EndpointUri = $"/endpoint_to_call"
			};
			request.AddPayload(payload);
			var response = await Client.PostAsync(request, cancellationToken);
			var result = await ProcessResponse<SampleResult>(response, cancellationToken);
			return result;
		}

		public async Task<object> Getdata(string search, CancellationToken cancellationToken)
		{
			await new ContextRemover();
			SampleRequest request;

			// Create instanse of RateLimitPolicy of type Sample
			IRetryPolicy rateLimitPolicy = new Retry.RateLimitPolicy("Sample", 3);

			request = new SampleRequest((SampleConfig)Config)
			{
				EndpointUri = $"/v1/search/query",
			};

			request.AddQueryParam("q", "search");

			// Call with RateLimitPolicy
			var response = await Client.GetAsync(request, rateLimitPolicy, cancellationToken);
			var result = await ProcessStringResponse(response, cancellationToken);
			return result;
		}
	}
}
