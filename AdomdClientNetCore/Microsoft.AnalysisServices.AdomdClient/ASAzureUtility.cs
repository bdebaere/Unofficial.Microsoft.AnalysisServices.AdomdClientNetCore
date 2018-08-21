using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class ASAzureUtility
	{
		[DataContract]
		private class NameResolutionRequest
		{
			[DataMember(Name = "serverName")]
			public string ServerName
			{
				get;
				set;
			}
		}

		[DataContract]
		private class NameResolutionResult
		{
			[DataMember(Name = "clusterFQDN")]
			public string ClusterFqdn
			{
				get;
				set;
			}
		}

		internal class ClustersCache
		{
			private const string AppDomainClusterCacheLockName = "MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE_USAGE_LOCK";

			private const string AsDataSourceClusterUrisCacheKey = "MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE";

			internal static void AddCluster(Uri dataSourceUri, string asAzureServerName, Uri clusterUri)
			{
				lock (ASAzureUtility.GetAppDomainLock("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE_USAGE_LOCK"))
				{
					string key = ASAzureUtility.ClustersCache.getKey(dataSourceUri, asAzureServerName);
					if (AppDomain.CurrentDomain.GetData("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE") == null)
					{
						AppDomain.CurrentDomain.SetData("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE", new Dictionary<string, Uri>());
					}
					((Dictionary<string, Uri>)AppDomain.CurrentDomain.GetData("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE"))[key] = clusterUri;
				}
			}

			internal static Uri GetCluster(Uri dataSourceUri, string asAzureServerName)
			{
				Uri result;
				lock (ASAzureUtility.GetAppDomainLock("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE_USAGE_LOCK"))
				{
					string key = ASAzureUtility.ClustersCache.getKey(dataSourceUri, asAzureServerName);
					Dictionary<string, Uri> dictionary = (Dictionary<string, Uri>)AppDomain.CurrentDomain.GetData("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE");
					result = ((dictionary != null) ? (dictionary.ContainsKey(key) ? dictionary[key] : null) : null);
				}
				return result;
			}

			internal static void Invalidate()
			{
				lock (ASAzureUtility.GetAppDomainLock("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE_USAGE_LOCK"))
				{
					Dictionary<string, Uri> dictionary = (Dictionary<string, Uri>)AppDomain.CurrentDomain.GetData("MICROSOFT_ANALYSISSERVICES_XMLA_LIBS_DATA_SOURCE_CLUSTER_URIS_CACHE");
					if (dictionary != null)
					{
						dictionary.Clear();
					}
				}
			}

			private static string getKey(Uri dataSourceUri, string asAzureServerName)
			{
				return string.Format("{0} + {1}", dataSourceUri, asAzureServerName);
			}
		}

		private const string AsAzureProtocolScheme = "asazure://";

		private const string HttpAuthorizationHeaderName = "Authorization";

		private const string HttpAuthenticationBearerSchemeName = "Bearer";

		private const string ASAzureRootActivityIdHeaderName = "x-ms-root-activity-id";

		private const string ASAzureCurrentUtcDateHeaderName = "x-ms-current-utc-date";

		private const string ASAzureErrorDetailsHeaderName = "x-ms-xmlaerror-extended";

		private const string XmlaExecutionUriPath = "/webapi/xmla";

		private const string NameResolutionUriPath = "/webapi/clusterResolve";

		private const uint SCS_32BIT_BINARY = 0u;

		private const uint SCS_64BIT_BINARY = 6u;

		private static readonly object getAppDomainLockObj = new object();

		internal static bool IsAsAzureInstance(string dataSourceUri)
		{
			return dataSourceUri != null && dataSourceUri.StartsWith("asazure://", StringComparison.InvariantCultureIgnoreCase);
		}

		internal static bool DataSourceUriWithOnlyServerName(string dataSourceUri, out string serverName)
		{
			Uri uri = new Uri(dataSourceUri);
			if (string.IsNullOrEmpty(uri.AbsolutePath) || !uri.AbsolutePath.StartsWith("/") || uri.AbsolutePath.Length == 1 || uri.AbsolutePath.IndexOf("/", 1, StringComparison.InvariantCulture) != -1)
			{
				serverName = null;
				return false;
			}
			serverName = uri.AbsolutePath.Substring(1);
			return true;
		}

		internal static string ConstructAsAzureSecureServerConnUri(string dataSourceUri)
		{
			return new UriBuilder(dataSourceUri)
			{
				Scheme = Uri.UriSchemeHttps,
				Path = "/webapi/xmla"
			}.ToString();
		}

		internal static Uri ResolveActualClusterUri(Uri dataSourceUri, string asAzureServerName, AadTokenHolder aadTokenHolder, ref TimeoutUtils.TimeLeft timeLeft, TimeoutUtils.OnTimoutAction timeoutAction)
		{
			Uri uri = ASAzureUtility.ClustersCache.GetCluster(dataSourceUri, asAzureServerName);
			if (uri != null)
			{
				return uri;
			}
			try
			{
				using (new TimeoutUtils.TimeRestrictedMonitor(timeLeft, timeoutAction))
				{
					Uri uri2 = new UriBuilder(dataSourceUri)
					{
						Path = "/webapi/clusterResolve"
					}.Uri;
					ASAzureUtility.NameResolutionRequest requestObject = new ASAzureUtility.NameResolutionRequest
					{
						ServerName = asAzureServerName
					};
					ASAzureUtility.NameResolutionResult nameResolutionResult = ASAzureUtility.PostHttpJsonData<ASAzureUtility.NameResolutionRequest, ASAzureUtility.NameResolutionResult>(uri2, requestObject, aadTokenHolder.GetValidAccessToken(), timeLeft.TimeMs);
					uri = new UriBuilder(dataSourceUri)
					{
						Host = nameResolutionResult.ClusterFqdn
					}.Uri;
					ASAzureUtility.ClustersCache.AddCluster(dataSourceUri, asAzureServerName, uri);
				}
			}
			catch (WebException ex)
			{
				ASAzureUtility.ThrowConnectionException(ex);
			}
			return uri;
		}

		internal static string GetExtendedErrorInfo(HttpWebResponse httpResponse)
		{
			string text = httpResponse.Headers["x-ms-root-activity-id"];
			string text2 = httpResponse.Headers["x-ms-current-utc-date"];
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
			{
				return string.Empty;
			}
			return XmlaSR.HttpStream_ASAzure_TechnicalDetailsText(text, text2);
		}

		internal static void ThrowConnectionException(WebException ex)
		{
			string text = ex.Message;
			if (ex.Response != null && ex.Response is HttpWebResponse)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				string text2 = httpWebResponse.Headers["x-ms-xmlaerror-extended"];
				if (!string.IsNullOrEmpty(text2))
				{
					text = text2;
				}
				text = string.Format("{0}{1}", text, ASAzureUtility.GetExtendedErrorInfo(httpWebResponse));
			}
			throw new AdomdConnectionException(text, ex);
		}

		internal static TResult PostHttpJsonData<TRequest, TResult>(Uri uri, TRequest requestObject, string bearerAuthToken, int timeoutMs)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = timeoutMs;
			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TRequest));
				dataContractJsonSerializer.WriteObject(requestStream, requestObject);
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			TResult result;
			using (Stream responseStream = httpWebResponse.GetResponseStream())
			{
				DataContractJsonSerializer dataContractJsonSerializer2 = new DataContractJsonSerializer(typeof(TResult));
				result = (TResult)((object)dataContractJsonSerializer2.ReadObject(responseStream));
			}
			return result;
		}

		private static Stream GenerateStreamFromString(string str)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(str));
		}

		internal static object GetAppDomainLock(string name)
		{
			object data;
			lock (ASAzureUtility.getAppDomainLockObj)
			{
				if (AppDomain.CurrentDomain.GetData(name) == null)
				{
					AppDomain.CurrentDomain.SetData(name, new object());
				}
				data = AppDomain.CurrentDomain.GetData(name);
			}
			return data;
		}

		internal static string GetGeneralInfoHeaderValue(bool UseAdalCache, string UserID)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Process currentProcess = Process.GetCurrentProcess();
			ProcessModule mainModule = currentProcess.MainModule;
			string moduleName = mainModule.ModuleName;
			stringBuilder.Append("AppName=").Append(moduleName);
			stringBuilder.Append(",AppVer=").Append(mainModule.FileVersionInfo.FileVersion);
			uint value = CheckBinaryType.Is64BitProcess ? 6u : 0u;
			stringBuilder.Append(",AppBinaryType=").Append(value);
			AssemblyName name = Assembly.GetExecutingAssembly().GetName();
			Version arg_7B_0 = name.Version;
			stringBuilder.Append(",ManagedVer=").Append(name.Name).Append(".").Append(name.Version);
			int value2 = UseAdalCache ? 1 : 0;
			stringBuilder.Append(",CacheUsed=").Append(value2);
			int value3 = (UserID == null) ? 0 : 1;
			stringBuilder.Append(",ConStrId=").Append(value3);
			return stringBuilder.ToString();
		}
	}
}
