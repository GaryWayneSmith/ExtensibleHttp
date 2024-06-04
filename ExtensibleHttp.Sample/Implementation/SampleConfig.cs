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

        static public SampleToken Token { get; private set; } = new SampleToken();

        private SampleConfig(ILogger logger)
            : base(logger)
        { }

        public static async Task<SampleConfig> GetConfig(ILogger logger, ApiFormat apiFormat, CancellationToken cancellationToken)
        {
            string customerId = "Test11";
            string customerSecret = "Password22";
            Uri baseUrl = new("https://yesno.wtf");
            string userAgent = "ExtensibleHttpClient";

            await Task.CompletedTask;

            return new SampleConfig(logger)
            {
                BaseUri = baseUrl,
                ApiFormat = apiFormat,
                UserAgent = userAgent,
                ClientId = customerId,
                ClientSecret = customerSecret,
                Authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(customerId + ":" + customerSecret)),
            };
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
