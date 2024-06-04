## ExtensibleHttp

The minimum configuration required to implement this library is a Config (derived from BaseConfig), a Request (derived from BaseRequest), and the implementation of a Resource (derived from BaseResourceEndpoint)

This framework is currently being utilized for the implementation of Walmart Seller Central API, Amazon SP-API (using the SP, STS, and RD Token models), as well as internal applications.  
The framework callback example included is designed to handle the complex nuances contained in those APIs.

This project was derived from the original works of walmartlabs (https://github.com/walmartlabs/partnerapi_sdk_dotnet), licensed under http://www.apache.org/licenses/LICENSE-2.0



## Implementation

### Sample config

```
    public class SampleConfig : BaseConfig
    {
        private SampleConfig(ILogger logger)
            : base(logger)
        {
                BaseUri = "http://example.net",
                ApiFormat = ApiFormat.Json,
                UserAgent = "MyClient",
        }
    }

```

### Sample request 

```
    public class SampleResource : BaseResourceEndpoint
    {
        public SampleResource(ApiClient apiClient) : base(apiClient)
        {
            PayloadFactory = new PayloadFactory();
        }

        public async Task<string> GetString(CancellationToken cancellationToken)
        {
            await new ContextRemover();
            SampleRequest request = new ((SampleConfig)Config)
            {
                EndpointUri = $"/GetRandomString",
            };

            var response = await Client.GetAsync(request, cancellationToken);
            return = await ProcessResponse<string>(response, cancellationToken);
        }
    }

```

### Sample application
```
    SampleConfig config = new SampleConfig(NullLogger.Instance);
    ApiClient apiClient = new(config);
    SampleResource resource = new(apiClient);
    string result = await resource.GetString(CancellationToken.None);
    Debug.WriteLine(result);

```

## OAuth considerations

The sample project included shows an example of calling authorization endpoints when an OAuth ticket is expired.  
This can be handled by adding a validation check on the token and overriding ValidateAccessToken in the request.

### SampleConfig.cs
```
    static public SampleToken Token { get; private set; } = new SampleToken();
    public async Task ValidateToken(CancellationToken cancellationToken)
    {
        if (!Token.IsExpired)
            return;
        Debug.WriteLine("Renewing Token");
        Token = await SampleToken.GetToken(this, cancellationToken);
    }
```

### SampleRequest.cs

```
	public override async Task ValidateAccessToken(CancellationToken cancellationToken)
	{
		SampleConfig config = (SampleConfig)Config;
		await config.ValidateToken(cancellationToken);
	}
```

## Roadmap

* Integration of IConfiguraton and ILogger into the core system (WIP)
* Redefine the Config to map to &lt;T&gt for easier reference in the base classes
* Sample API endpoints and better sample code.

