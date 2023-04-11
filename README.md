# ExtensibleHttp

This framework is currently being utilized for the implementation of Walmart Seller Central API, Amazon SP-API (using the SP, STS, and RD Token models), as well as internal applications.  The framework callback example included is designed to handle the complex nuances contained in those APIs.

This project was derived from the original works of walmartlabs (https://github.com/walmartlabs/partnerapi_sdk_dotnet), licensed under http://www.apache.org/licenses/LICENSE-2.0

The minimum configuration required to implement this library is a Config (derived from BaseConfig), a Request (derived from BaseRequest), and the implementation of a Resource (derived from BaseResourceEndpoint)

The derived Config class can be loaded via a simple method such as the static implementation of GetConfig() shown in SampleConfig, where the implementation can pull values from your preferred data source. The config class will generally also hold the validation logic for retrieving tokens from OAuth.  The logic for having the OAuth logic, and token, reside in the config class is so you can have multiple ApiClient instances derived from multiple instances of Config, each maintaining its own state engine. The use of the SemaphoreSlim is to ensure that multiple callers are not creating multiple tickets at the same time.  This logic may not suit your needs and should be modified accordingly.  

The derived Request class can implement several overrides that will be used within the Resource class. This includes a callback for token validation within the pipeline for cases where OAuth is required.

The derived Resource class, or ResourceEndpoint class, is used to make the calls necessary to execute and process the data call.  Each call should have its own method.

As mentioned, this is currently being used for both Amazon and Walmart API library calls in a multi-threaded environment.
