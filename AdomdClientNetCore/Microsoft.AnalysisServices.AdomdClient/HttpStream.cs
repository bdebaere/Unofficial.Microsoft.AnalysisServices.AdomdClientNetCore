using Microsoft.AnalysisServices.AdomdClient.MsoID;
using System;
using System.Globalization;
using System.IO;
using System.Net;
//using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Adomd;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class HttpStream : TransportCapabilitiesAwareXmlaStream
	{
		private sealed class RoutingTokenAccessor
		{
			private readonly ConnectionInfo connectionInfo;

			public string Value
			{
				get
				{
					return this.connectionInfo.RoutingToken;
				}
				set
				{
					this.connectionInfo.RoutingToken = value;
				}
			}

			public RoutingTokenAccessor(ConnectionInfo connectionInfo)
			{
				this.connectionInfo = connectionInfo;
			}
		}

		private const string TransportCapabilitiesNegotiationHeader = "X-Transport-Caps-Negotiation-Flags";

		private const string ApplicationNameHeader = "SspropInitAppName";

		private const string ActivityIDHeader = "X-AS-ActivityID";

		private const string RequestIDHeader = "X-AS-RequestID";

		private const string CurrentActivityIDHeader = "X-AS-CurrentActivityID";

		private const string OutdatedVersionHeader = "OutdatedVersion";

		private const string SessionIDHeader = "X-AS-SessionID";

		private const string ContentEncodingHeader = "Content-Encoding";

		private const string XASRouting = "X-AS-Routing";

		private const string HttpAuthorizationHeaderName = "Authorization";

		private const string HttpAuthenticationMsoIDSchemeName = "MsoID";

		private const string HttpAuthenticationBearerSchemeName = "Bearer";

		private const string SessionTokenRequestHeader = "X-AS-GetSessionToken";

		private const string ASAzureServerHeaderName = "x-ms-xmlaserver";

		private const string ASAzureCapsNegotiationFlagsHeaderName = "x-ms-xmlacaps-negotiation-flags";

		private const string ASAzureSessionIdHeaderName = "x-ms-xmlasession-id";

		private const string ASAzureAppGeneralInfoHeaderName = "x-ms-xmlaapp-general-info";

		private const string ASAzureParentActivityIdHeaderName = "x-ms-parent-activity-id";

		private const string ASAzureRootActivityIdHeaderName = "x-ms-root-activity-id";

		private const string RootActivityIdCallContextKey = "x-ms-root-activity-id";

		private const string ParentActivityIdCallContextKey = "x-ms-parent-activity-id";

		private const string AuthUserNameHeader = "X-AS-AuthorizedUserName";

		private const string AuthUserIDHeader = "X-AS-AuthorizedUserID";

		private const string AuthUserTenantHeader = "X-AS-AuthorizedUserTenant";

		private const string ASDatabaseAccessTokenHeader = "X-SAAS-DatabaseAccessToken";

		private const int TCP_KEEP_ALIVE_TIME_IN_MS = 30000;

		private const int TCP_KEEP_ALIVE_INTERVAL_IN_MS = 30000;

		private const string contentTypeParam = "content_type";

		private HttpWebRequest httpRequest;

		private Stream httpRequestStream;

		private HttpWebResponse httpResponse;

		private Stream httpResponseStream;

		private Uri webSite;

		private ICredentials credentials;

		private string soapAction;

		private bool acceptCompressedResponses;

		private string applicationName;

		private CookieContainer cookieContainer;

		private bool outdatedVersion;

		private WindowsIdentity logonWindowsIdentity;

		private string connectionSecureGroupName;

		private int timeoutMs;

		private string authorizationHeader;

		private AadTokenHolder aadTokenHolder;

		private string xmlaServerHeader;

		private X509Certificate2 clientCertificate;

		private bool allowAutoRedirect;

		private readonly HttpStream.RoutingTokenAccessor routingTokenAccessor;

		private string authorizedUserName;

		private string authorizedUserID;

		private string authorizedUserTenant;

		private string asdbAccessToken;

		private static readonly string recognizedContentTypes = string.Concat(new string[]
		{
			"(",
			"text/xml".Replace("+", "\\+"),
			")|(",
			"application/xml+xpress".Replace("+", "\\+"),
			")|(",
			"application/sx".Replace("+", "\\+"),
			" )|(",
			"application/sx+xpress".Replace("+", "\\+"),
			")"
		});

		private static readonly string contentRegExpr = "^(\\s*)(?<content_type>(" + HttpStream.recognizedContentTypes + "))(\\s*)((;(.*))|)(\\s*)\\z";

		private static readonly Regex contentTypeRegex = new Regex(HttpStream.contentRegExpr, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private XmlaStreamException streamException;

		private ConnectionInfo connectionInfo;

		private bool hasASAzureHeaders;

		private bool IsASAzure
		{
			get
			{
				return this.connectionInfo.IsAsAzure;
			}
		}

		private bool IsInternalASAzure
		{
			get
			{
				return this.connectionInfo.IsInternalASAzure;
			}
		}

		private Guid RootActivityID
		{
			get
			{
				try
				{
					object obj = CallContext.LogicalGetData("x-ms-root-activity-id");
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

		private Guid ParentActivityID
		{
			get
			{
				try
				{
					object obj = CallContext.LogicalGetData("x-ms-parent-activity-id");
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

		internal XmlaStreamException StreamException
		{
			get
			{
				return this.streamException;
			}
		}

		internal string RoutingToken
		{
			get
			{
				return this.routingTokenAccessor.Value;
			}
			set
			{
				this.routingTokenAccessor.Value = value;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.timeoutMs;
			}
			set
			{
				this.timeoutMs = value;
			}
		}

		internal HttpWebRequest HttpStreamRequest
		{
			get
			{
				return this.httpRequest;
			}
		}

		internal HttpWebResponse HttpStreamResponse
		{
			get
			{
				return this.httpResponse;
			}
		}

		internal HttpStream(Uri dataSourceUri, bool acceptCompressedResponses, DataType desiredRequestType, DataType desiredResponseType, CookieContainer cookieContainer, int timeoutMs, ConnectionInfo connectionInfo) : base(desiredRequestType, desiredResponseType)
		{
			try
			{
				this.webSite = dataSourceUri;
				this.hasASAzureHeaders = false;
				this.connectionInfo = connectionInfo;
				if (connectionInfo.IsInternalASAzure)
				{
					this.hasASAzureHeaders = true;
				}
				if (connectionInfo.IsAsAzure)
				{
					this.aadTokenHolder = connectionInfo.AadTokenHolder;
					this.xmlaServerHeader = connectionInfo.AsAzureServerName;
					this.hasASAzureHeaders = true;
				}
				else if (connectionInfo.IntegratedSecurity == IntegratedSecurity.Federated)
				{
					if (string.IsNullOrEmpty(connectionInfo.IdentityProvider) || string.Compare(connectionInfo.IdentityProvider, "MsoID", CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase) != 0)
					{
						throw new NotSupportedException(XmlaSR.ConnectionString_InvalidIdentityProviderForIntegratedSecurityFederated);
					}
					this.authorizationHeader = "MsoID " + MsoIDAuthenticationProvider.Instance.Authenticate(connectionInfo.UserID, connectionInfo.Password);
				}
				else if (connectionInfo.UserID == null)
				{
					this.logonWindowsIdentity = WindowsIdentity.GetCurrent();
					this.credentials = CredentialCache.DefaultCredentials;
					this.connectionSecureGroupName = this.logonWindowsIdentity.User.ToString();
				}
				else
				{
					this.credentials = new NetworkCredential(connectionInfo.UserID, connectionInfo.Password);
					SHA1Managed sHA1Managed = new SHA1Managed();
					byte[] bytes = sHA1Managed.ComputeHash(Encoding.UTF8.GetBytes(connectionInfo.Password));
					this.connectionSecureGroupName = connectionInfo.UserID.Replace(";", ";;") + ";:" + Encoding.Default.GetString(bytes).Replace(";", ";;");
				}
				if (!string.IsNullOrEmpty(connectionInfo.ClientCertificateThumbprint))
				{
					this.clientCertificate = CertUtils.LoadCertificateByThumbprint(connectionInfo.ClientCertificateThumbprint, true);
				}
				this.acceptCompressedResponses = acceptCompressedResponses;
				this.applicationName = connectionInfo.ApplicationName;
				this.cookieContainer = cookieContainer;
				this.timeoutMs = timeoutMs;
				this.routingTokenAccessor = new HttpStream.RoutingTokenAccessor(connectionInfo);
				this.allowAutoRedirect = connectionInfo.AllowAutoRedirect;
			}
			catch (UriFormatException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void Close()
		{
		}

		public override void Dispose()
		{
			try
			{
				if (this.httpRequest != null)
				{
					this.httpRequest.Abort();
					this.httpRequest = null;
				}
				if (this.httpRequestStream != null)
				{
					this.httpRequestStream.Close();
					this.httpRequestStream = null;
				}
				try
				{
					if (this.httpResponse != null)
					{
						this.httpResponse.Close();
						this.httpResponse = null;
					}
				}
				catch (IOException)
				{
				}
				catch (WebException)
				{
				}
				catch (ProtocolViolationException)
				{
				}
				catch (SecurityException)
				{
				}
				if (this.httpResponseStream != null)
				{
					this.httpResponseStream.Close();
					this.httpResponseStream = null;
				}
				if (this.logonWindowsIdentity != null)
				{
					this.logonWindowsIdentity.Dispose();
					this.logonWindowsIdentity = null;
					this.connectionSecureGroupName = null;
				}
				this.disposed = true;
			}
			finally
			{
				base.Dispose(true);
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (size + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer");
			}
			int result;
			try
			{
				this.GetResponseStream();
				int num = this.httpResponseStream.Read(buffer, offset, size);
				result = num;
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (WebException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			catch (ProtocolViolationException innerException3)
			{
				throw new XmlaStreamException(innerException3);
			}
			return result;
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				this.GetRequestStream();
				this.httpRequestStream.Write(buffer, offset, size);
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (WebException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			catch (SecurityException innerException3)
			{
				throw new XmlaStreamException(innerException3);
			}
			catch (ProtocolViolationException innerException4)
			{
				throw new XmlaStreamException(innerException4);
			}
		}

		public override void WriteEndOfMessage()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (this.httpRequestStream != null)
				{
					this.httpRequestStream.Flush();
					this.httpRequestStream.Close();
				}
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void WriteSoapActionHeader(string action)
		{
			this.soapAction = action;
		}

		public override string GetExtendedErrorInfo()
		{
			if (!this.IsASAzure || this.httpResponse == null)
			{
				return string.Empty;
			}
			return ASAzureUtility.GetExtendedErrorInfo(this.httpResponse);
		}

		public override void Skip()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				this.streamException = null;
				if (this.httpResponse != null)
				{
					this.httpResponse.Close();
					this.httpResponse = null;
					this.httpResponseStream = null;
				}
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
		}

		public override DataType GetResponseDataType()
		{
			DataType result;
			try
			{
				this.GetResponseStream();
				Match match = HttpStream.contentTypeRegex.Match(this.httpResponse.ContentType);
				if (!match.Success)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnsupportedDataFormat(this.httpResponse.ContentType), "");
				}
				DataType dataTypeFromString = DataTypes.GetDataTypeFromString(match.Groups["content_type"].Value);
				if (dataTypeFromString == DataType.Undetermined || dataTypeFromString == DataType.Unknown)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnsupportedDataFormat(this.httpResponse.ContentType), "");
				}
				result = dataTypeFromString;
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (WebException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			catch (ProtocolViolationException innerException3)
			{
				throw new XmlaStreamException(innerException3);
			}
			return result;
		}

		private void GetResponseStream()
		{
			if (this.outdatedVersion)
			{
				throw new AdomdConnectionException(XmlaSR.Connection_WorkbookIsOutdated);
			}
			if (this.httpResponseStream == null)
			{
				try
				{
					if (this.logonWindowsIdentity != null)
					{
						using (WindowsIdentity current = WindowsIdentity.GetCurrent())
						{
							if (this.logonWindowsIdentity.User != current.User)
							{
								//using (this.logonWindowsIdentity.Impersonate())
								//{
								//	this.httpResponse = (HttpWebResponse)this.httpRequest.GetResponse();
								//	goto IL_88;
								//}
							}
							this.httpResponse = (HttpWebResponse)this.httpRequest.GetResponse();
							IL_88:
							goto IL_AA;
						}
					}
					this.httpResponse = (HttpWebResponse)this.httpRequest.GetResponse();
					IL_AA:
					int statusCode = (int)this.httpResponse.StatusCode;
					if (statusCode >= 300 && statusCode <= 399)
					{
						ASAzureUtility.ClustersCache.Invalidate();
						throw new AdomdConnectionException(XmlaSR.Connection_AnalysisServicesInstanceWasMoved);
					}
					if (!this.hasASAzureHeaders)
					{
						string value = this.httpResponse.Headers["OutdatedVersion"];
						if (!string.IsNullOrEmpty(value))
						{
							this.outdatedVersion = true;
							throw new AdomdConnectionException(XmlaSR.Connection_WorkbookIsOutdated);
						}
					}
					if (!this.hasASAzureHeaders)
					{
						this.RoutingToken = this.httpResponse.Headers["X-AS-Routing"];
					}
					this.DetermineNegotiatedOptions();
				}
				catch (WebException ex)
				{
					if (ex.Response == null)
					{
						throw;
					}
					if (this.IsASAzure)
					{
						ASAzureUtility.ThrowConnectionException(ex);
					}
					HttpStatusCode statusCode2 = ((HttpWebResponse)ex.Response).StatusCode;
					if (HttpStatusCode.InternalServerError == statusCode2)
					{
						this.httpResponse = (HttpWebResponse)ex.Response;
						this.DetermineNegotiatedOptions();
						Match match = HttpStream.contentTypeRegex.Match(this.httpResponse.ContentType);
						if (!match.Success)
						{
							throw;
						}
						this.streamException = new XmlaStreamException(ex);
					}
					else
					{
						if (ex.Status == WebExceptionStatus.ProtocolError && statusCode2 == HttpStatusCode.Unauthorized)
						{
							throw new XmlaStreamException(ex, ConnectionExceptionCause.AuthenticationFailed);
						}
						throw;
					}
				}
				finally
				{
					this.httpRequestStream = null;
				}
				this.httpResponseStream = new BufferedStream(this.httpResponse.GetResponseStream(), XmlaClient.HttpStreamBufferSize);
			}
		}

		private void GetRequestStream()
		{
			if (this.httpRequestStream == null)
			{
				this.httpRequest = (HttpWebRequest)WebRequest.Create(this.webSite);
				this.httpRequest.ServicePoint.SetTcpKeepAlive(true, 30000, 30000);
				this.httpRequest.Method = "POST";
				this.httpRequest.Credentials = this.credentials;
				this.httpRequest.AllowAutoRedirect = this.allowAutoRedirect;
				this.httpRequest.UserAgent = "ADOMD.NET";
				this.httpRequest.ContentType = DataTypes.GetDataTypeFromEnum(this.GetRequestDataType());
				this.httpRequest.SendChunked = true;
				this.httpRequest.KeepAlive = true;
				this.httpRequest.UnsafeAuthenticatedConnectionSharing = true;
				this.httpRequest.ConnectionGroupName = this.connectionSecureGroupName;
				this.httpRequest.AutomaticDecompression = (this.acceptCompressedResponses ? DecompressionMethods.GZip : DecompressionMethods.None);
				this.httpRequest.Timeout = this.timeoutMs;
				this.httpRequest.CookieContainer = this.cookieContainer;
				if (this.clientCertificate != null)
				{
					this.httpRequest.ClientCertificates.Add(this.clientCertificate);
				}
				if (!this.hasASAzureHeaders && this.soapAction != null)
				{
					this.httpRequest.Headers.Add(this.soapAction);
				}
				this.httpRequest.Headers.Add(this.hasASAzureHeaders ? "x-ms-xmlacaps-negotiation-flags" : "X-Transport-Caps-Negotiation-Flags", base.GetTransportCapabilitiesString());
				if (!this.hasASAzureHeaders && !string.IsNullOrEmpty(this.applicationName))
				{
					this.httpRequest.Headers.Add("SspropInitAppName", this.applicationName);
				}
				if (!string.IsNullOrEmpty(base.SessionID))
				{
					this.httpRequest.Headers.Add(this.hasASAzureHeaders ? "x-ms-xmlasession-id" : "X-AS-SessionID", base.SessionID);
				}
				if (!this.hasASAzureHeaders && base.IsSessionTokenNeeded)
				{
					this.httpRequest.Headers.Add("X-AS-GetSessionToken", true.ToString());
				}
				else
				{
					this.httpRequest.Headers.Remove("X-AS-GetSessionToken");
				}
				if (!this.IsInternalASAzure && !object.Equals(this.ActivityID, Guid.Empty))
				{
					this.httpRequest.Headers.Add(this.hasASAzureHeaders ? "x-ms-parent-activity-id" : "X-AS-ActivityID", this.ActivityID.ToString());
				}
				if (!this.hasASAzureHeaders && !object.Equals(this.RequestID, Guid.Empty))
				{
					this.httpRequest.Headers.Add("X-AS-RequestID", this.RequestID.ToString());
				}
				if (!this.hasASAzureHeaders && !object.Equals(this.CurrentActivityID, Guid.Empty))
				{
					this.httpRequest.Headers.Add("X-AS-CurrentActivityID", this.CurrentActivityID.ToString());
				}
				if (!string.IsNullOrEmpty(this.authorizationHeader))
				{
					this.httpRequest.Headers.Add("Authorization", this.authorizationHeader);
				}
				else if (this.aadTokenHolder != null)
				{
					this.httpRequest.Headers.Add("Authorization", "Bearer " + this.aadTokenHolder.GetValidAccessToken());
				}
				if (this.IsASAzure)
				{
					if (!this.hasASAzureHeaders)
					{
						throw new ArgumentOutOfRangeException("hasASAzureHeaders");
					}
					if (string.IsNullOrEmpty(this.xmlaServerHeader))
					{
						throw new ArgumentOutOfRangeException("xmlaServerHeader");
					}
					this.httpRequest.Headers.Add("x-ms-xmlaserver", this.xmlaServerHeader);
				}
				if (this.IsASAzure)
				{
					string generalInfoHeaderValue = ASAzureUtility.GetGeneralInfoHeaderValue(this.connectionInfo.UseAdalCache, this.connectionInfo.UserID);
					this.httpRequest.Headers.Add("x-ms-xmlaapp-general-info", generalInfoHeaderValue);
				}
				if (this.IsInternalASAzure)
				{
					if (this.ParentActivityID == Guid.Empty)
					{
						throw new ArgumentNullException(string.Format("{0} not set on call context for internal AS Azure usage.", "x-ms-parent-activity-id"));
					}
					if (this.RootActivityID == Guid.Empty)
					{
						throw new ArgumentNullException(string.Format("{0} not set on call context for internal AS Azure usage.", "x-ms-root-activity-id"));
					}
					this.httpRequest.Headers.Add("x-ms-root-activity-id", this.RootActivityID.ToString());
					this.httpRequest.Headers.Add("x-ms-parent-activity-id", this.ParentActivityID.ToString());
				}
				if (!this.hasASAzureHeaders && !string.IsNullOrEmpty(this.authorizedUserName))
				{
					this.httpRequest.Headers.Add("X-AS-AuthorizedUserName", this.authorizedUserName);
				}
				if (!this.hasASAzureHeaders && !string.IsNullOrEmpty(this.authorizedUserID))
				{
					this.httpRequest.Headers.Add("X-AS-AuthorizedUserID", this.authorizedUserID);
				}
				if (!this.hasASAzureHeaders && !string.IsNullOrEmpty(this.authorizedUserTenant))
				{
					this.httpRequest.Headers.Add("X-AS-AuthorizedUserTenant", this.authorizedUserTenant);
				}
				if (!this.hasASAzureHeaders && !string.IsNullOrEmpty(this.asdbAccessToken))
				{
					this.httpRequest.Headers.Add("X-SAAS-DatabaseAccessToken", this.asdbAccessToken);
				}
				if (!this.hasASAzureHeaders && !string.IsNullOrEmpty(this.RoutingToken))
				{
					this.httpRequest.Headers.Add("X-AS-Routing", this.RoutingToken);
				}
				this.httpRequestStream = new BufferedStream(this.httpRequest.GetRequestStream(), XmlaClient.HttpStreamBufferSize);
			}
		}

		protected override void DetermineNegotiatedOptions()
		{
			if (!base.NegotiatedOptions)
			{
				TransportCapabilities transportCapabilities = new TransportCapabilities();
				string text = this.httpResponse.Headers.Get(this.hasASAzureHeaders ? "x-ms-xmlacaps-negotiation-flags" : "X-Transport-Caps-Negotiation-Flags");
				if (!string.IsNullOrEmpty(text))
				{
					transportCapabilities.FromString(text);
				}
				else if (this.hasASAzureHeaders)
				{
					throw new ArgumentOutOfRangeException("x-ms-xmlacaps-negotiation-flags");
				}
				transportCapabilities.ContentTypeNegotiated = true;
				base.SetTransportCapabilities(transportCapabilities);
			}
		}
	}
}
