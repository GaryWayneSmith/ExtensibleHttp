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
            payloadFactory = new PayloadFactory();

        }

        public async Task<SampleResult> PostData<T>(T payload, CancellationToken cancellationToken)
        {
            await new ContextRemover();
            SampleRequest request = new SampleRequest((SampleConfig)config)
            {
                EndpointUri = $"/endpoint_to_call"
            };
            request.AddPayload(payload);
            var response = await client.PostAsync(request, cancellationToken);
            var result = await ProcessResponse<SampleResult>(response, cancellationToken);
            return result;
        }

        public async Task<SampleResult> Getdata(int id, CancellationToken cancellationToken)
        {
            await new ContextRemover();
            SampleRequest request = new SampleRequest((SampleConfig)config)
            {
                EndpointUri = $"/endpoint_to_call/{id}"
            };
            var response = await client.GetAsync(request, cancellationToken);
            var result = await ProcessResponse<SampleResult>(response, cancellationToken);
            return result;
        }
    }
}
