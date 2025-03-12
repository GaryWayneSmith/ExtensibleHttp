using ExtensibleHttp.Exceptions;
using ExtensibleHttp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Retry
{
	public class RateLimitPolicy : BasePolicy
	{
		internal static ConcurrentDictionary<string, RateLimit> UsagePlansTimings { get; set; } = new ConcurrentDictionary<string, RateLimit>();

		static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

		public int RetryCount { get; private set; } = 3;
		public int DelayMs { get; set; }

		public string RateLimitType { get; private set; }

		public RateLimitPolicy(string rateLimitType, int retryCount = 1)
		{
			if (string.IsNullOrWhiteSpace(rateLimitType))
			{
				throw new InitException("Rate limit type should be set");
			}
			if (retryCount < 1)
			{
				throw new InitException("Retry count should be more than 0");
			}
			if (!UsagePlansTimings.ContainsKey(rateLimitType))
			{
				throw new InitException($"Rate Policy does not contain Rate limit type of {rateLimitType}");
			}

			RetryCount = retryCount;
			RateLimitType = rateLimitType;
		}

		/// <summary>
		/// Used to set up a rate limit policy for a specific rate limit type. If the rate limit 
		/// already exists, the original will be used.
		/// </summary>
		/// <param name="rateLimitType"></param>
		/// <param name="rate"></param>
		/// <param name="burst"></param>
		public static void AppendPolicy(string rateLimitType, decimal rate, int burst)
		{
			_semaphoreSlim.Wait();
			try
			{
				UsagePlansTimings.GetOrAdd(rateLimitType, new RateLimit(rate, burst));
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}

		public override async Task<IResponse> GetResponse(IFetcher fetcher, IRequest request, CancellationToken cancellationToken)
		{
			var tryCount = 0;
			do
			{
				try
				{
					await UsagePlansTimings[RateLimitType]
						.UpdateBurstRate()
						.ThrottleIfRequired(cancellationToken).ConfigureAwait(false);

					if (await ExecuteOnce(fetcher, request, cancellationToken).ConfigureAwait(false))
					{
						// The LastRequest should go here. Some providers count the request as soon as it's sent, 
						// others once it's completed. If it's a large payload then it might still be off if it's
						// post request.
						UsagePlansTimings[RateLimitType].LastRequest = DateTime.UtcNow;
						return Response;
					}
					else
					{
						throw LatestException;
					}
				}
				catch (ThrottleException ex)
				{
					if (tryCount >= RetryCount)
					{
						throw NoRetriesLeftException.Factory(RetryCount, ex);
					}
					cancellationToken.ThrowIfCancellationRequested();
					await UsagePlansTimings[RateLimitType].Delay(cancellationToken).ConfigureAwait(false);
					tryCount++;
				}
			}
			while (true);
		}
	}
}
