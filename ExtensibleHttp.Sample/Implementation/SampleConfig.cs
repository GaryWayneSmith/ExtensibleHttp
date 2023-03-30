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
        public string ChannelType { get; private set; }
        public string ServiceName { get; private set; }
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public string Authorization { get; private set; }
        static public SampleToken Token { get; private set; } = new SampleToken();

        public static async Task<SampleConfig> GetConfig(ApiFormat apiFormat, CancellationToken cancellationToken)
        {
            string customerId = "";
            string customerSecret = "";
            string baseUrl = "";
            string channelType = "";
            string userAgent = "";
            string serviceName = "";

            return new SampleConfig()
            {
                BaseUrl = baseUrl,
                ChannelType = channelType,
                ApiFormat = apiFormat,
                UserAgent = userAgent,
                ServiceName = serviceName,
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
            Token = await SampleToken.GetToken(this, cancellationToken);
        }
    }
}
