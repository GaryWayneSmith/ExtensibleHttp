using ExtensibleHttp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleHttp.Sample.Implementation
{
	internal class SampleRequest : BaseRequest
	{
		public SampleRequest(IRequestConfig config)
		: base(config)
		{

		}

		public override async Task AddUserAgentHeader(CancellationToken cancellationToken)
		{
			HttpRequest.Headers.Add(SampleHeaders.USER_AGENT, Config.UserAgent);
			await base.AddUserAgentHeader(cancellationToken);
		}

		public override async Task AddHeaders(CancellationToken cancellationToken)
		{
			SampleConfig config = (SampleConfig)Config;

			HttpRequest.Headers.Add(SampleHeaders.AUTHORIZATION, config.Authorization);
			HttpRequest.Headers.Add(SampleHeaders.CORRELATION_ID, CorrelationId);
			// Must go last.
			HttpRequest.Headers.Add(SampleHeaders.ACCEPT, Config.GetContentType(Config.ApiFormat));

			//HttpRequest.Headers.Add("accept", "text/json");
			await base.AddHeaders(cancellationToken);
		}

		public override async Task ValidateAccessToken(CancellationToken cancellationToken)
		{
			SampleConfig config = (SampleConfig)Config;
			await config.ValidateToken(cancellationToken);

		}
	}
}
