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
using ExtensibleHttp.Exceptions;
using ExtensibleHttp.Interfaces;
using ExtensibleHttp.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp
{
	public abstract class BaseRequest : IRequest //<T> where T : IRequestConfig, new()
	{
		public IRequestConfig Config => _config;
		public string EndpointUri { get; set; }
		public HttpRequestMessage HttpRequest { get; private set; }
		public Dictionary<string, IEnumerable<string>> QueryParams { get; private set; } = new Dictionary<string, IEnumerable<string>>();
		public string CorrelationId { get; private set; }
		protected static Dictionary<string, IEnumerable<string>> EMPTYHEADERS { get; private set; } = new Dictionary<string, IEnumerable<string>>();
		readonly IRequestConfig _config;

		public ApiFormat ApiFormat { get; set; }

		public HttpMethod Method
		{
			get { return HttpRequest.Method; }
			set { HttpRequest.Method = value; }
		}

		protected BaseRequest(IRequestConfig config)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			HttpRequest = new HttpRequestMessage();
			CorrelationId = config.NewCorrelationId();
			ApiFormat = config.ApiFormat;
		}

		public virtual void AddMultipartContent(byte[] content)
		{
#pragma warning disable CA2000 // Dispose objects before losing scope
			var multipartContent = new MultipartFormDataContent
			{
				new ByteArrayContent(content)
			};
#pragma warning restore CA2000 // Dispose objects before losing scope

			HttpRequest.Content = multipartContent;
		}

		public virtual void AddMultipartContent(System.IO.Stream contentStream)
		{
#pragma warning disable CA2000 // Dispose objects before losing scope
			var multipartContent = new MultipartFormDataContent
			{
				new StreamContent(contentStream)
			};
#pragma warning restore CA2000 // Dispose objects before losing scope

			HttpRequest.Content = multipartContent;
		}

		public virtual void AddPayload<T>(T payload)
		{
			var data = new SerializerFactory().GetSerializer(ApiFormat).Serialize(payload);
			Debug.WriteLine(data);
			AddPayload(data);
		}

		public virtual void AddPayload(string payload)
		{
			System.Net.Http.Headers.MediaTypeHeaderValue mediaType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(Config.GetContentType(Config.ApiFormat));
			HttpRequest.Content = new StringContent(payload, Encoding.UTF8, mediaType.MediaType);
			HttpRequest.Content.Headers.ContentType = mediaType;
		}

		public virtual string BuildQueryParams()
		{
			var list = new List<string>();
			foreach (var param in QueryParams)
			{
				if (param.Value != null && param.Value.Any())
				{
					string value = string.Empty;

					if (param.Value.Count() > 1)
						value = string.Join(",", param.Value.Select(s => Uri.EscapeDataString(s)));
					else
						value = param.Value.First();

					list.Add(Uri.EscapeDataString(param.Key) + "=" + Uri.EscapeDataString(value));

					//foreach (var value in param.Value)
					//{
					//	list.Add(Uri.EscapeDataString(param.Key) + "=" + Uri.EscapeDataString(value));
					//}
				}
			}
			if (list.Count > 0)
			{
				return string.Join("&", list);
			}
			return "";
		}

		public virtual Task AddHeaders(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public virtual Task AddUserAgentHeader(CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(Config.UserAgent)) throw new InvalidValueException("Config.UserAgent");

			HttpRequest.Headers.Add(BaseHeaders.USER_AGENT, Config.UserAgent.Replace(' ', '_'));
			return Task.CompletedTask;
		}

		public virtual Task BuildUri(CancellationToken cancellationToken)
		{
			string queryString = BuildQueryParams();

			if (!string.IsNullOrWhiteSpace(queryString))
			{
#pragma warning disable CA1307 // Specify StringComparison for clarity
				if (!EndpointUri.Contains('?'))
				{
					queryString = "?" + queryString;
				}
				else
				{
					queryString = "&" + queryString;
				}
#pragma warning restore CA1307 // Specify StringComparison for clarity
			}

			HttpRequest.RequestUri = new Uri(Config.BaseUri, EndpointUri + queryString);
			return Task.CompletedTask;
		}

		public virtual Task FinalizeRequest(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public virtual Task SignRequest(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public virtual Task ValidateAccessToken(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}


		public virtual Task ValidateResponse(IResponse response, CancellationToken cancellationToken)
		{
			if (response == null) throw new ArgumentNullException(nameof(response));

			if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
			{
				// 503 Service Unavailable
				throw new GatewayException("Service is unavailable, gateway connection error");
			}

			if (response.StatusCode == (HttpStatusCode)429)
			{
				// 429 Too many requests
				throw new ThrottleException("HTTP request was throttled");
			}
			return Task.CompletedTask;
		}
	}
}
