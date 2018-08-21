using Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
//using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Adomd;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class ConnectionInfo
	{
		private class XmlaPropsKnownByIXMLA
		{
			private Dictionary<string, int> hash;

			private static string[] knownWritableProperties = new string[]
			{
				"Content",
				"Format",
				"AxisFormat",
				"BeginRange",
				"EndRange",
				"Cube",
				"DataSourceInfo",
				"Timeout",
				"UserName",
				"Password",
				"LocaleIdentifier",
				"CachePolicy",
				"CompareCaseSensitiveStringFlags",
				"CompareCaseNotSensitiveStringFlags",
				"ReadOnlySession",
				"SecuredCellValue",
				"MDXCompatibility",
				"MDXUniqueNameStyle",
				"DbpropMsmdMDXCompatibility",
				"DbpropMsmdMDXUniqueNameStyle",
				"CacheRatio",
				"NonEmptyThreshold",
				"Roles",
				"Catalog"
			};

			internal Dictionary<string, int> Known
			{
				get
				{
					return this.hash;
				}
			}

			internal XmlaPropsKnownByIXMLA()
			{
				this.hash = new Dictionary<string, int>(ConnectionInfo.XmlaPropsKnownByIXMLA.knownWritableProperties.Length, StringComparer.OrdinalIgnoreCase);
				string[] array = ConnectionInfo.XmlaPropsKnownByIXMLA.knownWritableProperties;
				for (int i = 0; i < array.Length; i++)
				{
					string key = array[i];
					this.hash[key] = 1;
				}
			}
		}

		private delegate void KeyValueCallback(string key, string value);

		private class RestrictedConnectionStringBuilder
		{
			private string original;

			private StringBuilder builder;

			internal RestrictedConnectionStringBuilder(string connectionString)
			{
				this.original = connectionString;
				this.builder = new StringBuilder(connectionString.Length);
			}

			internal string GetRestricted()
			{
				ConnectionInfo.ParseStringKeyValue(this.original, new ConnectionInfo.KeyValueCallback(this.HandleKeyValue));
				return this.builder.ToString();
			}

			private void HandleKeyValue(string key, string value)
			{
				if (ConnectionInfo.IsPassword(key))
				{
					return;
				}
				if (ConnectionInfo.IsExtendedProperties(key))
				{
					ConnectionInfo.RestrictedConnectionStringBuilder restrictedConnectionStringBuilder = new ConnectionInfo.RestrictedConnectionStringBuilder(value);
					string restricted = restrictedConnectionStringBuilder.GetRestricted();
					if (!string.IsNullOrEmpty(restricted))
					{
						this.builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}';", new object[]
						{
							key,
							restricted.Replace("'", "''")
						});
						return;
					}
				}
				else if (!string.IsNullOrEmpty(value))
				{
					this.builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}';", new object[]
					{
						key,
						value.Replace("'", "''")
					});
				}
			}
		}

		private const string LogicalActivityContextClient = "AnalysisServices.ClientActivityId";

		private const string LogicalActivityContextCurrent = "AnalysisServices.CurrentActivityId";

		private const string HttpPrefix = "http://";

		private const string HttpsPrefix = "https://";

		private const string AsAzurePrefix = "asazure://";

		internal const int DefaultInstancePort = 2383;

		private const int SqlBrowserPort = 2382;

		internal const string Localhost = "localhost";

		internal const string Embedded = "$Embedded$";

		internal const int TimeoutDefault = 0;

		internal const int ConnectTimeoutDefault = 60;

		internal const uint AutoSyncPeriodDefault = 10000u;

		private const ProtectionLevel ProtectionLevelDefault = ProtectionLevel.Privacy;

		private const int CompressionLevelDefault = 0;

		private const SafetyOptions SafetyOptionDefault = SafetyOptions.Default;

		private const IntegratedSecurity IntegratedSecurityDefault = IntegratedSecurity.Unspecified;

		private const ImpersonationLevel ImpersonationLevelDefault = ImpersonationLevel.Impersonate;

		private const string SspiDefault = "Negotiate";

		private const string SspiSchannel = "Schannel";

		private const string SspiUni = "Microsoft Unified Security Protocol Provider";

		private const bool UseExistingFileDefault = false;

		private const int PacketSizeDefault = 4096;

		private const int PacketSizeMinValue = 512;

		private const int PacketSizeMaxValue = 32767;

		private const string ProviderPropertyDefaultValue = "MSOLAP.2";

		private const bool RestrictedClientDefault = false;

		private const bool PersistSecurityInfoDefault = false;

		private const int MaxCharsInLinkFile = 4096;

		private const string LinkFileExtension = ".bism";

		private const string LinkFileXmlNamespace = "http://schemas.microsoft.com/analysisservices/linkfile";

		private const string LinkFileMainElement = "ASLinkFile";

		private const string LinkFileServerElement = "Server";

		private const string LinkFileDatabaseElement = "Database";

		private const string LinkFileDelegationAttribute = "allowDelegation";

		private const string LinkFileSchema = "<?xml version='1.0' encoding='utf-8'?> \r\n<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://schemas.microsoft.com/analysisservices/linkfile' elementFormDefault='qualified'>\r\n\t<xs:element name='ASLinkFile'>\r\n\t\t\t\t\t<xs:complexType>\r\n\t\t\t\t\t\t<xs:all>\r\n\t\t\t\t\t\t    <xs:element name='Server' type='xs:string'/> \r\n\t\t\t\t\t\t    <xs:element name='Database' type='xs:string' />\r\n\t\t\t\t\t\t    <xs:element name='Description' type='xs:string' minOccurs='0'/>\r\n\t\t\t\t\t\t</xs:all>\r\n                        <xs:attribute name='allowDelegation' type='xs:boolean' default='false'/>\r\n\t\t\t\t\t</xs:complexType>\r\n\t</xs:element>\r\n</xs:schema>";

		private const char EQUAL_SIGN = '=';

		private const char SEMI_COLON = ';';

		private const char SINGLE_QUOTE = '\'';

		private const char DOUBLE_QUOTE = '"';

		private const char SPACE = ' ';

		private const char TAB = '\t';

		private const string TimeoutPropertyName = "Timeout";

		private const string DataSourcePropertyName = "Data Source";

		private const string UserIDPropertyName = "User ID";

		private const string PasswordPropertyName = "Password";

		private const string ProtectionLevelPropertyName = "Protection Level";

		private const string ConnectTimeoutPropertyName = "Connect Timeout";

		private const string AutoSyncPeriodPropertyName = "Auto Synch Period";

		private const string ProviderPropertyName = "Provider";

		private const string DataSourceInfoPropertyName = "DataSourceInfo";

		private const string CatalogPropertyName = "Catalog";

		private const string IntegratedSecurityPropertyName = "Integrated Security";

		private const string ConnectToPropertyName = "ConnectTo";

		private const string SafetyOptionsPropertyName = "Safety Options";

		private const string ProtocolFormatPropertyName = "Protocol Format";

		private const string TransportCompressionPropertyName = "Transport Compression";

		private const string CompressionLevelPropertyName = "Compression Level";

		private const string EncryptionPasswordPropertyName = "Encryption Password";

		private const string ImpersonationLevelPropertyName = "Impersonation Level";

		private const string SspiPropertyName = "SSPI";

		private const string UseExistingFilePropertyName = "UseExistingFile";

		private const string CharacterEncodingPropertyName = "Character Encoding";

		private const string UseEncryptionForDataPropertyName = "Use Encryption for Data";

		private const string PacketSizePropertyName = "Packet Size";

		private const string ExtendedPropertiesPropertyName = "Extended Properties";

		private const string LcidPropertyName = "LocaleIdentifier";

		private const string LocationPropertyName = "Location";

		private const string RestrictedClientPropertyName = "Restricted Client";

		private const string PersistSecurityInfoName = "Persist Security Info";

		private const string SessionIDPropertyName = "SessionID";

		private const string ClientProcessIDPropertyName = "ClientProcessID";

		private const string ApplicationNameXmlaPropertyName = "SspropInitAppName";

		private const string ClientCertificateThumbprintPropertyName = "Client Certificate Thumbprint";

		private const string UserIdentityPropertyName = "User Identity";

		private const string DataSourceVersionPropertyName = "Data Source Version";

		private const string CertificatePropertyName = "Certificate";

		private const string ExternalTenantIdPropertyName = "External Tenant Id";

		private const string ExternalUserIdPropertyName = "External User Id";

		private const string ExternalServiceDomainNamePropertyName = "External Service Domain Name";

		private const string ExternalCertificateThumbprintPropertyName = "External Certificate Thumbprint";

		private const string AuthenticationSchemePropertyName = "Authentication Scheme";

		private const string ExtAuthInfoPropertyName = "Ext Auth Info";

		private const string IdentityProviderPropertyName = "Identity Provider";

		private const string BypassAuthorizationPropertyName = "Bypass Authorization";

		private const string RestrictCatalogPropertyName = "Restrict Catalog";

		private const string AccessModePropertyName = "Access Mode";

		private const string UseAdalCachePropertyName = "UseADALCache";

		private const string IsInternalASAzurePropertyName = "IsInternalASAzure";

		internal const string EffectiveUserNamePropertyName = "EffectiveUserName";

		private const string ProtectionLevelNone = "NONE";

		private const string ProtectionLevelConnect = "CONNECT";

		private const string ProtectionLevelIntegrity = "PKT INTEGRITY";

		private const string ProtectionLevelPrivacy = "PKT PRIVACY";

		private const string IntegratedSecuritySspi = "SSPI";

		private const string IntegratedSecurityBasic = "Basic";

		private const string IntegratedSecurityFederated = "Federated";

		private const string IntegratedSecurityClaimsToken = "ClaimsToken";

		private const string IdentityProviderMsoID = "MsoID";

		private const string UserIdentityDefault = "DEFAULT";

		private const string UserIdentityWindows = "Windows Identity";

		private const string UserIdentitySharePointPrincipal = "SharePoint Principal";

		private const string ConnectToDefault = "DEFAULT";

		private const string ConnectToShiloh = "8.0";

		private const string ConnectToYukon = "9.0";

		private const string ConnectToKatmai = "10.0";

		private const string ConnectToDenali = "11.0";

		private const string ConnectToSQL14 = "12.0";

		private const string ConnectToSQL15 = "13.0";

		private const string AutoSyncPeriodNull = "NULL";

		private const string ProtocolFormatDefault = "Default";

		private const string ProtocolFormatXml = "XML";

		private const string ProtocolFormatBinary = "Binary";

		private const string TransportCompressionDefault = "Default";

		private const string TransportCompressionNone = "None";

		private const string TransportCompressionCompressed = "Compressed";

		private const string TransportCompressionGzip = "Gzip";

		private const string ImpersonationLevelAnonymous = "Anonymous";

		private const string ImpersonationLevelIdentify = "Identify";

		private const string ImpersonationLevelImpersonate = "Impersonate";

		private const string ImpersonationLevelDelegate = "Delegate";

		private const string CharacterEncodingDefault = "Default";

		private static MDXMLAPropInfo PropInfo = new MDXMLAPropInfo();

		private string connectionString;

		private ConnectionType connectionType;

		private bool isOnPremFromCloudAccess;

		private AsInstanceType asInstanceType;

		private string dataSource;

		private string location;

		private string server;

		private string instanceName;

		private string port;

		private string userID;

		private string password;

		private AadTokenHolder aadTokenHolder;

		private int timeout;

		private int connectTimeout;

		private uint autoSyncPeriod;

		private string catalog;

		private ProtectionLevel protectionLevel;

		private bool protectionLevelWasSet;

		private ConnectTo connectTo;

		private SafetyOptions safetyOptions;

		private bool restrictedClient;

		private ProtocolFormat protocolFormat;

		private TransportCompression transportCompression;

		private int compressionLevel;

		private IntegratedSecurity integratedSecurity;

		private string encryptionPassword;

		private ImpersonationLevel impersonationLevel;

		private string sspi;

		private bool useExistingFile;

		private Encoding characterEncoding;

		private int packetSize;

		private string useEncryptionForData;

		private string innerConnectionString;

		private ListDictionary extendedProperties = new ListDictionary();

		private bool isForSqlBrowser;

		private bool persistSecurityInfo;

		private bool restrictConnectionString;

		private string restrictedConnectionString;

		private bool pwdPresent;

		private string sessionID;

		private bool revertToProcessAccountForConnection;

		private bool allowDelegation;

		private bool useEU;

		private string clientCertificateThumbprint;

		private bool isLightweightConnection;

		private Guid autoGeneratedActivityID = Guid.NewGuid();

		private UserIdentityType userIdentity;

		private string excelSessionId;

		private string certificate;

		private string authenticationScheme;

		private string extAuthInfo;

		private string identityProvider;

		private string bypassAuthorization;

		private string restrictCatalog;

		private string accessMode;

		private string externalTenantId;

		private string externalUserId;

		private string externalServiceDomainName;

		private string externalCertificateThumbprint;

		private ListDictionary ixmlaProperties;

		private ListDictionary originalConnStringProps = new ListDictionary();

		private bool iXMLAMode;

		private string dataSourceVersion;

		private string redirectorAddress;

		private string sandboxPath;

		private bool allowAutoRedirect = true;

		private bool allowPrompt;

		private bool useAdalCache = true;

		private bool isInternalASAzure;

		private string asAzureServerName;

		private static string[] DatasourcePropertyNames = new string[]
		{
			"Data Source",
			"DataSource",
			"DSN"
		};

		private static string[] UserIDPropertyNames = new string[]
		{
			"User ID",
			"UID"
		};

		private static string[] PasswordPropertyNames = new string[]
		{
			"Password",
			"PWD"
		};

		private static string[] CatalogPropertyNames = new string[]
		{
			"Initial Catalog",
			"Catalog",
			"Database"
		};

		private static string[] PropertyNamesToIgnore = new string[]
		{
			"Provider",
			"ConnectTo",
			"DataSourceInfo"
		};

		private static readonly ConnectionInfo.XmlaPropsKnownByIXMLA XMLAPropertiesKnownByIXMLA = new ConnectionInfo.XmlaPropsKnownByIXMLA();

		internal string ConnectionString
		{
			get
			{
				if (this.restrictConnectionString && !this.persistSecurityInfo && this.pwdPresent)
				{
					if (string.IsNullOrEmpty(this.restrictedConnectionString))
					{
						ConnectionInfo.RestrictedConnectionStringBuilder restrictedConnectionStringBuilder = new ConnectionInfo.RestrictedConnectionStringBuilder(this.connectionString);
						this.restrictedConnectionString = restrictedConnectionStringBuilder.GetRestricted();
					}
					return this.restrictedConnectionString;
				}
				return this.connectionString;
			}
			set
			{
				this.SetConnectionString(value);
			}
		}

		internal bool RestrictConnectionString
		{
			set
			{
				this.restrictConnectionString = value;
			}
		}

		internal bool IsForRedirector
		{
			get
			{
				return this.redirectorAddress != null;
			}
		}

		internal bool IsEmbedded
		{
			get
			{
				return string.Compare(this.server, "$Embedded$", StringComparison.OrdinalIgnoreCase) == 0;
			}
		}

		internal bool IsForSqlBrowser
		{
			get
			{
				return this.isForSqlBrowser;
			}
		}

		public ConnectionType ConnectionType
		{
			get
			{
				return this.connectionType;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
		}

		public string Port
		{
			get
			{
				return this.port;
			}
		}

		public string Location
		{
			get
			{
				return this.location;
			}
		}

		public int Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		public int ConnectTimeout
		{
			get
			{
				return this.connectTimeout;
			}
			set
			{
				this.connectTimeout = value;
			}
		}

		public string Catalog
		{
			get
			{
				return this.catalog;
			}
		}

		internal bool RevertToProcessAccountForConnection
		{
			get
			{
				return this.revertToProcessAccountForConnection;
			}
			set
			{
				this.revertToProcessAccountForConnection = value;
			}
		}

		internal bool AllowDelegation
		{
			get
			{
				return this.allowDelegation;
			}
			set
			{
				this.allowDelegation = value;
			}
		}

		internal bool UseEU
		{
			get
			{
				return this.useEU;
			}
			set
			{
				this.useEU = value;
			}
		}

		internal bool IsLightweightConnection
		{
			get
			{
				return this.isLightweightConnection;
			}
			set
			{
				this.isLightweightConnection = value;
			}
		}

		public bool AllowPrompt
		{
			get
			{
				return this.allowPrompt;
			}
		}

		public string UserID
		{
			get
			{
				return this.userID;
			}
		}

		public string Password
		{
			get
			{
				return this.password;
			}
		}

		internal AadTokenHolder AadTokenHolder
		{
			get
			{
				return this.aadTokenHolder;
			}
			set
			{
				this.aadTokenHolder = value;
			}
		}

		public ProtectionLevel ProtectionLevel
		{
			get
			{
				return this.protectionLevel;
			}
		}

		public ProtocolFormat ProtocolFormat
		{
			get
			{
				return this.protocolFormat;
			}
		}

		public TransportCompression TransportCompression
		{
			get
			{
				return this.transportCompression;
			}
		}

		public int CompressionLevel
		{
			get
			{
				return this.compressionLevel;
			}
		}

		public IntegratedSecurity IntegratedSecurity
		{
			get
			{
				return this.integratedSecurity;
			}
		}

		internal uint AutoSyncPeriod
		{
			get
			{
				return this.autoSyncPeriod;
			}
		}

		internal ConnectTo ConnectTo
		{
			get
			{
				return this.connectTo;
			}
		}

		internal SafetyOptions SafetyOptions
		{
			get
			{
				if (this.safetyOptions != SafetyOptions.Default)
				{
					return this.safetyOptions;
				}
				return SafetyOptions.Safe;
			}
		}

		internal string RoutingToken
		{
			get;
			set;
		}

		internal string SessionID
		{
			get
			{
				return this.sessionID;
			}
		}

		public string EncryptionPassword
		{
			get
			{
				return this.encryptionPassword;
			}
		}

		public ImpersonationLevel ImpersonationLevel
		{
			get
			{
				return this.impersonationLevel;
			}
		}

		internal bool RestrictedClient
		{
			get
			{
				return this.restrictedClient;
			}
		}

		internal bool IsOnPremFromCloudAccess
		{
			get
			{
				return this.isOnPremFromCloudAccess;
			}
		}

		public string Sspi
		{
			get
			{
				return this.sspi;
			}
		}

		public bool UseExistingFile
		{
			get
			{
				return this.useExistingFile;
			}
		}

		public Encoding CharacterEncoding
		{
			get
			{
				return this.characterEncoding;
			}
		}

		public int PacketSize
		{
			get
			{
				return this.packetSize;
			}
		}

		internal string ApplicationName
		{
			get
			{
				return (string)this.ExtendedProperties["SspropInitAppName"];
			}
		}

		public string ClientCertificateThumbprint
		{
			get
			{
				return this.clientCertificateThumbprint;
			}
		}

		internal Guid ClientActivityID
		{
			get
			{
				Guid clientActivityIDInThreadContext = this.ClientActivityIDInThreadContext;
				if (clientActivityIDInThreadContext != Guid.Empty)
				{
					return clientActivityIDInThreadContext;
				}
				return this.CurrentActivityID;
			}
		}

		internal Guid CurrentActivityID
		{
			get
			{
				Guid currentActivityIDInThreadContext = this.CurrentActivityIDInThreadContext;
				if (currentActivityIDInThreadContext != Guid.Empty)
				{
					return currentActivityIDInThreadContext;
				}
				return this.ActivityIDInThreadContext;
			}
		}

		public string Certificate
		{
			get
			{
				return this.certificate;
			}
		}

		public string AuthenticationScheme
		{
			get
			{
				return this.authenticationScheme;
			}
		}

		public string ExtAuthInfo
		{
			get
			{
				return this.extAuthInfo;
			}
		}

		public string IdentityProvider
		{
			get
			{
				return this.identityProvider;
			}
		}

		public string BypassAuthorization
		{
			get
			{
				return this.bypassAuthorization;
			}
		}

		public string RestrictCatalog
		{
			get
			{
				return this.restrictCatalog;
			}
		}

		public string AccessMode
		{
			get
			{
				return this.accessMode;
			}
		}

		public ListDictionary ExtendedProperties
		{
			get
			{
				if (this.iXMLAMode)
				{
					return this.IXMLAProperties;
				}
				return this.extendedProperties;
			}
		}

		internal string DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		internal string DataSourceVersion
		{
			get
			{
				return this.dataSourceVersion;
			}
			set
			{
				this.dataSourceVersion = value;
			}
		}

		internal bool AllowAutoRedirect
		{
			get
			{
				return this.allowAutoRedirect;
			}
			set
			{
				this.allowAutoRedirect = value;
			}
		}

		internal string AsAzureServerName
		{
			get
			{
				return this.asAzureServerName;
			}
			set
			{
				this.asAzureServerName = value;
			}
		}

		internal bool UseAdalCache
		{
			get
			{
				return this.useAdalCache;
			}
			set
			{
				this.useAdalCache = value;
			}
		}

		internal bool IsInternalASAzure
		{
			get
			{
				return this.isInternalASAzure;
			}
			set
			{
				this.isInternalASAzure = value;
			}
		}

		internal UserIdentityType UserIdentity
		{
			get
			{
				return this.userIdentity;
			}
		}

		internal string ExcelSessionId
		{
			get
			{
				return this.excelSessionId;
			}
		}

		internal bool IsSspiAnonymous
		{
			get
			{
				return string.Compare(this.sspi, "Anonymous", StringComparison.OrdinalIgnoreCase) == 0;
			}
		}

		internal bool IsServerLocal
		{
			get
			{
				return this.server == ".";
			}
		}

		internal bool IsAsAzure
		{
			get
			{
				return this.asInstanceType == AsInstanceType.AsAzure;
			}
			set
			{
				this.asInstanceType = (value ? AsInstanceType.AsAzure : AsInstanceType.Other);
			}
		}

		internal bool IXMLAMode
		{
			get
			{
				return this.iXMLAMode;
			}
			set
			{
				this.iXMLAMode = value;
			}
		}

		private ListDictionary IXMLAProperties
		{
			get
			{
				if (this.ixmlaProperties == null)
				{
					this.FillIXMLAProperties();
				}
				return this.ixmlaProperties;
			}
		}

		private Guid ClientActivityIDInThreadContext
		{
			get
			{
				try
				{
					object obj = CallContext.LogicalGetData("AnalysisServices.ClientActivityId");
					if (obj != null)
					{
						return (Guid)obj;
					}
				}
				catch
				{
				}
				return Guid.Empty;
			}
		}

		private Guid CurrentActivityIDInThreadContext
		{
			get
			{
				try
				{
					object obj = CallContext.LogicalGetData("AnalysisServices.CurrentActivityId");
					if (obj != null)
					{
						return (Guid)obj;
					}
				}
				catch
				{
				}
				return Guid.Empty;
			}
		}

		private Guid ActivityIDInThreadContext
		{
			get
			{
				if (Trace.CorrelationManager.ActivityId != Guid.Empty)
				{
					return Trace.CorrelationManager.ActivityId;
				}
				return this.autoGeneratedActivityID;
			}
		}

		private static string Trim(string str)
		{
			if (str == null)
			{
				return null;
			}
			str = str.Trim();
			if (str.Length == 0)
			{
				return null;
			}
			return str;
		}

		internal ConnectionInfo()
		{
			this.ResetFields();
		}

		public ConnectionInfo(string connectionString)
		{
			this.SetConnectionString(connectionString);
		}

		internal ConnectionInfo(ConnectionInfo connectionInfo) : this(connectionInfo, false)
		{
		}

		internal ConnectionInfo(ConnectionInfo connectionInfo, bool noTimeout)
		{
			ConnectionInfo.CopyConnectionInfo(connectionInfo, this);
			if (noTimeout)
			{
				this.timeout = 0;
			}
		}

		internal static void ValidateSpecifiedEffectiveUserName(string effectiveUserName)
		{
			SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
			NTAccount nTAccount = new NTAccount(effectiveUserName);
			SecurityIdentifier sid = (SecurityIdentifier)nTAccount.Translate(typeof(SecurityIdentifier));
			if (user.CompareTo(sid) != 0)
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileDupEffectiveUsername);
			}
		}

		private static void CopyConnectionInfo(ConnectionInfo sourceInfo, ConnectionInfo destinationInfo)
		{
			destinationInfo.connectionString = sourceInfo.connectionString;
			destinationInfo.connectionType = sourceInfo.connectionType;
			destinationInfo.asInstanceType = sourceInfo.asInstanceType;
			destinationInfo.dataSource = sourceInfo.dataSource;
			destinationInfo.location = sourceInfo.location;
			destinationInfo.server = sourceInfo.server;
			destinationInfo.instanceName = sourceInfo.instanceName;
			destinationInfo.port = sourceInfo.port;
			destinationInfo.userID = sourceInfo.userID;
			destinationInfo.password = sourceInfo.password;
			destinationInfo.timeout = sourceInfo.timeout;
			destinationInfo.connectTimeout = sourceInfo.connectTimeout;
			destinationInfo.autoSyncPeriod = sourceInfo.autoSyncPeriod;
			destinationInfo.catalog = sourceInfo.catalog;
			destinationInfo.protectionLevel = sourceInfo.protectionLevel;
			destinationInfo.protectionLevelWasSet = sourceInfo.protectionLevelWasSet;
			destinationInfo.connectTo = sourceInfo.connectTo;
			destinationInfo.safetyOptions = sourceInfo.safetyOptions;
			destinationInfo.protocolFormat = sourceInfo.protocolFormat;
			destinationInfo.transportCompression = sourceInfo.transportCompression;
			destinationInfo.compressionLevel = sourceInfo.compressionLevel;
			destinationInfo.integratedSecurity = sourceInfo.integratedSecurity;
			destinationInfo.identityProvider = sourceInfo.identityProvider;
			destinationInfo.encryptionPassword = sourceInfo.encryptionPassword;
			destinationInfo.impersonationLevel = sourceInfo.ImpersonationLevel;
			destinationInfo.sspi = sourceInfo.sspi;
			destinationInfo.useExistingFile = sourceInfo.useExistingFile;
			destinationInfo.characterEncoding = sourceInfo.characterEncoding;
			destinationInfo.packetSize = sourceInfo.packetSize;
			destinationInfo.useEncryptionForData = sourceInfo.useEncryptionForData;
			destinationInfo.innerConnectionString = sourceInfo.innerConnectionString;
			destinationInfo.extendedProperties = ConnectionInfo.CloneListDictionary(sourceInfo.extendedProperties);
			destinationInfo.isForSqlBrowser = sourceInfo.isForSqlBrowser;
			destinationInfo.restrictedClient = sourceInfo.restrictedClient;
			destinationInfo.persistSecurityInfo = sourceInfo.persistSecurityInfo;
			destinationInfo.restrictedConnectionString = sourceInfo.restrictedConnectionString;
			destinationInfo.restrictConnectionString = sourceInfo.restrictConnectionString;
			destinationInfo.pwdPresent = sourceInfo.pwdPresent;
			destinationInfo.sessionID = sourceInfo.sessionID;
			destinationInfo.dataSourceVersion = sourceInfo.dataSourceVersion;
			destinationInfo.redirectorAddress = sourceInfo.redirectorAddress;
			destinationInfo.sandboxPath = sourceInfo.sandboxPath;
			destinationInfo.revertToProcessAccountForConnection = sourceInfo.revertToProcessAccountForConnection;
			destinationInfo.allowDelegation = sourceInfo.allowDelegation;
			destinationInfo.useEU = sourceInfo.useEU;
			destinationInfo.isLightweightConnection = sourceInfo.isLightweightConnection;
			destinationInfo.clientCertificateThumbprint = sourceInfo.clientCertificateThumbprint;
			destinationInfo.userIdentity = sourceInfo.userIdentity;
			destinationInfo.clientCertificateThumbprint = sourceInfo.clientCertificateThumbprint;
			destinationInfo.certificate = sourceInfo.certificate;
			destinationInfo.authenticationScheme = sourceInfo.authenticationScheme;
			destinationInfo.extAuthInfo = sourceInfo.extAuthInfo;
			destinationInfo.identityProvider = sourceInfo.identityProvider;
			destinationInfo.bypassAuthorization = sourceInfo.bypassAuthorization;
			destinationInfo.restrictCatalog = sourceInfo.restrictCatalog;
			destinationInfo.accessMode = sourceInfo.accessMode;
			destinationInfo.RoutingToken = sourceInfo.RoutingToken;
			destinationInfo.allowAutoRedirect = sourceInfo.allowAutoRedirect;
			destinationInfo.allowPrompt = sourceInfo.allowPrompt;
			destinationInfo.useAdalCache = sourceInfo.useAdalCache;
			destinationInfo.isInternalASAzure = sourceInfo.isInternalASAzure;
			destinationInfo.asAzureServerName = sourceInfo.asAzureServerName;
			destinationInfo.ixmlaProperties = ConnectionInfo.CloneListDictionary(sourceInfo.ixmlaProperties);
			destinationInfo.originalConnStringProps = ConnectionInfo.CloneListDictionary(sourceInfo.originalConnStringProps);
			destinationInfo.iXMLAMode = sourceInfo.iXMLAMode;
		}

		private static ListDictionary CloneListDictionary(ListDictionary original)
		{
			ListDictionary listDictionary = null;
			if (original != null)
			{
				listDictionary = new ListDictionary();
				foreach (object current in original.Keys)
				{
					listDictionary[current] = original[current];
				}
			}
			return listDictionary;
		}

		private void ResetFields()
		{
			this.asInstanceType = AsInstanceType.Other;
			this.dataSource = null;
			this.location = null;
			this.server = null;
			this.instanceName = null;
			this.port = null;
			this.userID = null;
			this.password = null;
			this.timeout = 0;
			this.connectTimeout = 60;
			this.autoSyncPeriod = 10000u;
			this.catalog = null;
			this.protectionLevel = ProtectionLevel.Privacy;
			this.protectionLevelWasSet = false;
			this.connectTo = ConnectTo.Default;
			this.safetyOptions = SafetyOptions.Default;
			this.protocolFormat = ProtocolFormat.Default;
			this.transportCompression = TransportCompression.Default;
			this.compressionLevel = 0;
			this.integratedSecurity = IntegratedSecurity.Unspecified;
			this.encryptionPassword = null;
			this.impersonationLevel = ImpersonationLevel.Impersonate;
			this.sspi = "Negotiate";
			this.useExistingFile = false;
			this.characterEncoding = Encoding.UTF8;
			this.packetSize = 4096;
			this.useEncryptionForData = null;
			this.innerConnectionString = null;
			this.isForSqlBrowser = false;
			this.isLightweightConnection = false;
			this.restrictedClient = false;
			this.persistSecurityInfo = false;
			this.sessionID = null;
			this.restrictedConnectionString = null;
			this.restrictConnectionString = false;
			this.pwdPresent = false;
			this.dataSourceVersion = null;
			this.redirectorAddress = null;
			this.sandboxPath = null;
			this.clientCertificateThumbprint = null;
			this.userIdentity = UserIdentityType.WindowsIdentity;
			this.certificate = null;
			this.authenticationScheme = null;
			this.extAuthInfo = null;
			this.identityProvider = null;
			this.bypassAuthorization = null;
			this.restrictCatalog = null;
			this.accessMode = null;
			this.RoutingToken = null;
			this.isOnPremFromCloudAccess = false;
			this.allowAutoRedirect = true;
			this.allowPrompt = false;
			this.useAdalCache = true;
			this.isInternalASAzure = false;
			this.asAzureServerName = null;
			this.extendedProperties.Clear();
			this.ixmlaProperties = null;
			this.originalConnStringProps.Clear();
			this.iXMLAMode = false;
		}

		private void SetConnectionString(string cs)
		{
			if (cs == null)
			{
				throw new ArgumentNullException("cs");
			}
			cs = cs.Trim();
			if (cs.Length == 0)
			{
				throw new ArgumentException(XmlaSR.ConnectionString_Invalid);
			}
			ConnectionInfo sourceInfo = new ConnectionInfo(this);
			try
			{
				this.ResetFields();
				this.connectionString = cs;
				if (!this.ParseShortcutForm())
				{
					ConnectionInfo.ParseStringKeyValue(this.connectionString, new ConnectionInfo.KeyValueCallback(this.HandleKeyValueDuringConnectionStringParsing));
					if (this.innerConnectionString != null)
					{
						string text = this.connectionString;
						do
						{
							this.connectionString = this.innerConnectionString;
							this.innerConnectionString = null;
							ConnectionInfo.ParseStringKeyValue(this.connectionString, new ConnectionInfo.KeyValueCallback(this.HandleKeyValueDuringConnectionStringParsing));
						}
						while (this.innerConnectionString != null);
						this.connectionString = text;
					}
					if (this.dataSource == null)
					{
						this.dataSource = this.location;
					}
					if (this.dataSource == null)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_DataSourceNotSpecified);
					}
					this.ExtractDataSourceParts(this.dataSource, this.location);
				}
				if (string.Compare(this.server, "(local)", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.server, "local", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.server = ".";
				}
				if (this.useEncryptionForData == bool.TrueString)
				{
					this.protectionLevel = ProtectionLevel.Privacy;
				}
				else if (!this.protectionLevelWasSet)
				{
					if (this.useEncryptionForData == bool.FalseString)
					{
						this.protectionLevel = ProtectionLevel.Connection;
					}
					else if (this.connectionType == ConnectionType.Http && !ConnectionInfo.IsHttpsAddress(this.server) && !ConnectionInfo.IsAsAzureAddress(this.server))
					{
						this.protectionLevel = ProtectionLevel.Connection;
					}
				}
				if (this.connectionType == ConnectionType.Http)
				{
					Uri uri = new Uri(this.dataSource);
					if (Uri.UriSchemeHttp.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase) || Uri.UriSchemeHttps.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
					{
						string text2 = uri.Query.Trim();
						if (text2.Length > 1 && text2[0] == '?')
						{
							text2 = text2.Remove(0, 1);
							string[] array = text2.Split(new char[]
							{
								'&',
								';'
							});
							if (array != null)
							{
								string[] array2 = array;
								for (int i = 0; i < array2.Length; i++)
								{
									string text3 = array2[i];
									string[] array3 = text3.Trim().Split(new char[]
									{
										'='
									}, StringSplitOptions.RemoveEmptyEntries);
									if (array3.Length == 2)
									{
										string value = array3[0].Trim();
										string value2 = array3[1].Trim();
										if ("integratedsecurity".Equals(value, StringComparison.OrdinalIgnoreCase) && "federated".Equals(value2, StringComparison.OrdinalIgnoreCase))
										{
											this.integratedSecurity = IntegratedSecurity.Federated;
										}
										else if ("identityprovider".Equals(value, StringComparison.OrdinalIgnoreCase))
										{
											if (!"msoid".Equals(value2, StringComparison.OrdinalIgnoreCase))
											{
												throw new ArgumentException(XmlaSR.ConnectionString_InvalidIdentityProviderForIntegratedSecurityFederated);
											}
											this.identityProvider = value2;
										}
										else if ("userid".Equals(value, StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(this.userID))
										{
											this.userID = value2;
										}
										else if ("password".Equals(value, StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(this.password))
										{
											this.password = value2;
										}
									}
								}
							}
						}
					}
				}
				if (this.connectionType == ConnectionType.Native)
				{
					if (this.integratedSecurity != IntegratedSecurity.Unspecified && this.integratedSecurity != IntegratedSecurity.Sspi)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_InvalidIntegratedSecurityForNative(this.integratedSecurity.ToString()));
					}
				}
				else if (this.connectionType == ConnectionType.Http)
				{
					if (this.integratedSecurity != IntegratedSecurity.Unspecified && this.integratedSecurity != IntegratedSecurity.Federated && this.integratedSecurity != IntegratedSecurity.ClaimsToken && (this.integratedSecurity != IntegratedSecurity.Sspi || this.userID != null || this.password != null))
					{
						throw new ArgumentException(XmlaSR.ConnectionString_InvalidIntegratedSecurityForHttpOrHttps(this.integratedSecurity.ToString()));
					}
					if (this.integratedSecurity == IntegratedSecurity.Federated && string.IsNullOrEmpty(this.identityProvider))
					{
						throw new ArgumentException(XmlaSR.ConnectionString_MissingIdentityProviderForIntegratedSecurityFederated);
					}
					this.IsAsAzure = ASAzureUtility.IsAsAzureInstance(this.server);
					if (this.IsAsAzure)
					{
						if (this.integratedSecurity != IntegratedSecurity.ClaimsToken && this.integratedSecurity != IntegratedSecurity.Unspecified)
						{
							throw new ArgumentException(XmlaSR.Authentication_AsAzure_OnlyClaimsTokenSupported);
						}
						string text4;
						if (!ASAzureUtility.DataSourceUriWithOnlyServerName(this.server, out text4))
						{
							throw new NotSupportedException(XmlaSR.ConnectionString_AsAzure_DataSourcePathMoreThanOneSegment);
						}
						this.integratedSecurity = IntegratedSecurity.ClaimsToken;
						this.server = ASAzureUtility.ConstructAsAzureSecureServerConnUri(this.server);
						this.asAzureServerName = text4;
						this.allowAutoRedirect = false;
						bool flag;
						if (ConnectionInfo.IsProcessWithUserInterface())
						{
							this.allowPrompt = true;
							flag = true;
						}
						else
						{
							this.allowPrompt = false;
							flag = false;
						}
						this.useAdalCache = (this.useAdalCache && flag);
						if (!this.allowPrompt && this.password == null)
						{
							throw new ArgumentException(XmlaSR.Authentication_ClaimsToken_UserIdAndPasswordRequired);
						}
					}
					if (ConnectionInfo.IsHttpsAddress(this.server))
					{
						switch (this.protectionLevel)
						{
						case ProtectionLevel.None:
						case ProtectionLevel.Connection:
						case ProtectionLevel.Integrity:
							throw new ArgumentException(XmlaSR.ConnectionString_InvalidProtectionLevelForHttps(this.protectionLevel.ToString()));
						default:
							if (this.impersonationLevel == ImpersonationLevel.Anonymous)
							{
								throw new ArgumentException(XmlaSR.ConnectionString_InvalidImpersonationLevelForHttps(ImpersonationLevel.Anonymous.ToString()));
							}
							break;
						}
					}
					else
					{
						switch (this.protectionLevel)
						{
						case ProtectionLevel.None:
						case ProtectionLevel.Integrity:
						case ProtectionLevel.Privacy:
							throw new ArgumentException(XmlaSR.ConnectionString_InvalidProtectionLevelForHttp(this.protectionLevel.ToString()));
						}
						if (this.impersonationLevel == ImpersonationLevel.Anonymous || this.impersonationLevel == ImpersonationLevel.Identify)
						{
							throw new ArgumentException(XmlaSR.ConnectionString_InvalidImpersonationLevelForHttp(this.impersonationLevel.ToString()));
						}
					}
				}
				if (this.redirectorAddress == null && this.dataSourceVersion != null)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_PropertyNotApplicableWithTheDataSourceType("Data Source Version"));
				}
				if (this.userIdentity == UserIdentityType.SharePointPrincipal && !this.IsRunningInO15CompatibleFarm())
				{
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("User Identity", "SharePoint Principal"));
				}
				if (!this.extendedProperties.Contains("LocaleIdentifier"))
				{
					this.extendedProperties["LocaleIdentifier"] = FormattersHelpers.ConvertToXml(CultureInfo.CurrentCulture.LCID);
					this.SetOriginalConnectionStringValue("LocaleIdentifier", FormattersHelpers.ConvertToXml(CultureInfo.CurrentCulture.LCID));
				}
				if (!this.extendedProperties.Contains("ClientProcessID") && this.extendedProperties.Contains("SspropInitAppName"))
				{
					int id = Process.GetCurrentProcess().Id;
					this.extendedProperties["ClientProcessID"] = FormattersHelpers.ConvertToXml(id);
					this.SetOriginalConnectionStringValue("ClientProcessID", FormattersHelpers.ConvertToXml(id));
				}
			}
			catch
			{
				ConnectionInfo.CopyConnectionInfo(sourceInfo, this);
				throw;
			}
		}

		private ASLinkFile GetRemoteLinkFile(string dataSource)
		{
			ASLinkFile result;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(dataSource);
				httpWebRequest.Credentials = ((this.userID == null) ? CredentialCache.DefaultCredentials : new NetworkCredential(this.userID, this.password));
				httpWebRequest.UserAgent = "ADOMD.NET";
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				ASLinkFile aSLinkFile;
				using (Stream responseStream = httpWebResponse.GetResponseStream())
				{
					aSLinkFile = ASLinkFile.LoadFromStream(responseStream, true);
				}
				result = aSLinkFile;
			}
			catch (WebException innerException)
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileDownloadError(dataSource), innerException);
			}
			catch (XmlException innerException2)
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileParseError(4096), innerException2);
			}
			catch (XmlSchemaException innerException3)
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileParseError(4096), innerException3);
			}
			return result;
		}

		internal bool SupportsSharePointAuth()
		{
			return this.connectionType == ConnectionType.LocalFarm || (this.IsLinkFile() && ConnectionInfo.IsInO15CompatibleFarm(this.server));
		}

		internal bool IsLinkFile()
		{
			bool result = false;
			if (ConnectionInfo.IsHttpAddress(this.server))
			{
				Uri uri = new Uri(this.server);
				if (uri.AbsolutePath.EndsWith(".bism", StringComparison.OrdinalIgnoreCase))
				{
					result = true;
				}
			}
			return result;
		}

		internal void ResolveLinkFileDataSource()
		{
			if (this.IsLinkFile())
			{
				this.AllowDelegation = false;
				this.revertToProcessAccountForConnection = false;
				ASLinkFile aSLinkFile = this.GetLocalLinkFile(this.server);
				if (aSLinkFile.IsFileMalformed)
				{
					throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileParseError(4096));
				}
				if (!aSLinkFile.IsInFarm)
				{
					aSLinkFile = this.GetRemoteLinkFile(this.server);
				}
				this.allowDelegation = aSLinkFile.IsDelegationAllowed;
				if (!string.IsNullOrEmpty(aSLinkFile.Server) && ConnectionInfo.IsHttpAddress(aSLinkFile.Server))
				{
					Uri uri = new Uri(aSLinkFile.Server);
					if (uri.AbsolutePath.EndsWith(".bism", StringComparison.OrdinalIgnoreCase))
					{
						throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileInvalidServer);
					}
				}
				if (!string.IsNullOrEmpty(aSLinkFile.Database))
				{
					this.SetCatalog(aSLinkFile.Database);
				}
				this.ExtractDataSourceParts(aSLinkFile.Server, null);
			}
		}

		internal void TryAddEffectiveUserName()
		{
			if (this.AllowDelegation && this.ConnectionType != ConnectionType.Wcf)
			{
				if (!this.ExtendedProperties.Contains("EffectiveUserName"))
				{
					this.ExtendedProperties.Add("EffectiveUserName", WindowsIdentity.GetCurrent().Name);
					this.revertToProcessAccountForConnection = true;
					return;
				}
				ConnectionInfo.ValidateSpecifiedEffectiveUserName(this.ExtendedProperties["EffectiveUserName"].ToString());
				this.revertToProcessAccountForConnection = true;
			}
		}

		private void XmlWarningValidationCallback(object sender, ValidationEventArgs args)
		{
			throw args.Exception;
		}

		private static void ExtractDataSourceStringParts(string dataSource, out string theServer, out string thePort, out string theInstance)
		{
			theServer = string.Empty;
			thePort = string.Empty;
			theInstance = string.Empty;
			string[] array = dataSource.Split(new char[]
			{
				'\\'
			});
			if (array.Length > 2)
			{
				throw new ArgumentException(XmlaSR.ConnectionString_Invalid);
			}
			if (array.Length == 2)
			{
				theInstance = array[1].Trim();
				if (theInstance.Length == 0)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_Invalid);
				}
			}
			dataSource = array[0].Trim();
			array = dataSource.Split(new char[]
			{
				':'
			});
			if (array.Length == 2 || array.Length == 9)
			{
				thePort = array[array.Length - 1].Trim();
				if (string.IsNullOrEmpty(thePort))
				{
					throw new ArgumentException(XmlaSR.ConnectionString_Invalid);
				}
				int length = dataSource.LastIndexOf(':');
				theServer = dataSource.Substring(0, length).Trim();
				return;
			}
			else
			{
				if (array.Length > 9)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_Invalid);
				}
				thePort = string.Empty;
				theServer = dataSource;
				return;
			}
		}

		internal void ExtractDataSourceParts(string dataSource, string location)
		{
			string arg_05_0 = string.Empty;
			if (dataSource == "*")
			{
				this.SetServer("*");
				this.connectionType = ConnectionType.LocalServer;
				return;
			}
			if (this.IsEmbeddedPath(dataSource))
			{
				if (string.IsNullOrEmpty(this.Location))
				{
					throw new ArgumentException(XmlaSR.ConnectionString_DataSourceNotSpecified);
				}
				this.SetServer("$Embedded$");
				this.connectionType = ConnectionType.LocalCube;
				return;
			}
			else
			{
				if (this.IsLocalCubeFile(dataSource))
				{
					this.SetServer(dataSource);
					this.connectionType = ConnectionType.LocalCube;
					return;
				}
				if (this.isOnPremFromCloudAccess)
				{
					this.SetServer(dataSource);
					this.connectionType = ConnectionType.OnPremFromCloudAccess;
					return;
				}
				if (ConnectionInfo.IsHttpAddress(dataSource))
				{
					Uri uri = new Uri(dataSource);
					if (!uri.AbsolutePath.EndsWith(".bism", StringComparison.OrdinalIgnoreCase))
					{
						this.connectionType = ConnectionType.Http;
						if (uri.AbsolutePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || uri.AbsolutePath.EndsWith(".xlsb", StringComparison.OrdinalIgnoreCase) || uri.AbsolutePath.EndsWith(".xlsm", StringComparison.OrdinalIgnoreCase))
						{
							string uriString = XmlaHttpUtility.ExtractSessionFromUrl(dataSource, out this.excelSessionId);
							uri = new Uri(uriString);
							if (!string.IsNullOrEmpty(uri.Query))
							{
								throw new ArgumentException(XmlaSR.ConnectionString_DataSourceTypeDoesntSupportQuery);
							}
							dataSource = uri.GetLeftPart(UriPartial.Path);
							if (ConnectionInfo.IsInO15CompatibleFarm(dataSource))
							{
								this.connectionType = ConnectionType.LocalFarm;
							}
							else if (this.IsInLegacyFarm(dataSource))
							{
								this.connectionType = ConnectionType.Wcf;
							}
							this.SetCatalog(null);
							this.sandboxPath = uri.AbsolutePath;
							if (string.Compare(dataSource, 0, "https://", 0, "https://".Length, StringComparison.OrdinalIgnoreCase) == 0)
							{
								this.redirectorAddress = uri.GetLeftPart(UriPartial.Authority) + "/_vti_bin/PowerPivot/secureredirector.svc";
							}
							else
							{
								this.redirectorAddress = uri.GetLeftPart(UriPartial.Authority) + "/_vti_bin/PowerPivot/redirector.svc";
							}
						}
					}
					this.SetServer(dataSource);
					return;
				}
				string text;
				string value;
				string value2;
				ConnectionInfo.ExtractDataSourceStringParts(dataSource, out text, out value, out value2);
				if (!string.IsNullOrEmpty(location) && !location.Equals(dataSource))
				{
					string text2;
					string text3;
					string text4;
					ConnectionInfo.ExtractDataSourceStringParts(location, out text2, out text3, out text4);
					this.location = text2.Trim();
					if (string.IsNullOrEmpty(value2))
					{
						value2 = text4;
					}
					if (string.IsNullOrEmpty(value))
					{
						value = text3;
					}
				}
				this.SetServer(text);
				this.SetPort(value);
				this.SetInstanceName(value2);
				this.connectionType = ConnectionType.Native;
				if (string.IsNullOrEmpty(this.server))
				{
					throw new ArgumentException(XmlaSR.ConnectionString_Invalid);
				}
				return;
			}
		}

		internal string GetOnPremConnectionString(out string externalServiceConfiguration)
		{
			if (!this.IsOnPremFromCloudAccess)
			{
				this.SetConnectionString(this.connectionString);
			}
			if (string.IsNullOrEmpty(this.externalTenantId))
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_ExternalConnectionIsIncomplete("External Tenant Id"));
			}
			if (string.IsNullOrEmpty(this.externalUserId))
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_ExternalConnectionIsIncomplete("External User Id"));
			}
			if (string.IsNullOrEmpty(this.externalServiceDomainName))
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_ExternalConnectionIsIncomplete("External Service Domain Name"));
			}
			if (string.IsNullOrEmpty(this.externalCertificateThumbprint))
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_ExternalConnectionIsIncomplete("External Certificate Thumbprint"));
			}
			externalServiceConfiguration = string.Format("{0}={1};{2}={3}", new object[]
			{
				"External Service Domain Name",
				this.externalServiceDomainName,
				"External Certificate Thumbprint",
				this.externalCertificateThumbprint
			});
			DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder();
			dbConnectionStringBuilder.ConnectionString = this.ConnectionString;
			dbConnectionStringBuilder.Remove("External Service Domain Name");
			dbConnectionStringBuilder.Remove("External Certificate Thumbprint");
			return dbConnectionStringBuilder.ConnectionString;
		}

		internal ConnectionInfo CloneForInstanceLookup()
		{
			ConnectionInfo connectionInfo = new ConnectionInfo();
			connectionInfo.server = this.server;
			if (this.IsSchannelSspi() && this.location != null)
			{
				connectionInfo.server = this.location;
			}
			connectionInfo.instanceName = null;
			connectionInfo.port = ((this.Port == null) ? 2382.ToString(CultureInfo.InvariantCulture) : this.Port);
			connectionInfo.location = this.location;
			connectionInfo.userID = this.userID;
			connectionInfo.password = this.password;
			connectionInfo.timeout = this.timeout;
			connectionInfo.connectTimeout = this.connectTimeout;
			connectionInfo.autoSyncPeriod = this.autoSyncPeriod;
			connectionInfo.catalog = null;
			connectionInfo.protectionLevel = this.protectionLevel;
			connectionInfo.protectionLevelWasSet = this.protectionLevelWasSet;
			connectionInfo.connectTo = this.connectTo;
			connectionInfo.safetyOptions = this.safetyOptions;
			connectionInfo.protocolFormat = this.protocolFormat;
			connectionInfo.transportCompression = this.transportCompression;
			connectionInfo.compressionLevel = this.compressionLevel;
			connectionInfo.integratedSecurity = this.integratedSecurity;
			connectionInfo.encryptionPassword = this.encryptionPassword;
			connectionInfo.impersonationLevel = this.impersonationLevel;
			connectionInfo.sspi = "Negotiate";
			connectionInfo.useExistingFile = this.useExistingFile;
			connectionInfo.characterEncoding = this.characterEncoding;
			connectionInfo.packetSize = this.packetSize;
			connectionInfo.useEncryptionForData = this.useEncryptionForData;
			connectionInfo.innerConnectionString = null;
			connectionInfo.isForSqlBrowser = true;
			connectionInfo.isLightweightConnection = false;
			connectionInfo.restrictedClient = this.restrictedClient;
			return connectionInfo;
		}

		internal void SetServer(string value)
		{
			this.server = ConnectionInfo.Trim(value);
		}

		internal void SetInstanceName(string value)
		{
			this.instanceName = ConnectionInfo.Trim(value);
		}

		internal void SetPort(string value)
		{
			this.port = ConnectionInfo.Trim(value);
		}

		internal void SetCatalog(string value)
		{
			this.catalog = value;
			this.InsertKeyValueIntoHash("Catalog", value);
		}

		internal bool IsSchannelSspi()
		{
			return string.Compare(this.sspi, "Schannel", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.sspi, "Microsoft Unified Security Protocol Provider", StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal string GetRedirectorUrlForDatabase()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			stringBuilder.Append(this.redirectorAddress);
			stringBuilder.Append("/?DataSource=");
			stringBuilder.Append(Uri.EscapeDataString(this.sandboxPath));
			if (!string.IsNullOrEmpty(this.dataSourceVersion))
			{
				stringBuilder.Append("&DataSourceVersion=");
				stringBuilder.Append(Uri.EscapeDataString(this.dataSourceVersion));
			}
			return stringBuilder.ToString();
		}

		internal string GetRedirectorUrlForRedirect(string databaseId, bool specificVersion)
		{
			if (specificVersion)
			{
				return this.redirectorAddress + "/?DatabaseId=" + databaseId + "&SpecificVersion=true";
			}
			return this.redirectorAddress + "/?DatabaseId=" + databaseId;
		}

		private static void ParseStringKeyValue(string stringToParse, ConnectionInfo.KeyValueCallback keyValueHandler)
		{
			string value = null;
			int i = 0;
			int num = 0;
			int num2 = 0;
			ConnectionInfo.SkipWhiteSpace(stringToParse, ref i);
			int length = stringToParse.Length;
			char[] array = new char[length - i];
			while (i < length)
			{
				if (stringToParse[i] == ';')
				{
					i++;
					ConnectionInfo.SkipWhiteSpace(stringToParse, ref i);
				}
				else
				{
					int num3 = 0;
					ConnectionInfo.DecodeFragment(stringToParse, array, ref num3, i, '=', ref num);
					if (num == -1)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_ExpectedEqualSignNotFound(i));
					}
					if (num3 == 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_PropertyNameNotDefined(num));
					}
					string key = new string(array, 0, num3).TrimEnd(null);
					if (num + 1 == length)
					{
						value = null;
						i = length;
					}
					else
					{
						i = num + 1;
						ConnectionInfo.SkipWhiteSpace(stringToParse, ref i);
						if (i == length)
						{
							value = null;
						}
						else
						{
							ConnectionInfo.ParseValue(stringToParse, ref value, ref i, ref num2);
						}
					}
					keyValueHandler(key, value);
				}
			}
		}

		private void HandleKeyValueDuringConnectionStringParsing(string key, string value)
		{
			if (this.CheckAndSetDataSource(key, value))
			{
				this.SetOriginalConnectionStringValue("Data Source", value);
				return;
			}
			if (this.CheckAndSetLocation(key, value))
			{
				this.SetOriginalConnectionStringValue("Location", value);
				return;
			}
			if (this.CheckAndSetUserID(key, value))
			{
				this.SetOriginalConnectionStringValue("User ID", value);
				return;
			}
			if (this.CheckAndSetPassword(key, value))
			{
				this.SetOriginalConnectionStringValue("Password", value);
				return;
			}
			if (this.CheckAndSetProtectionLevel(key, value))
			{
				this.SetOriginalConnectionStringValue("Protection Level", value);
				return;
			}
			if (this.CheckAndSetIntegratedSecurity(key, value))
			{
				this.SetOriginalConnectionStringValue("Integrated Security", value);
				return;
			}
			if (this.CheckAndSetConnectTimeout(key, value))
			{
				this.SetOriginalConnectionStringValue("Connect Timeout", value);
				return;
			}
			if (this.CheckAndSetAutoSyncPeriod(key, value))
			{
				this.SetOriginalConnectionStringValue("Auto Synch Period", value);
				return;
			}
			if (this.CheckAndSetConnectTo(key, value))
			{
				this.SetOriginalConnectionStringValue("ConnectTo", value);
				return;
			}
			if (this.CheckAndSetProtocolFormat(key, value))
			{
				this.SetOriginalConnectionStringValue("Protocol Format", value);
				return;
			}
			if (this.CheckAndSetTransportCompression(key, value))
			{
				this.SetOriginalConnectionStringValue("Transport Compression", value);
				return;
			}
			if (this.CheckAndSetCompressionLevel(key, value))
			{
				this.SetOriginalConnectionStringValue("Compression Level", value);
				return;
			}
			if (this.CheckAndSetEncryptionPassword(key, value))
			{
				this.SetOriginalConnectionStringValue("Encryption Password", value);
				return;
			}
			if (this.CheckAndSetImpersonationLevel(key, value))
			{
				this.SetOriginalConnectionStringValue("Impersonation Level", value);
				return;
			}
			if (this.CheckAndSetRestrictedClient(key, value))
			{
				this.SetOriginalConnectionStringValue("Restricted Client", value);
				return;
			}
			if (this.CheckAndSetSspi(key, value))
			{
				this.SetOriginalConnectionStringValue("SSPI", value);
				return;
			}
			if (this.CheckAndSetUseExistingFile(key, value))
			{
				this.SetOriginalConnectionStringValue("UseExistingFile", value);
				return;
			}
			if (this.CheckAndSetCharacterEncoding(key, value))
			{
				this.SetOriginalConnectionStringValue("Character Encoding", value);
				return;
			}
			if (this.CheckAndSetUseEncryptionForData(key, value))
			{
				this.SetOriginalConnectionStringValue("Use Encryption for Data", value);
				return;
			}
			if (this.CheckAndSetPacketSize(key, value))
			{
				this.SetOriginalConnectionStringValue("Packet Size", value);
				return;
			}
			if (this.CheckAndSetPersistSecurityInfo(key, value))
			{
				this.SetOriginalConnectionStringValue("Persist Security Info", value);
				return;
			}
			if (this.CheckAndSetSessionID(key, value))
			{
				this.SetOriginalConnectionStringValue("SessionID", value);
				return;
			}
			if (this.CheckAndSetDataSourceVersion(key, value))
			{
				this.SetOriginalConnectionStringValue("Data Source Version", value);
				return;
			}
			if (this.CheckAndSetClientCertificateThumbprint(key, value))
			{
				this.SetOriginalConnectionStringValue("Client Certificate Thumbprint", value);
				return;
			}
			if (this.CheckAndSetUserIdentity(key, value))
			{
				this.SetOriginalConnectionStringValue("User Identity", value);
				return;
			}
			if (this.CheckAndSetExtendedProperties(key, value))
			{
				this.SetOriginalConnectionStringValue("Extended Properties", value);
				return;
			}
			if (this.CheckAndSetCertificate(key, value))
			{
				this.SetOriginalConnectionStringValue("Certificate", value);
				return;
			}
			if (this.CheckAndSetExternalTenantId(key, value))
			{
				this.SetOriginalConnectionStringValue("External Tenant Id", value);
				this.isOnPremFromCloudAccess = true;
				return;
			}
			if (this.CheckAndSetExternalUserId(key, value))
			{
				this.SetOriginalConnectionStringValue("External User Id", value);
				this.isOnPremFromCloudAccess = true;
				return;
			}
			if (this.CheckAndSetExternalServiceDomainName(key, value))
			{
				this.SetOriginalConnectionStringValue("External Service Domain Name", value);
				return;
			}
			if (this.CheckAndSetExternalCertificateThumbprint(key, value))
			{
				this.SetOriginalConnectionStringValue("External Certificate Thumbprint", value);
				return;
			}
			if (this.CheckAndSetUseAdalCache(key, value))
			{
				this.SetOriginalConnectionStringValue("UseADALCache", value);
				return;
			}
			if (this.CheckAndSetIsInternalASAzure(key, value))
			{
				this.SetOriginalConnectionStringValue("IsInternalASAzure", value);
				return;
			}
			if (this.CheckAndSetAuthenticationScheme(key, value))
			{
				this.SetOriginalConnectionStringValue("Authentication Scheme", value);
				return;
			}
			if (this.CheckAndSetExtAuthInfo(key, value))
			{
				this.SetOriginalConnectionStringValue("Ext Auth Info", value);
				return;
			}
			if (this.CheckAndSetIdentityProvider(key, value))
			{
				this.SetOriginalConnectionStringValue("Identity Provider", value);
				return;
			}
			if (this.CheckAndSetBypassAuthorization(key, value))
			{
				this.SetOriginalConnectionStringValue("Bypass Authorization", value);
				return;
			}
			if (this.CheckAndSetRestrictCatalog(key, value))
			{
				this.SetOriginalConnectionStringValue("Restrict Catalog", value);
				return;
			}
			if (this.CheckAndSetAccessMode(key, value))
			{
				this.SetOriginalConnectionStringValue("Access Mode", value);
				return;
			}
			if (this.CheckAndSetSafetyOptions(key, value))
			{
				key = "Safety Options";
			}
			else if (this.CheckAndSetCatalog(key, value))
			{
				key = "Catalog";
			}
			else
			{
				this.CheckAndSetTimeout(key, value);
			}
			this.SetOriginalConnectionStringValue(key, value);
			this.InsertKeyValueIntoHash(key, value);
		}

		private static void DecodeFragment(string stringToParse, char[] buffer, ref int bufferOffset, int startIndex, char delimiter, ref int delimiterIndex)
		{
			delimiterIndex = -1;
			int i = startIndex;
			int length = stringToParse.Length;
			while (i < length)
			{
				char c = stringToParse[i];
				if (c == delimiter)
				{
					i++;
					if (i == length || stringToParse[i] != delimiter)
					{
						delimiterIndex = i - 1;
						return;
					}
				}
				buffer[bufferOffset++] = c;
				i++;
			}
		}

		private static void SkipWhiteSpace(string conString, ref int index)
		{
			while (index < conString.Length)
			{
				char c = conString[index];
				if (c != ' ' && c != '\t')
				{
					return;
				}
				index++;
			}
		}

		private static void ParseValue(string stringToParse, ref string value, ref int index, ref int valueIndex)
		{
			int num = -1;
			char c = stringToParse[index];
			char c2 = c;
			if (c2 != '"' && c2 != '\'')
			{
				if (c2 == ';')
				{
					value = null;
					index++;
					ConnectionInfo.SkipWhiteSpace(stringToParse, ref index);
					return;
				}
				num = stringToParse.IndexOf(';', index + 1);
				int num2;
				if (num == -1)
				{
					num2 = stringToParse.Length;
				}
				else
				{
					num2 = num;
				}
				for (int i = index; i < num2; i++)
				{
					char c3 = stringToParse[i];
					if (c3 == '\'' || c3 == '"')
					{
						throw new ArgumentException(XmlaSR.ConnectionString_InvalidCharInUnquotedPropertyValue(c3, i));
					}
				}
				if (num == -1)
				{
					value = stringToParse.Substring(index).TrimEnd(null);
					valueIndex = index;
					index = stringToParse.Length;
					return;
				}
				value = stringToParse.Substring(index, num - index).TrimEnd(null);
				valueIndex = index;
				index = num + 1;
				ConnectionInfo.SkipWhiteSpace(stringToParse, ref index);
			}
			else
			{
				char[] array = new char[stringToParse.Length - index - 1];
				int length = 0;
				ConnectionInfo.DecodeFragment(stringToParse, array, ref length, index + 1, c, ref num);
				if (num == -1)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_OpenedQuoteIsNotClosed(c, index));
				}
				value = new string(array, 0, length);
				valueIndex = index + 1;
				index = num + 1;
				ConnectionInfo.SkipWhiteSpace(stringToParse, ref index);
				if (index < stringToParse.Length)
				{
					if (stringToParse[index] == ';')
					{
						index++;
						ConnectionInfo.SkipWhiteSpace(stringToParse, ref index);
						return;
					}
					throw new ArgumentException(XmlaSR.ConnectionString_ExpectedSemicolonNotFound(index));
				}
			}
		}

		private void InsertKeyValueIntoHash(string propName, string propValue)
		{
			string key;
			if (ConnectionInfo.PropInfo.Properties.ContainsKey(propName))
			{
				if (!(string.Empty != ConnectionInfo.PropInfo.Properties[propName].ToString()))
				{
					return;
				}
				key = ConnectionInfo.PropInfo.Properties[propName].ToString();
			}
			else
			{
				if (!XmlReader.IsName(propName))
				{
					throw new ArgumentException(XmlaSR.ConnectionString_InvalidPropertyNameFormat(propName));
				}
				key = propName;
			}
			this.extendedProperties[key] = propValue;
		}

		private bool CheckAndSetDataSource(string key, string value)
		{
			int i = 0;
			int num = ConnectionInfo.DatasourcePropertyNames.Length;
			while (i < num)
			{
				if (string.Compare(key, ConnectionInfo.DatasourcePropertyNames[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.dataSource = ConnectionInfo.Trim(value);
					return true;
				}
				i++;
			}
			return false;
		}

		private bool CheckAndSetLocation(string key, string value)
		{
			if (string.Compare(key, "Location", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.location = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetUserID(string key, string value)
		{
			int i = 0;
			int num = ConnectionInfo.UserIDPropertyNames.Length;
			while (i < num)
			{
				if (string.Compare(key, ConnectionInfo.UserIDPropertyNames[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.userID = value;
					return true;
				}
				i++;
			}
			return false;
		}

		private bool CheckAndSetPassword(string key, string value)
		{
			if (ConnectionInfo.IsPassword(key))
			{
				this.pwdPresent = true;
				this.password = value;
				return true;
			}
			return false;
		}

		private static bool IsPassword(string key)
		{
			int i = 0;
			int num = ConnectionInfo.PasswordPropertyNames.Length;
			while (i < num)
			{
				if (string.Compare(key, ConnectionInfo.PasswordPropertyNames[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		private bool CheckAndSetCatalog(string key, string value)
		{
			int i = 0;
			int num = ConnectionInfo.CatalogPropertyNames.Length;
			while (i < num)
			{
				if (string.Compare(key, ConnectionInfo.CatalogPropertyNames[i], StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.catalog = value;
					return true;
				}
				i++;
			}
			return false;
		}

		private bool CheckAndSetProtectionLevel(string key, string value)
		{
			if (string.Compare(key, "Protection Level", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (string.Compare(value, "NONE", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.protectionLevel = ProtectionLevel.None;
				}
				else if (string.Compare(value, "CONNECT", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.protectionLevel = ProtectionLevel.Connection;
				}
				else if (string.Compare(value, "PKT INTEGRITY", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.protectionLevel = ProtectionLevel.Integrity;
				}
				else
				{
					if (string.Compare(value, "PKT PRIVACY", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Protection Level", value));
					}
					this.protectionLevel = ProtectionLevel.Privacy;
				}
				this.protectionLevelWasSet = true;
				return true;
			}
			return false;
		}

		private bool CheckAndSetIntegratedSecurity(string key, string value)
		{
			if (string.Compare(key, "Integrated Security", StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = ConnectionInfo.Trim(value);
				if (value == null)
				{
					this.integratedSecurity = IntegratedSecurity.Unspecified;
				}
				else if (string.Compare(value, "SSPI", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.integratedSecurity = IntegratedSecurity.Sspi;
				}
				else if (string.Compare(value, "Basic", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.integratedSecurity = IntegratedSecurity.Basic;
				}
				else if (string.Compare(value, "Federated", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.integratedSecurity = IntegratedSecurity.Federated;
				}
				else
				{
					if (string.Compare(value, "ClaimsToken", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Integrated Security", value));
					}
					this.integratedSecurity = IntegratedSecurity.ClaimsToken;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetConnectTo(string key, string value)
		{
			if (string.Compare(key, "ConnectTo", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value == null || value.Length == 0)
				{
					this.connectTo = ConnectTo.Default;
				}
				else
				{
					value = ConnectionInfo.Trim(value);
					if (string.Compare(value, "DEFAULT", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.connectTo = ConnectTo.Default;
					}
					else if (string.Compare(value, "8.0", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.connectTo = ConnectTo.Shiloh;
					}
					else if (string.Compare(value, "9.0", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.connectTo = ConnectTo.Yukon;
					}
					else if (string.Compare(value, "10.0", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.connectTo = ConnectTo.Katmai;
					}
					else if (string.Compare(value, "11.0", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.connectTo = ConnectTo.Denali;
					}
					else if (string.Compare(value, "12.0", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.connectTo = ConnectTo.SQL14;
					}
					else
					{
						if (string.Compare(value, "13.0", StringComparison.OrdinalIgnoreCase) != 0)
						{
							throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("ConnectTo", value));
						}
						this.connectTo = ConnectTo.SQL15;
					}
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetTimeout(string key, string value)
		{
			if (string.Compare(key, "Timeout", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value != null && value.Length != 0)
				{
					try
					{
						this.timeout = int.Parse(value, CultureInfo.InvariantCulture);
						return true;
					}
					catch (FormatException innerException)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Timeout", value), innerException);
					}
					catch (OverflowException innerException2)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Timeout", value), innerException2);
					}
					return true;
				}
				this.timeout = 0;
				return true;
			}
			return false;
		}

		private bool CheckAndSetConnectTimeout(string key, string value)
		{
			if (string.Compare(key, "Connect Timeout", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value == null || value.Length == 0)
				{
					this.connectTimeout = 60;
				}
				else
				{
					try
					{
						this.connectTimeout = int.Parse(value, CultureInfo.InvariantCulture);
					}
					catch (FormatException innerException)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Connect Timeout", value), innerException);
					}
					catch (OverflowException innerException2)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Connect Timeout", value), innerException2);
					}
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetAutoSyncPeriod(string key, string value)
		{
			if (string.Compare(key, "Auto Synch Period", StringComparison.OrdinalIgnoreCase) == 0)
			{
				try
				{
					if (string.Compare(value, "NULL", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.autoSyncPeriod = 0u;
					}
					else
					{
						this.autoSyncPeriod = uint.Parse(value, CultureInfo.InvariantCulture);
					}
					return true;
				}
				catch (FormatException innerException)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Auto Synch Period", value), innerException);
				}
				catch (OverflowException innerException2)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Auto Synch Period", value), innerException2);
				}
				return false;
			}
			return false;
		}

		private bool CheckAndSetSafetyOptions(string key, string value)
		{
			if (string.Compare(key, "Safety Options", StringComparison.OrdinalIgnoreCase) == 0)
			{
				uint num = 3u;
				try
				{
					num = uint.Parse(value, CultureInfo.InvariantCulture);
				}
				catch (FormatException innerException)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Safety Options", value), innerException);
				}
				catch (OverflowException innerException2)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Safety Options", value), innerException2);
				}
				switch (num)
				{
				case 0u:
					this.safetyOptions = SafetyOptions.Default;
					break;
				case 1u:
					this.safetyOptions = SafetyOptions.All;
					break;
				case 2u:
					this.safetyOptions = SafetyOptions.Safe;
					break;
				case 3u:
					this.safetyOptions = SafetyOptions.None;
					break;
				default:
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Safety Options", value));
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetProtocolFormat(string key, string value)
		{
			if (string.Compare(key, "Protocol Format", StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = ConnectionInfo.Trim(value);
				if (string.Compare(value, "Default", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.protocolFormat = ProtocolFormat.Default;
				}
				else if (string.Compare(value, "XML", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.protocolFormat = ProtocolFormat.Xml;
				}
				else
				{
					if (string.Compare(value, "Binary", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue(key, value));
					}
					this.protocolFormat = ProtocolFormat.Binary;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetTransportCompression(string key, string value)
		{
			if (string.Compare(key, "Transport Compression", StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = ConnectionInfo.Trim(value);
				if (string.Compare(value, "Default", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.transportCompression = TransportCompression.Default;
				}
				else if (string.Compare(value, "None", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.transportCompression = TransportCompression.None;
				}
				else if (string.Compare(value, "Compressed", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.transportCompression = TransportCompression.Compressed;
				}
				else
				{
					if (string.Compare(value, "Gzip", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue(key, value));
					}
					this.transportCompression = TransportCompression.Gzip;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetCompressionLevel(string key, string value)
		{
			if (string.Compare(key, "Compression Level", StringComparison.OrdinalIgnoreCase) != 0)
			{
				return false;
			}
			int num;
			try
			{
				num = int.Parse(value, CultureInfo.InvariantCulture);
			}
			catch (FormatException innerException)
			{
				throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue(key, value), innerException);
			}
			catch (OverflowException innerException2)
			{
				throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue(key, value), innerException2);
			}
			if (num < 0 || num > 9)
			{
				throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue(key, value));
			}
			this.compressionLevel = num;
			return true;
		}

		private bool CheckAndSetEncryptionPassword(string key, string value)
		{
			if (string.Compare(key, "Encryption Password", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.encryptionPassword = value;
				return true;
			}
			return false;
		}

		private bool CheckAndSetImpersonationLevel(string key, string value)
		{
			if (string.Compare(key, "Impersonation Level", StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = ConnectionInfo.Trim(value);
				if (string.Compare(value, "Anonymous", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.impersonationLevel = ImpersonationLevel.Anonymous;
				}
				else if (string.Compare(value, "Identify", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.impersonationLevel = ImpersonationLevel.Identify;
				}
				else if (string.Compare(value, "Impersonate", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.impersonationLevel = ImpersonationLevel.Impersonate;
				}
				else
				{
					if (string.Compare(value, "Delegate", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Impersonation Level", value));
					}
					this.impersonationLevel = ImpersonationLevel.Delegate;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetRestrictedClient(string key, string value)
		{
			if (string.Compare(key, "Restricted Client", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.restrictedClient = ConnectionInfo.ParseBool(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetSspi(string key, string value)
		{
			if (string.Compare(key, "SSPI", StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = ConnectionInfo.Trim(value);
				this.sspi = ((value == null) ? "Negotiate" : value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetUseExistingFile(string key, string value)
		{
			if (string.Compare(key, "UseExistingFile", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value == null || value.Length == 0)
				{
					this.useExistingFile = false;
				}
				else if (string.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.useExistingFile = true;
				}
				else
				{
					if (string.Compare(value, bool.FalseString, StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("UseExistingFile", value));
					}
					this.useExistingFile = false;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetCharacterEncoding(string key, string value)
		{
			if (string.Compare(key, "Character Encoding", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value == null || value.Length == 0)
				{
					this.characterEncoding = Encoding.UTF8;
				}
				else if (string.Compare(value, "Default", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.characterEncoding = Encoding.UTF8;
				}
				else
				{
					try
					{
						this.characterEncoding = Encoding.GetEncoding(value);
					}
					catch (NotSupportedException innerException)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Character Encoding", value), innerException);
					}
					catch (ArgumentException innerException2)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Character Encoding", value), innerException2);
					}
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetUseEncryptionForData(string key, string value)
		{
			if (string.Compare(key, "Use Encryption for Data", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value != null && value.Length > 0)
				{
					value = ConnectionInfo.Trim(value);
					if (string.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.useEncryptionForData = bool.TrueString;
					}
					else
					{
						if (string.Compare(value, bool.FalseString, StringComparison.OrdinalIgnoreCase) != 0)
						{
							throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Use Encryption for Data", value));
						}
						this.useEncryptionForData = bool.FalseString;
					}
				}
				else
				{
					this.useEncryptionForData = null;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetPacketSize(string key, string value)
		{
			if (string.Compare(key, "Packet Size", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value != null && value.Length > 0)
				{
					int num;
					try
					{
						num = int.Parse(value, CultureInfo.InvariantCulture);
					}
					catch (FormatException innerException)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Packet Size", value), innerException);
					}
					catch (OverflowException innerException2)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Packet Size", value), innerException2);
					}
					if (num < 512 || num > 32767)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Packet Size", value));
					}
					this.packetSize = num;
				}
				else
				{
					this.packetSize = 4096;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetDataSourceVersion(string key, string value)
		{
			if (!"Data Source Version".Equals(key, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Data Source Version", value));
			}
			this.dataSourceVersion = value;
			return true;
		}

		private bool CheckAndSetUserIdentity(string key, string value)
		{
			if (string.Compare(key, "User Identity", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (value == null)
				{
					this.userIdentity = UserIdentityType.WindowsIdentity;
				}
				else if (string.Compare(value, "DEFAULT", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.userIdentity = UserIdentityType.WindowsIdentity;
				}
				else if (string.Compare(value, "Windows Identity", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.userIdentity = UserIdentityType.WindowsIdentity;
				}
				else
				{
					if (string.Compare(value, "SharePoint Principal", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("User Identity", value));
					}
					this.userIdentity = UserIdentityType.SharePointPrincipal;
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetUseAdalCache(string key, string value)
		{
			if (string.Compare(key, "UseADALCache", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.useAdalCache = ConnectionInfo.ParseBool(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetIsInternalASAzure(string key, string value)
		{
			return false;
		}

		private bool CheckAndSetClientCertificateThumbprint(string key, string value)
		{
			if (string.Compare(key, "Client Certificate Thumbprint", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.clientCertificateThumbprint = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetExtendedProperties(string key, string value)
		{
			if (ConnectionInfo.IsExtendedProperties(key))
			{
				this.innerConnectionString = value;
				return true;
			}
			return false;
		}

		private static bool IsExtendedProperties(string key)
		{
			return string.Compare(key, "Extended Properties", StringComparison.OrdinalIgnoreCase) == 0;
		}

		private bool CheckAndSetPersistSecurityInfo(string key, string value)
		{
			if (string.Compare(key, "Persist Security Info", StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = ConnectionInfo.Trim(value);
				try
				{
					this.persistSecurityInfo = bool.Parse(value);
				}
				catch (ArgumentNullException)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Persist Security Info", value));
				}
				catch (FormatException)
				{
					throw new ArgumentException(XmlaSR.ConnectionString_UnsupportedPropertyValue("Persist Security Info", value));
				}
				return true;
			}
			return false;
		}

		private bool CheckAndSetSessionID(string key, string value)
		{
			if (string.Compare(key, "SessionID", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.sessionID = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetCertificate(string key, string value)
		{
			if (string.Compare(key, "Certificate", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.certificate = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetExternalTenantId(string key, string value)
		{
			if (string.Compare(key, "External Tenant Id", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.externalTenantId = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetExternalUserId(string key, string value)
		{
			if (string.Compare(key, "External User Id", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.externalUserId = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetExternalServiceDomainName(string key, string value)
		{
			if (string.Compare(key, "External Service Domain Name", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.externalServiceDomainName = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetExternalCertificateThumbprint(string key, string value)
		{
			if (string.Compare(key, "External Certificate Thumbprint", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.externalCertificateThumbprint = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetAuthenticationScheme(string key, string value)
		{
			if (string.Compare(key, "Authentication Scheme", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.authenticationScheme = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetExtAuthInfo(string key, string value)
		{
			if (string.Compare(key, "Ext Auth Info", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.extAuthInfo = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetIdentityProvider(string key, string value)
		{
			if (string.Compare(key, "Identity Provider", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.identityProvider = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetBypassAuthorization(string key, string value)
		{
			if (string.Compare(key, "Bypass Authorization", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.bypassAuthorization = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetRestrictCatalog(string key, string value)
		{
			if (string.Compare(key, "Restrict Catalog", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.restrictCatalog = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		private bool CheckAndSetAccessMode(string key, string value)
		{
			if (string.Compare(key, "Access Mode", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.accessMode = ConnectionInfo.Trim(value);
				return true;
			}
			return false;
		}

		internal static bool IsHttpAddress(string address)
		{
			return address != null && (string.Compare(address, 0, "http://", 0, "http://".Length, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(address, 0, "https://", 0, "https://".Length, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(address, 0, "asazure://", 0, "asazure://".Length, StringComparison.OrdinalIgnoreCase) == 0);
		}

		internal static bool IsBism(string address)
		{
			if (ConnectionInfo.IsHttpAddress(address) || ConnectionInfo.IsHttpsAddress(address))
			{
				Uri uri = new Uri(address);
				if (uri.AbsolutePath.EndsWith(".bism", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsHttpsAddress(string address)
		{
			return address != null && string.Compare(address, 0, "https://", 0, "https://".Length, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private static bool IsAsAzureAddress(string address)
		{
			return address != null && string.Compare(address, 0, "asazure://", 0, "asazure://".Length, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private bool IsLocalCubeFile(string path)
		{
			return -1 == path.IndexOfAny(Path.GetInvalidPathChars()) && 0 == string.Compare(".cub", Path.GetExtension(path), StringComparison.OrdinalIgnoreCase);
		}

		private bool IsEmbeddedPath(string path)
		{
			return string.Compare(path, "$Embedded$", StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static bool IsInO15CompatibleFarm(string path)
		{
			bool result;
			try
			{
				result = XmlaClient.SPProxy.IsWorkbookInFarm(path);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		internal bool IsInO15CompatibleFarm()
		{
			bool result;
			try
			{
				result = XmlaClient.SPProxy.IsWorkbookInFarm(this.dataSource);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		internal bool IsRunningInO15CompatibleFarm()
		{
			bool result;
			try
			{
				result = XmlaClient.SPProxy.IsRunningInFarm(15);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private bool IsInLegacyFarm(string path)
		{
			bool result;
			try
			{
				object[] invokeArgs = new object[]
				{
					path
				};
				bool flag = (bool)XmlaClient.InvokeMemberFromWCFXmlaClientClass("IsInFarmHelper", invokeArgs);
				result = flag;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private ASLinkFile GetLocalLinkFile(string filepath)
		{
			ASLinkFile aSLinkFile = ASLinkFile.NotInFarm;
			if (!aSLinkFile.IsInFarm)
			{
				aSLinkFile = this.GetLocalO15CompatibleLinkFile(filepath);
			}
			if (!aSLinkFile.IsInFarm)
			{
				aSLinkFile = this.GetLocalLegacyLinkFile(filepath);
			}
			return aSLinkFile;
		}

		private ASLinkFile GetLocalO15CompatibleLinkFile(string filepath)
		{
			ASLinkFile result;
			try
			{
				ILinkFile linkFile = XmlaClient.SPProxy.GetLinkFile(filepath);
				ASLinkFile aSLinkFile = new ASLinkFile(linkFile);
				result = aSLinkFile;
			}
			catch
			{
				result = ASLinkFile.NotInFarm;
			}
			return result;
		}

		private ASLinkFile GetLocalLegacyLinkFile(string filepath)
		{
			if (!this.IsInLegacyFarm(filepath))
			{
				return ASLinkFile.NotInFarm;
			}
			Assembly assembly;
			try
			{
				assembly = Assembly.Load("Microsoft.AnalysisServices.SharePoint.Integration, PublicKeyToken=89845dcd8080cc91, Culture=neutral, Version=13.0.0.0");
			}
			catch (Exception)
			{
				ASLinkFile notInFarm = ASLinkFile.NotInFarm;
				return notInFarm;
			}
			bool flag = false;
			string text = null;
			bool flag2 = false;
			bool flag3 = false;
			Type type = assembly.GetType("Microsoft.AnalysisServices.SharePoint.Integration.ASLinkFile", true, true);
			object[] array = new object[]
			{
				filepath,
				flag,
				this.server,
				text,
				flag2,
				flag3
			};
			type.InvokeMember("ExtractProperties", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, array, CultureInfo.InvariantCulture);
			flag = (bool)array[1];
			this.server = (string)array[2];
			text = (string)array[3];
			flag2 = (bool)array[4];
			flag3 = (bool)array[5];
			if (!flag)
			{
				return ASLinkFile.NotInFarm;
			}
			if (flag3)
			{
				return ASLinkFile.CreateMalformed(new XmlSchemaException());
			}
			return new ASLinkFile
			{
				Server = this.server,
				Database = text,
				IsDelegationAllowed = flag2
			};
		}

		private void FillIXMLAProperties()
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			this.ixmlaProperties = new ListDictionary();
			foreach (string text in this.originalConnStringProps.Keys)
			{
				if (!this.ShouldIgnoreProperty(text))
				{
					string text2 = null;
					ConnectionInfo.PropInfo.Properties.TryGetValue(text, out text2);
					if (!string.IsNullOrEmpty(text2) && !this.ShouldUseConnectionStringProptrty(text2))
					{
						this.ixmlaProperties[text2] = this.originalConnStringProps[text];
					}
					else
					{
						string text3 = this.originalConnStringProps[text] as string;
						if (text3 == null)
						{
							text3 = string.Empty;
						}
						if (text3.LastIndexOfAny(new char[]
						{
							'=',
							';',
							'\'',
							'"',
							' '
						}) > -1)
						{
							stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}';", new object[]
							{
								text,
								text3.Replace("'", "''")
							});
						}
						else
						{
							stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}={1};", new object[]
							{
								text,
								text3
							});
						}
					}
				}
			}
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}';", new object[]
			{
				"Provider",
				"MSOLAP.2"
			});
			this.ixmlaProperties["DataSourceInfo"] = stringBuilder.ToString();
		}

		private bool ShouldIgnoreProperty(string propertyName)
		{
			int i = 0;
			int num = ConnectionInfo.PropertyNamesToIgnore.Length;
			while (i < num)
			{
				if (string.Compare(ConnectionInfo.PropertyNamesToIgnore[i], propertyName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		private bool ShouldUseConnectionStringProptrty(string xmlaProperty)
		{
			return !ConnectionInfo.XMLAPropertiesKnownByIXMLA.Known.ContainsKey(xmlaProperty);
		}

		internal static ConnectionInfo GetModifiedConnectionInfo(ConnectionInfo info, string dataSource)
		{
			ConnectionInfo connectionInfo = new ConnectionInfo(info);
			connectionInfo.SetServer(dataSource);
			connectionInfo.SetInstanceName(null);
			connectionInfo.SetPort(null);
			connectionInfo.connectionType = ConnectionType.LocalCube;
			connectionInfo.connectTo = ConnectTo.SQL15;
			connectionInfo.IXMLAMode = false;
			connectionInfo.dataSource = dataSource;
			connectionInfo.catalog = string.Empty;
			if (connectionInfo.ExtendedProperties.Contains("Catalog"))
			{
				connectionInfo.ExtendedProperties.Remove("Catalog");
			}
			return connectionInfo;
		}

		private static bool TryParseGuid(string strGuid, out Guid guid)
		{
			bool result;
			try
			{
				guid = new Guid(strGuid);
				result = true;
			}
			catch
			{
				guid = Guid.Empty;
				result = false;
			}
			return result;
		}

		private bool ParseShortcutForm()
		{
			return false;
		}

		[Conditional("ADOMD")]
		private void SetOriginalConnectionStringValue(string key, string value)
		{
			this.originalConnStringProps[key] = value;
		}

		private static bool ParseBool(string value)
		{
			value = ConnectionInfo.Trim(value);
			int num;
			return value.StartsWith("y", StringComparison.OrdinalIgnoreCase) || value.StartsWith("t", StringComparison.OrdinalIgnoreCase) || (int.TryParse(value, out num) && num != 0);
		}

		private static bool IsProcessWithUserInterface()
		{
			Process currentProcess = Process.GetCurrentProcess();
			return currentProcess.MainWindowHandle != IntPtr.Zero;
		}
	}
}
