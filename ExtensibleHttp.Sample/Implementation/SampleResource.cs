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

        public async Task<SampleResult> Getdata(string force, CancellationToken cancellationToken)
        {
            await new ContextRemover();
            SampleRequest request;

            if (string.IsNullOrWhiteSpace(force))
            {
                request = new SampleRequest((SampleConfig)Config)
                {
                    EndpointUri = $"/api",
                };
            }
            else
            {
                request = new SampleRequest((SampleConfig)Config)
                {
                    EndpointUri = $"/api?force={force}",
                };
            }

            var response = await Client.GetAsync(request, cancellationToken);
            var result = await ProcessResponse<SampleResult>(response, cancellationToken);
            return result;
        }
    }
}
