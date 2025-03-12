using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensibleHttp.Retry
{
	sealed internal class RateLimit
	{
		internal decimal Rate { get; set; }
		internal int Burst { get; set; }
		internal DateTime LastRequest { get; set; }
		internal int RequestsSent { get; set; }

		internal RateLimit(decimal rate, int burst)
		{
			Rate = rate;
			Burst = burst;
			LastRequest = DateTime.UtcNow;
			RequestsSent = 0;
		}
		private int GetRatePeriodMs() { return (int)(((1 / Rate) * 1000) / 1); }

		/// <summary>
		/// Checks to see if the the rate limit has been hit, incrementing the burst usage if it 
		/// has not been.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task ThrottleIfRequired(CancellationToken cancellationToken)
		{

			System.Diagnostics.Debug.WriteLine($"Requests sent: {RequestsSent}, Burst rate: {Burst}");
			int ratePeriodMs = GetRatePeriodMs();
			if (RequestsSent >= Burst)
			{

				LastRequest = LastRequest.AddMilliseconds(ratePeriodMs);
				var tempLastRequest = LastRequest;
				DateTime now = DateTime.UtcNow;
				if (tempLastRequest >= now)
				{
					System.Diagnostics.Debug.WriteLine($"Waiting {tempLastRequest - now} ");
					await Task.Delay(tempLastRequest - now, cancellationToken).ConfigureAwait(false);
				}
			}
			else
			{
				RequestsSent += 1;
			}
			// The last request should go last. Some providers count the request as soon as it's sent, 
			// others once it's completed. If it's a large payload then it might still be off if it's
			// post request.

		}

		/// <summary>
		/// Clears the burst rate if the rate period has passed, one rate point at a time
		/// until full burst is available or the max available rate is reached.
		/// </summary>
		/// <returns></returns>
		public RateLimit UpdateBurstRate()
		{
			if (RequestsSent < 0)
				RequestsSent = 0;

			int ratePeriodMs = GetRatePeriodMs();

			if (RequestsSent >= Burst)
			{
				var lastRequestTime = LastRequest;
				while (true)
				{
					lastRequestTime = lastRequestTime.AddMilliseconds(ratePeriodMs);
					if (lastRequestTime > DateTime.UtcNow)
						break;
					else
						RequestsSent -= 1;

					if (RequestsSent <= 0)
					{
						RequestsSent = 0;
						break;
					}
				}
			}
			return this;
		}

		internal async Task Delay(CancellationToken cancellationToken)
		{
			await Task.Delay(GetRatePeriodMs(), cancellationToken).ConfigureAwait(false);
		}
	}
}
