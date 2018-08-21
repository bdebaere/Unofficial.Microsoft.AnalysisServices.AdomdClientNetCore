using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class AadAuthenticator
	{
		private const string ResourceId = "https://{0}";

		private static readonly Uri RedirectUri = new Uri("urn:ietf:wg:oauth:2.0:oob");

		internal static AadTokenHolder AcquireToken(Uri dataSourceUri, string dataSource, string identityProvider, string userId, string password, bool useAdalCache)
		{
			if (userId == null && password != null)
			{
				return new AadTokenHolder(password);
			}
			AadAuthParams aadAuthParams = AadAuthParams.FindMatchingAuthParams(identityProvider, dataSourceUri);
			string text = string.Format("https://{0}", dataSourceUri.Host);
			AadTokenHolder result;
			using (new AdalRuntimeLibrary.Usage())
			{
				AdalRuntimeLibrary instance = AdalRuntimeLibrary.Instance;
				object obj;
				if (useAdalCache)
				{
					obj = AadAuthenticator.CreateAuthenticationContextWithCache(aadAuthParams, dataSource);
					AadAuthParams aadAuthParams2;
					if (aadAuthParams.IsCommonTenant && AadAuthenticator.TryToUpdateTenant(aadAuthParams, obj, userId, out aadAuthParams2))
					{
						aadAuthParams = aadAuthParams2;
						obj = AadAuthenticator.CreateAuthenticationContextWithCache(aadAuthParams, dataSource);
					}
				}
				else
				{
					obj = Activator.CreateInstance(instance.AuthenticationContextT, new object[]
					{
						aadAuthParams.Authority
					});
				}
				object obj3;
				if (userId != null && password != null)
				{
					object obj2 = Activator.CreateInstance(instance.UserCredentialT, new object[]
					{
						userId,
						password
					});
					obj3 = instance.AcquireTokenCredentialsM.Invoke(obj, new object[]
					{
						text,
						aadAuthParams.ApplicationId,
						obj2
					});
				}
				else if (userId != null)
				{
					object obj4 = Activator.CreateInstance(instance.UserIdentifierT, new object[]
					{
						userId,
						instance.OptionalDisplayableIdV
					});
					obj3 = instance.AcquireTokenUserIdM.Invoke(obj, new object[]
					{
						text,
						aadAuthParams.ApplicationId,
						AadAuthenticator.RedirectUri,
						instance.PromptBehaviorAutoV,
						obj4
					});
				}
				else
				{
					obj3 = instance.AcquireTokenM.Invoke(obj, new object[]
					{
						text,
						aadAuthParams.ApplicationId,
						AadAuthenticator.RedirectUri,
						instance.PromptBehaviorAutoV
					});
				}
				result = new AadTokenHolder((string)instance.AccessTokenP.GetValue(obj3, null), (string)instance.RefreshTokenP.GetValue(obj3, null), (DateTimeOffset)instance.ExpiresOnP.GetValue(obj3, null), aadAuthParams, dataSource, useAdalCache);
			}
			return result;
		}

		internal static AadTokenHolder ReAcquireToken(string refreshToken, AadAuthParams authParams, string dataSource, bool useAdalCache)
		{
			AadTokenHolder result;
			using (new AdalRuntimeLibrary.Usage())
			{
				AdalRuntimeLibrary instance = AdalRuntimeLibrary.Instance;
				object obj = useAdalCache ? AadAuthenticator.CreateAuthenticationContextWithCache(authParams, dataSource) : Activator.CreateInstance(instance.AuthenticationContextT, new object[]
				{
					authParams.Authority
				});
				object obj2 = instance.AcquireTokenByRefreshTokenM.Invoke(obj, new string[]
				{
					refreshToken,
					authParams.ApplicationId
				});
				result = new AadTokenHolder((string)instance.AccessTokenP.GetValue(obj2, null), (string)instance.RefreshTokenP.GetValue(obj2, null), (DateTimeOffset)instance.ExpiresOnP.GetValue(obj2, null), authParams, dataSource, useAdalCache);
			}
			return result;
		}

		private static object CreateAuthenticationContextWithCache(AadAuthParams authParams, string dataSource)
		{
			AdalRuntimeLibrary instance = AdalRuntimeLibrary.Instance;
			object obj = Activator.CreateInstance(instance.TokenCacheT);
			new AdalRuntimeLibrary.CacheLocalStorage(obj, dataSource);
			return Activator.CreateInstance(instance.AuthenticationContextT, new object[]
			{
				authParams.Authority,
				obj
			});
		}

		private static bool TryToUpdateTenant(AadAuthParams authParams, object authContext, string userId, out AadAuthParams authParamsUpdated)
		{
			AdalRuntimeLibrary instance = AdalRuntimeLibrary.Instance;
			object value = instance.TokenCacheP.GetValue(authContext, null);
			foreach (object current in ((IEnumerable)instance.ReadItemsM.Invoke(value, null)))
			{
				string text = (string)instance.AuthorityP.GetValue(current, null);
				string b = (string)instance.ClientIdP.GetValue(current, null);
				string b2 = (string)instance.DisplayableIdP.GetValue(current, null);
				if (authParams.ApplicationId == b && AadAuthParams.IsSameInstance(authParams.Authority, text) && (userId == null || userId == b2))
				{
					authParamsUpdated = new AadAuthParams
					{
						Authority = text,
						ApplicationId = authParams.ApplicationId,
						DomainPostfix = authParams.DomainPostfix
					};
					return true;
				}
			}
			authParamsUpdated = null;
			return false;
		}
	}
}
