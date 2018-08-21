using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class AadTokenHolder
	{
		private const double RefreshTokenInAdvanceTimePercentage = 0.08;

		private string accessToken;

		private string refreshToken;

		private DateTimeOffset expiresOn;

		private DateTimeOffset reAcquireOn;

		private bool refreshableToken;

		private AadAuthParams authParams;

		private string dataSource;

		private bool useAdalCache;

		internal AadTokenHolder(string accessToken, string refreshToken, DateTimeOffset expiresOn, AadAuthParams authParams, string dataSource, bool useAdalCache)
		{
			this.SetValues(accessToken, refreshToken, expiresOn, authParams, dataSource, useAdalCache, true);
		}

		internal AadTokenHolder(string accessToken)
		{
			this.SetValues(accessToken, string.Empty, DateTimeOffset.Now, null, string.Empty, false, false);
		}

		internal string GetValidAccessToken()
		{
			if (this.refreshableToken && DateTimeOffset.Now > this.reAcquireOn)
			{
				AadTokenHolder aadTokenHolder = AadAuthenticator.ReAcquireToken(this.refreshToken, this.authParams, this.dataSource, this.useAdalCache);
				this.SetValues(aadTokenHolder.accessToken, aadTokenHolder.refreshToken, aadTokenHolder.expiresOn, aadTokenHolder.authParams, aadTokenHolder.dataSource, aadTokenHolder.useAdalCache, true);
			}
			return this.accessToken;
		}

		private void SetValues(string accessToken, string refreshToken, DateTimeOffset expiresOn, AadAuthParams authParams, string dataSource, bool useAdalCache, bool refreshableToken)
		{
			this.accessToken = accessToken;
			this.refreshToken = refreshToken;
			this.authParams = authParams;
			this.dataSource = dataSource;
			this.useAdalCache = useAdalCache;
			this.refreshableToken = refreshableToken;
			this.expiresOn = expiresOn;
			double totalSeconds = expiresOn.Subtract(DateTimeOffset.Now).TotalSeconds;
			if (totalSeconds > 0.0)
			{
				double value = totalSeconds * 0.08;
				this.reAcquireOn = expiresOn.Subtract(TimeSpan.FromSeconds(value));
				return;
			}
			this.reAcquireOn = expiresOn;
		}
	}
}
