// See https://aka.ms/new-console-template for more information
using ExtensibleHttp;
using ExtensibleHttp.Sample.Implementation;
using System.Threading;
Console.WriteLine("Hello, World!");

SampleConfig config = await SampleConfig.GetConfig(ApiFormat.Json, CancellationToken.None);
ApiClient apiClient = new ApiClient(config);
SampleResource resource = new SampleResource(apiClient);
SampleResult result = await resource.Getdata(1, CancellationToken.None);
