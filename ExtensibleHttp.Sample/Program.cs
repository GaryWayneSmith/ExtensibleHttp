// See https://aka.ms/new-console-template for more information
using ExtensibleHttp;
using ExtensibleHttp.Sample.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;



IConfiguration configuration = new ConfigurationBuilder()
	.AddUserSecrets("ExtensibleHttp.Sample")
	.AddJsonFile("appsettings.json", false, true)
	.Build();


// Burst 10, recovering at a rate of 0.5 requests per second
// 2 seconds to recover a request
//ExtensibleHttp.Retry.RateLimitPolicy.AppendPolicy("Sample", 0.5m, 10);
// Burst 2, recovering at a rate of 0.1 requests per second
// 10 seconds to recover a request
ExtensibleHttp.Retry.RateLimitPolicy.AppendPolicy("Sample", 0.1m, 2);



SampleConfig config = await SampleConfig.GetConfig(configuration, ApiFormat.Json, CancellationToken.None);
ApiClient apiClient = new(config);
SampleResource resource = new(apiClient);
for (int i = 0; i < 15; i++)
{
	var result = await resource.Getdata("star", CancellationToken.None);
	//Debug.WriteLine($"Index: {i}, Length of result: {result.ToString().Length}");
	//Debug.WriteLine(result.ToString());
	 if (i == 6)
		await Task.Delay(30000);

}
