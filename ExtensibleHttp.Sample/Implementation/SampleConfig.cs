using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleHttp.Sample.Implementation
{
	internal class SampleConfig : BaseConfig
	{
		public string ClientId { get; private set; } = string.Empty;
		public string ClientSecret { get; private set; } = string.Empty;
		public string Authorization { get; private set; } = string.Empty;
		public string BearerToken { get; set; } = string.Empty;

		static public SampleToken Token { get; private set; } = new SampleToken();

		class ConfigOptions
		{
			public string BaseUrl { get; set; } = string.Empty;
			public string UserAgent { get; set; } = string.Empty;
			public ApiFormat ApiFormat { get; set; }
			public int RequestTimeoutMs { get; set; }

			public string ClientId { get; set; } = string.Empty;
			public string ClientSecret { get; set; } = string.Empty;

			public string BearerToken { get; set; } = string.Empty;
		}

		private SampleConfig(ConfigOptions options)
			: base()
		{
			BaseUri = new Uri(options.BaseUrl);
			ApiFormat = options.ApiFormat;
			UserAgent = options.UserAgent;
			ClientId = options.ClientId;
			ClientSecret = options.ClientSecret;
			RequestTimeoutMs = options.RequestTimeoutMs;
			BearerToken = options.BearerToken;

			Authorization = "Bearer " + BearerToken;
		}

		public static async Task<SampleConfig> GetConfig(IConfiguration configuration, ApiFormat apiFormat, CancellationToken cancellationToken)
		{
			await Task.CompletedTask;

			ConfigOptions options = new ConfigOptions();
			configuration
				.GetSection("Sample")
				.GetSection("Api")
				.Bind(options);

			if (options == null)
			{
				options = new ConfigOptions();
			}

			if (options.ApiFormat == 0) options.ApiFormat = apiFormat;
			// Minimum time is 5 seconds
			if (options.RequestTimeoutMs < 5000) options.RequestTimeoutMs = 5000;
			await Task.CompletedTask;
			return new SampleConfig(options);
		}

		public async Task ValidateToken(CancellationToken cancellationToken)
		{
			if (!Token.IsExpired)
				return;
			Debug.WriteLine("Renewing Token");

			Token = new SampleToken
			{
				AccessToken = "SOMETOKEN",
				TokenType = "EVERYTHING",
				Expiration = DateTime.UtcNow.AddYears(10),
			};
			//Token = await SampleToken.GetToken(this, cancellationToken);
		}
	}
}
