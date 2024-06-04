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
using ExtensibleHttp.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Reflection;

namespace ExtensibleHttp
{
	public abstract class BaseConfig : IRequestConfig, IApiClientConfig
	{
		public Uri BaseUri { get; set; }
		public string UserAgent { get; set; }

		public ILogger Logger { get; set; }

		/// <summary>
		/// API payload format
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public ApiFormat ApiFormat { get; set; }

		public int RequestTimeoutMs { get; set; } = 100000; // in milliseconds

		public string NewCorrelationId()
		{
			return Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Override this to provider your own customized contenttype matching
		/// 
		/// Defaults to whatever is passed in if using fixed type of ApiFormat
		/// </summary>
		/// <param name="apiFormat"></param>
		/// <returns></returns>
		public virtual string GetContentType(string apiFormat)
		{
			return apiFormat;
		}

		/// <summary>
		/// Use this for basic default xml and json format types.  Anything exotic 
		/// should use the GetContentType(string) signature.
		/// </summary>
		/// <param name="apiFormat"></param>
		/// <returns></returns>
		public string GetContentType(ApiFormat apiFormat)
		{
			switch (apiFormat)
			{
				case ApiFormat.Json:
					return "application/json";
				default:
				case ApiFormat.Xml:
					return "application/xml";
			}
		}

		protected BaseConfig()
			: this(null)
		{

		}

		protected BaseConfig(ILogger logger)
		{
			var assembly = GetType().GetTypeInfo().Assembly;
			UserAgent = $".Net_{assembly.GetName().Name}_v{assembly.GetName().Version}";
			Logger = logger ?? NullLogger.Instance;
		}

		public IRequestConfig GetRequestConfig() => this;

		public virtual BaseApiClient GetApiClient()
		{
			return new ApiClient(this);
		}

	}
}
