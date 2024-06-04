// See https://aka.ms/new-console-template for more information
using ExtensibleHttp;
using ExtensibleHttp.Sample.Implementation;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;


SampleConfig config = await SampleConfig.GetConfig(NullLogger.Instance, ApiFormat.Json, CancellationToken.None);
ApiClient apiClient = new(config);
SampleResource resource = new(apiClient);
for (int i = 0; i < 15; i++)
{
    string force = string.Empty;
    if (i % 3 == 0)
        force = "yes";
    else if (i % 7 == 0)
        force = "maybe";

    SampleResult result = await resource.Getdata(force, CancellationToken.None);
    Debug.WriteLine(result.ToString());
}
