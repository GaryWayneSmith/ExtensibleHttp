using ExtensibleHttp.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ExtensibleHttp.Interfaces;

namespace ExtensibleHttp.Sample.Implementation
{
    internal class SampleToken : IExpirable
    {
        readonly static HttpClient _httpClient = new();
        readonly static SemaphoreSlim _lock = new(1);

        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }

        public bool IsExpired
        {
            get
            {
                return DateTime.UtcNow > Expiration;
            }
        }

        #region Private Classes

        [XmlRootAttribute("OAuthTokenDTO")]
        class OAuthToken
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonProperty("token_type")]
            public string TokenType { get; set; } = string.Empty;

            [JsonProperty("expires_in")]
            public int Expires { get; set; }
        }

        #endregion Private Classes

        public static async Task<SampleToken> GetToken(SampleConfig config, CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {

                using HttpRequestMessage requestMessage = new(HttpMethod.Post, "https://oauthapihost/token");
                string correlationId = Guid.NewGuid().ToString();
                requestMessage.Headers.Add(SampleHeaders.USER_AGENT, config.UserAgent);
                requestMessage.Headers.Add(SampleHeaders.AUTHORIZATION, config.Authorization);
                requestMessage.Headers.Add(SampleHeaders.CORRELATION_ID, correlationId);

                requestMessage.Headers.Add(SampleHeaders.ACCEPT, config.GetContentType(ApiFormat.Json));
                requestMessage.Content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                });

                var response = await _httpClient
                    .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                OAuthToken oAuthToken = new SerializerFactory()
                    .GetSerializer(ApiFormat.Json)
                    .Deserialize<OAuthToken>(result);

                return new SampleToken
                {
                    AccessToken = oAuthToken.AccessToken,
                    TokenType = oAuthToken.TokenType,
                    Expiration = DateTime.UtcNow.AddSeconds(oAuthToken.Expires - 30),
                };
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
