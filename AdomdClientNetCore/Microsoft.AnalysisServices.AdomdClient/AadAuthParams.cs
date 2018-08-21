using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[DataContract(Name = "AuthenticationInformation", Namespace = "")]
	internal class AadAuthParams
	{
		private const string LocalSecurityConfigName = "ASAzureSecurityConfig.xml";

		private const string RemoteSecurityConfigUrl = "https://global.asazure.windows.net/ASAzureSecurityConfig.xml";

		private const string DefaultApplicationId = "cf710c6e-dfcc-4fa8-a093-d47294e44c66";

		private const string CommonTenant = "common";

		private string authority;

		private bool isCommonTenant;

		private static AadAuthParams[] _localSecurityAuthParams;

		private static AadAuthParams[] _remoteSecurityAuthParams;

		[DataMember(Name = "DomainPostfix", Order = 0)]
		public string DomainPostfix
		{
			get;
			set;
		}

		[DataMember(Name = "Authority", Order = 1)]
		public string Authority
		{
			get
			{
				return this.authority;
			}
			set
			{
				this.authority = value;
				AadAuthParams.ValidateAuthority(this.authority);
				string a = AadAuthParams.ExtractTenant(this.authority);
				this.isCommonTenant = string.Equals(a, "common", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		[DataMember(Name = "ApplicationId", Order = 2)]
		public string ApplicationId
		{
			get;
			set;
		}

		internal bool IsCommonTenant
		{
			get
			{
				return this.isCommonTenant;
			}
		}

		private static AadAuthParams[] LocalSecurityAuthParams
		{
			get
			{
				AadAuthParams[] arg_14_0;
				if ((arg_14_0 = AadAuthParams._localSecurityAuthParams) == null)
				{
					arg_14_0 = (AadAuthParams._localSecurityAuthParams = AadAuthParams.ReadLocalSecurityAuthParams());
				}
				return arg_14_0;
			}
		}

		private static AadAuthParams[] RemoteSecurityAuthParams
		{
			get
			{
				AadAuthParams[] arg_14_0;
				if ((arg_14_0 = AadAuthParams._remoteSecurityAuthParams) == null)
				{
					arg_14_0 = (AadAuthParams._remoteSecurityAuthParams = AadAuthParams.ReadRemoteSecurityAuthParams());
				}
				return arg_14_0;
			}
		}

		internal static bool IsSameInstance(string authority1, string authority2)
		{
			string a = new UriBuilder(authority1)
			{
				Path = "/"
			}.ToString();
			string b = new UriBuilder(authority2)
			{
				Path = "/"
			}.ToString();
			return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}

		public static AadAuthParams FindMatchingAuthParams(string identityProvider, Uri dataSourceUri)
		{
			AadAuthParams aadAuthParams = AadAuthParams.ExtractAuthParamsFromIdentityProvider(identityProvider);
			if (aadAuthParams != null)
			{
				return aadAuthParams;
			}
			aadAuthParams = AadAuthParams.FindAuthParams(dataSourceUri, AadAuthParams.LocalSecurityAuthParams);
			if (aadAuthParams != null)
			{
				return aadAuthParams;
			}
			aadAuthParams = AadAuthParams.FindAuthParams(dataSourceUri, AadAuthParams.RemoteSecurityAuthParams);
			if (aadAuthParams != null)
			{
				return aadAuthParams;
			}
			throw new ArgumentException(XmlaSR.Authentication_ClaimsToken_AuthorityNotFound);
		}

		private static AadAuthParams ExtractAuthParamsFromIdentityProvider(string identityProvider)
		{
			if (string.IsNullOrEmpty(identityProvider))
			{
				return null;
			}
			AadAuthParams aadAuthParams = new AadAuthParams();
			if (identityProvider.Contains(","))
			{
				string[] array = identityProvider.Split(new char[]
				{
					','
				});
				aadAuthParams.Authority = array[0];
				aadAuthParams.ApplicationId = array[1];
				if (string.IsNullOrEmpty(aadAuthParams.ApplicationId))
				{
					aadAuthParams.ApplicationId = "cf710c6e-dfcc-4fa8-a093-d47294e44c66";
				}
			}
			else
			{
				aadAuthParams.Authority = identityProvider;
				aadAuthParams.ApplicationId = "cf710c6e-dfcc-4fa8-a093-d47294e44c66";
			}
			return aadAuthParams;
		}

		private static AadAuthParams FindAuthParams(Uri uri, AadAuthParams[] authParams)
		{
			string host = uri.Host;
			AadAuthParams aadAuthParams = null;
			for (int i = 0; i < authParams.Length; i++)
			{
				AadAuthParams aadAuthParams2 = authParams[i];
				if (host.EndsWith(aadAuthParams2.DomainPostfix, StringComparison.InvariantCultureIgnoreCase) && (aadAuthParams == null || aadAuthParams2.DomainPostfix.Length > aadAuthParams.DomainPostfix.Length))
				{
					aadAuthParams = aadAuthParams2;
				}
			}
			return aadAuthParams;
		}

		private static AadAuthParams[] ReadLocalSecurityAuthParams()
		{
			AadAuthParams[] result;
			using (Stream resourceAsStream = AadAuthParams.GetResourceAsStream("ASAzureSecurityConfig.xml"))
			{
				result = AadAuthParams.DeserializeAuthParams(resourceAsStream);
			}
			return result;
		}

		private static AadAuthParams[] ReadRemoteSecurityAuthParams()
		{
			AadAuthParams[] result;
			using (WebClient webClient = new WebClient())
			{
				try
				{
					byte[] buffer = webClient.DownloadData("https://global.asazure.windows.net/ASAzureSecurityConfig.xml");
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						result = AadAuthParams.DeserializeAuthParams(memoryStream);
					}
				}
				catch (WebException)
				{
					result = new AadAuthParams[0];
				}
			}
			return result;
		}

		private static AadAuthParams[] DeserializeAuthParams(Stream stream)
		{
			AadAuthParams[] result;
			using (XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(AadAuthParams[]), "AuthenticationInformations", string.Empty);
				result = (AadAuthParams[])dataContractSerializer.ReadObject(xmlDictionaryReader, true);
			}
			return result;
		}

		private static void ValidateAuthority(string authority)
		{
			Uri uri;
			if (!AadAuthParams.IsValidUrl(authority, out uri) || string.IsNullOrEmpty(uri.AbsolutePath) || !uri.AbsolutePath.StartsWith("/") || uri.AbsolutePath.Length == 1)
			{
				throw new ArgumentException(XmlaSR.Authentication_ClaimsToken_IdentityProviderFormatInvalid);
			}
		}

		private static string ExtractTenant(string authority)
		{
			string text = new Uri(authority).AbsolutePath.Substring(1);
			int num = text.IndexOf("/");
			if (num != -1)
			{
				return text.Substring(0, num);
			}
			return text;
		}

		private static bool IsValidUrl(string address, out Uri uri)
		{
			return Uri.TryCreate(address, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
		}

		private static Stream GetResourceAsStream(string resourceName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			string name = manifestResourceNames.FirstOrDefault((string rn) => rn.EndsWith(resourceName));
			return executingAssembly.GetManifestResourceStream(name);
		}
	}
}
