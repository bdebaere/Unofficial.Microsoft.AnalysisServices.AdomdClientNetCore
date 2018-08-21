using Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class XmlaClient
	{
		private class Lock
		{
		}

		internal const string IntegrationDllStrongName = "Microsoft.AnalysisServices.SharePoint.Integration, PublicKeyToken=89845dcd8080cc91, Culture=neutral, Version=13.0.0.0";

		internal const string WCFXmlaClientClassName = "Microsoft.AnalysisServices.SharePoint.Integration.WCFXmlaClient";

		internal const string OnPremisesExternalChannelManifest = "Microsoft.DataProxy.NativeClient.dll.manifest";

		private const bool BeginSessionOnConnect = true;

		private const bool EndSessionOnDisconnect = true;

		internal const int ReadStreamBufferSize = 4096;

		internal const int MaxHttpConnections = 1000;

		internal const string LocaleIdentifierName = "LocaleIdentifier";

		private const string PropertyListName = "PropertyList";

		protected const string ActivityIDPropertyName = "DbpropMsmdActivityID";

		protected const string RequestIDPropertyName = "DbpropMsmdRequestID";

		protected const string CurrentActivityIDPropertyName = "DbpropMsmdCurrentActivityID";

		protected const string RequestMemoryLimitPropertyName = "DbPropmsmdRequestMemoryLimit";

		private const string VersionSequenceNumber = "500";

		private static IASSPClientProxy spProxy;

		private static Type wcfXmlaClientType;

		private static object clientInstance;

		protected internal static readonly TraceSwitch TRACESWITCH;

		internal static int HttpStreamBufferSize;

		internal static int BufferSizeForNetworkStream;

		private static int EndSessionTimeout;

		private string sessionID;

		private Guid activityID = Guid.Empty;

		private bool connected;

		internal bool captureXml;

		private StringCollection captureLog;

		private BufferedStream networkStream;

		internal XmlaStream xmlaStream;

		private TcpClient tcpClient;

		private ConnectionInfo connInfo;

		private IdentityTransferToken identityTransferToken;

		internal XmlTextWriter writer;

		internal XmlReader reader;

		internal StringWriter logEntry;

		private NamespacesMgr namespacesManager;

		private NameTable nameTable;

		private ConnectionState connectionState;

		private readonly XmlaClient.Lock lockForCloseAll = new XmlaClient.Lock();

		private IWorkbookSession workbookSession;

		private bool userOpened;

		private CookieContainer cookieContainer;

		private bool supportsActivityIDAndRequestID;

		private bool supportsCurrentActivityID;

		private bool isCompressionEnabled;

		private static readonly byte[] preSessionID;

		private static readonly byte[] preDatabaseName;

		private static readonly byte[] preDatabaseID;

		private static readonly byte[] preReadWriteMode;

		private static readonly byte[] preAbfContent;

		private static readonly byte[] localeBegin;

		private static readonly byte[] localeEnd;

		private static readonly byte[] activityIDBegin;

		private static readonly byte[] activityIDEnd;

		private static readonly byte[] requestIDBegin;

		private static readonly byte[] requestIDEnd;

		private static readonly byte[] currentActivityIDBegin;

		private static readonly byte[] currentActivityIDEnd;

		private static readonly byte[] endContent;

		internal static IASSPClientProxy SPProxy
		{
			get
			{
				if (Interlocked.CompareExchange<IASSPClientProxy>(ref XmlaClient.spProxy, null, null) == null)
				{
					IASSPClientProxy value = ASSPClientProxyFactory.CreateProxy();
					Interlocked.CompareExchange<IASSPClientProxy>(ref XmlaClient.spProxy, value, null);
				}
				return XmlaClient.spProxy;
			}
		}

		internal bool IsConnected
		{
			get
			{
				return this.connected;
			}
		}

		public bool IsCompressionEnabled
		{
			get
			{
				return this.isCompressionEnabled;
			}
			set
			{
				this.isCompressionEnabled = value;
				if (this.xmlaStream != null)
				{
					this.xmlaStream.IsCompressionEnabled = value;
				}
			}
		}

		public string SessionID
		{
			get
			{
				return this.sessionID;
			}
			set
			{
				this.sessionID = value;
				if (this.xmlaStream != null)
				{
					this.xmlaStream.SessionID = this.sessionID;
				}
			}
		}

		public ConnectionInfo ConnectionInfo
		{
			get
			{
				return this.connInfo;
			}
		}

		public IdentityTransferToken IdentityTransferToken
		{
			get
			{
				return this.identityTransferToken;
			}
			set
			{
				this.identityTransferToken = value;
			}
		}

		internal bool SupportsActivityIDAndRequestID
		{
			get
			{
				return this.supportsActivityIDAndRequestID;
			}
			set
			{
				this.supportsActivityIDAndRequestID = value;
			}
		}

		internal bool SupportsCurrentActivityID
		{
			get
			{
				return this.supportsCurrentActivityID;
			}
		}

		internal bool IsReaderDetached
		{
			get
			{
				XmlaReader xmlaReader = this.reader as XmlaReader;
				return xmlaReader != null && xmlaReader.IsReaderDetached;
			}
		}

		private void OpenIXMLAConnection(ConnectionInfo connectionInfo)
		{
			try
			{
				this.connInfo = connectionInfo;
				this.xmlaStream = new IXMLAStream();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.CannotConnect, innerException);
			}
			catch (XmlException innerException2)
			{
				this.CloseAll();
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException2);
			}
			catch
			{
				this.CloseAll();
				throw;
			}
			this.connected = true;
		}

		internal XmlReader DiscoverWithCreateSession(string discoverType, ListDictionary properties, bool sendNamespacesCompatibility)
		{
			this.CheckConnection();
			try
			{
				this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Discover\"", true, sendNamespacesCompatibility, false);
				this.WriteStartDiscover(discoverType, null);
				this.WriteRestrictions(null);
				this.WriteEndDiscover(properties);
				this.EndMessage();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			return XmlaClient.GetReaderToReturnToPublic(this.SendMessage(true, true, sendNamespacesCompatibility));
		}

		internal static void ReadExecuteResponse(XmlReader reader)
		{
			XmlaResultCollection xmlaResultCollection = new XmlaResultCollection();
			XmlaResult xmlaResult = new XmlaResult();
			XmlaClient.ReadExecuteResponsePrivate(reader, true, xmlaResultCollection, xmlaResult);
			if (xmlaResultCollection.ContainsErrors)
			{
				throw XmlaResultCollection.ExceptionOnError(xmlaResultCollection);
			}
		}

		internal static void ReadMultipleResults(XmlReader reader)
		{
			XmlaResultCollection xmlaResultCollection = new XmlaResultCollection();
			XmlaClient.ReadMultipleResults(reader, xmlaResultCollection, true);
			if (xmlaResultCollection.ContainsErrors)
			{
				throw XmlaResultCollection.ExceptionOnError(xmlaResultCollection);
			}
		}

		internal XmlReader ExecuteStream(Stream stream, IDictionary connectionProperties, IDictionary commandProperties, IDataParameterCollection parameters, bool appendStatementTags)
		{
			this.CheckConnection();
			try
			{
				if (!this.captureXml)
				{
					this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"");
					this.WriteStartCommand(ref commandProperties);
					if (appendStatementTags)
					{
						this.writer.WriteStartElement("Statement");
					}
					StreamReader streamReader = new StreamReader(stream);
					char[] buffer = new char[4096];
					int count;
					while ((count = streamReader.Read(buffer, 0, 4096)) > 0)
					{
						this.writer.WriteRaw(buffer, 0, count);
					}
					if (appendStatementTags)
					{
						this.writer.WriteEndElement();
					}
					this.WriteEndCommand(connectionProperties, commandProperties, parameters);
					this.EndMessage();
				}
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			if (!this.captureXml)
			{
				return XmlaClient.GetReaderToReturnToPublic(this.SendMessage(true, false, false));
			}
			return null;
		}

		internal XmlReader Discover(string requestType, ListDictionary properties, IDictionary restrictions)
		{
			return XmlaClient.GetReaderToReturnToPublic(this.Discover(requestType, null, properties, restrictions, false, null));
		}

		internal XmlReader Discover(string requestType, string requestNamespace, ListDictionary properties, IDictionary restrictions)
		{
			return this.Discover(requestType, requestNamespace, properties, restrictions, null);
		}

		internal XmlReader Discover(string requestType, string requestNamespace, ListDictionary properties, IDictionary restrictions, IDictionary requestProperties)
		{
			return XmlaClient.GetReaderToReturnToPublic(this.Discover(requestType, requestNamespace, properties, restrictions, false, requestProperties));
		}

		internal XmlReader Discover(string requestType, ListDictionary properties, IDictionary restrictions, bool sendNamespacesCompatibility)
		{
			return XmlaClient.GetReaderToReturnToPublic(this.Discover(requestType, null, properties, restrictions, sendNamespacesCompatibility, null));
		}

		internal static bool IsTypeSupportedForParameters(Type type)
		{
			return typeof(IDataReader).IsAssignableFrom(type) || typeof(DataTable).IsAssignableFrom(type) || XmlTypeMapper.IsTypeSupported(type);
		}

		internal XmlReader ExecuteStatement(string statement, IDictionary connectionProperties, IDictionary commandProperties, IDataParameterCollection parameters, bool isMdx)
		{
			this.CheckConnection();
			try
			{
				this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"");
				this.WriteStartCommand(ref commandProperties);
				if (isMdx)
				{
					this.WriteStatement(statement);
				}
				else
				{
					this.WriteCommandContent(statement);
				}
				this.WriteEndCommand(connectionProperties, commandProperties, parameters);
				this.EndMessage();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			return XmlaClient.GetReaderToReturnToPublic(this.SendMessage(true, false, false));
		}

		internal static object InvokeMemberFromWCFXmlaClientClass(string methodName, object[] invokeArgs)
		{
			if (XmlaClient.wcfXmlaClientType == null || XmlaClient.clientInstance == null)
			{
				Assembly assembly = Assembly.Load("Microsoft.AnalysisServices.SharePoint.Integration, PublicKeyToken=89845dcd8080cc91, Culture=neutral, Version=13.0.0.0");
				XmlaClient.wcfXmlaClientType = assembly.GetType("Microsoft.AnalysisServices.SharePoint.Integration.WCFXmlaClient", true, true);
				XmlaClient.clientInstance = Activator.CreateInstance(XmlaClient.wcfXmlaClientType);
			}
			return XmlaClient.wcfXmlaClientType.InvokeMember(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, XmlaClient.clientInstance, invokeArgs, CultureInfo.InvariantCulture);
		}

		internal void WCFConnect(string dataSource, string dataSourceVersion, ref string timeLastModified, ref string databaseId, ref string databaseName, ref string loginName, ref string serverEndpointAddress)
		{
			try
			{
				object[] array = new object[]
				{
					dataSource,
					dataSourceVersion,
					timeLastModified,
					databaseId,
					databaseName,
					loginName,
					serverEndpointAddress
				};
				XmlaClient.InvokeMemberFromWCFXmlaClientClass("WCFConnect", array);
				timeLastModified = Convert.ToString(array[2], CultureInfo.InvariantCulture);
				databaseId = Convert.ToString(array[3], CultureInfo.InvariantCulture);
				databaseName = Convert.ToString(array[4], CultureInfo.InvariantCulture);
				loginName = Convert.ToString(array[5], CultureInfo.InvariantCulture);
				serverEndpointAddress = Convert.ToString(array[6], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.InnerException == null)
				{
					throw new AdomdConnectionException(XmlaSR.CannotConnect);
				}
				throw new AdomdConnectionException(XmlaSR.CannotConnect, ex.InnerException);
			}
		}

		internal static void UlsWriterSetCurrentRequestCategoryToRequestProcessing()
		{
			XmlaClient.InvokeMemberFromWCFXmlaClientClass("UlsWriterSetCurrentRequestCategoryToRequestProcessing", null);
		}

		internal static void UlsWriterLogException(Exception e)
		{
			try
			{
				object[] invokeArgs = new object[]
				{
					e
				};
				XmlaClient.InvokeMemberFromWCFXmlaClientClass("UlsWriterLogException", invokeArgs);
			}
			catch
			{
			}
		}

		internal static IDisposable CreateSPSite(string dataSource)
		{
			object[] invokeArgs = new object[]
			{
				dataSource
			};
			return (IDisposable)XmlaClient.InvokeMemberFromWCFXmlaClientClass("CreateSPSite", invokeArgs);
		}

		internal static Stream GetResponseStreamHelper(object spSite, Stream inStream, string serverEndpointAddress, string loginName, string databaseId, bool specificVersion, bool isFirstRequest, string userAgent, string applicationName, string userAddress, string requestFlags, string requestDataType, ref string responseFlags, ref string responseDataType, ref bool outdatedVersion)
		{
			object[] array = new object[]
			{
				spSite,
				inStream,
				serverEndpointAddress,
				loginName,
				databaseId,
				specificVersion,
				isFirstRequest,
				userAgent,
				applicationName,
				userAddress,
				requestFlags,
				requestDataType,
				responseFlags,
				responseDataType,
				outdatedVersion
			};
			Stream result = (Stream)XmlaClient.InvokeMemberFromWCFXmlaClientClass("GetResponseStreamHelper", array);
			responseFlags = Convert.ToString(array[12], CultureInfo.InvariantCulture);
			responseDataType = Convert.ToString(array[13], CultureInfo.InvariantCulture);
			outdatedVersion = Convert.ToBoolean(array[14], CultureInfo.InvariantCulture);
			return result;
		}

		static XmlaClient()
		{
			XmlaClient.spProxy = null;
			XmlaClient.wcfXmlaClientType = null;
			XmlaClient.clientInstance = null;
			XmlaClient.TRACESWITCH = new TraceSwitch(typeof(XmlaClient).FullName, typeof(XmlaClient).FullName, TraceLevel.Off.ToString());
			XmlaClient.HttpStreamBufferSize = 131072;
			XmlaClient.BufferSizeForNetworkStream = 8192;
			XmlaClient.EndSessionTimeout = 30000;
			XmlaClient.preSessionID = new byte[]
			{
				223,
				255,
				1,
				176,
				4,
				254,
				3,
				49,
				0,
				46,
				0,
				48,
				0,
				253,
				6,
				85,
				0,
				84,
				0,
				70,
				0,
				45,
				0,
				49,
				0,
				54,
				0,
				0,
				240,
				41,
				104,
				0,
				116,
				0,
				116,
				0,
				112,
				0,
				58,
				0,
				47,
				0,
				47,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				46,
				0,
				120,
				0,
				109,
				0,
				108,
				0,
				115,
				0,
				111,
				0,
				97,
				0,
				112,
				0,
				46,
				0,
				111,
				0,
				114,
				0,
				103,
				0,
				47,
				0,
				115,
				0,
				111,
				0,
				97,
				0,
				112,
				0,
				47,
				0,
				101,
				0,
				110,
				0,
				118,
				0,
				101,
				0,
				108,
				0,
				111,
				0,
				112,
				0,
				101,
				0,
				47,
				0,
				240,
				8,
				69,
				0,
				110,
				0,
				118,
				0,
				101,
				0,
				108,
				0,
				111,
				0,
				112,
				0,
				101,
				0,
				239,
				1,
				0,
				2,
				248,
				1,
				240,
				5,
				120,
				0,
				109,
				0,
				108,
				0,
				110,
				0,
				115,
				0,
				239,
				0,
				3,
				0,
				246,
				2,
				14,
				41,
				104,
				0,
				116,
				0,
				116,
				0,
				112,
				0,
				58,
				0,
				47,
				0,
				47,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				46,
				0,
				120,
				0,
				109,
				0,
				108,
				0,
				115,
				0,
				111,
				0,
				97,
				0,
				112,
				0,
				46,
				0,
				111,
				0,
				114,
				0,
				103,
				0,
				47,
				0,
				115,
				0,
				111,
				0,
				97,
				0,
				112,
				0,
				47,
				0,
				101,
				0,
				110,
				0,
				118,
				0,
				101,
				0,
				108,
				0,
				111,
				0,
				112,
				0,
				101,
				0,
				47,
				0,
				240,
				6,
				72,
				0,
				101,
				0,
				97,
				0,
				100,
				0,
				101,
				0,
				114,
				0,
				239,
				1,
				0,
				4,
				245,
				248,
				3,
				240,
				38,
				117,
				0,
				114,
				0,
				110,
				0,
				58,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				45,
				0,
				109,
				0,
				105,
				0,
				99,
				0,
				114,
				0,
				111,
				0,
				115,
				0,
				111,
				0,
				102,
				0,
				116,
				0,
				45,
				0,
				99,
				0,
				111,
				0,
				109,
				0,
				58,
				0,
				120,
				0,
				109,
				0,
				108,
				0,
				45,
				0,
				97,
				0,
				110,
				0,
				97,
				0,
				108,
				0,
				121,
				0,
				115,
				0,
				105,
				0,
				115,
				0,
				240,
				1,
				120,
				0,
				240,
				7,
				83,
				0,
				101,
				0,
				115,
				0,
				115,
				0,
				105,
				0,
				111,
				0,
				110,
				0,
				239,
				5,
				6,
				7,
				248,
				4,
				240,
				7,
				120,
				0,
				109,
				0,
				108,
				0,
				110,
				0,
				115,
				0,
				58,
				0,
				120,
				0,
				239,
				0,
				8,
				0,
				246,
				5,
				14,
				38,
				117,
				0,
				114,
				0,
				110,
				0,
				58,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				45,
				0,
				109,
				0,
				105,
				0,
				99,
				0,
				114,
				0,
				111,
				0,
				115,
				0,
				111,
				0,
				102,
				0,
				116,
				0,
				45,
				0,
				99,
				0,
				111,
				0,
				109,
				0,
				58,
				0,
				120,
				0,
				109,
				0,
				108,
				0,
				45,
				0,
				97,
				0,
				110,
				0,
				97,
				0,
				108,
				0,
				121,
				0,
				115,
				0,
				105,
				0,
				115,
				0,
				240,
				9,
				83,
				0,
				101,
				0,
				115,
				0,
				115,
				0,
				105,
				0,
				111,
				0,
				110,
				0,
				73,
				0,
				100,
				0,
				239,
				0,
				0,
				9,
				246,
				6
			};
			XmlaClient.preDatabaseName = new byte[]
			{
				240,
				14,
				109,
				0,
				117,
				0,
				115,
				0,
				116,
				0,
				85,
				0,
				110,
				0,
				100,
				0,
				101,
				0,
				114,
				0,
				115,
				0,
				116,
				0,
				97,
				0,
				110,
				0,
				100,
				0,
				239,
				1,
				0,
				10,
				246,
				7,
				14,
				1,
				49,
				0,
				245,
				247,
				247,
				240,
				4,
				66,
				0,
				111,
				0,
				100,
				0,
				121,
				0,
				239,
				1,
				0,
				11,
				248,
				8,
				240,
				7,
				69,
				0,
				120,
				0,
				101,
				0,
				99,
				0,
				117,
				0,
				116,
				0,
				101,
				0,
				239,
				5,
				0,
				12,
				248,
				9,
				246,
				2,
				14,
				38,
				117,
				0,
				114,
				0,
				110,
				0,
				58,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				45,
				0,
				109,
				0,
				105,
				0,
				99,
				0,
				114,
				0,
				111,
				0,
				115,
				0,
				111,
				0,
				102,
				0,
				116,
				0,
				45,
				0,
				99,
				0,
				111,
				0,
				109,
				0,
				58,
				0,
				120,
				0,
				109,
				0,
				108,
				0,
				45,
				0,
				97,
				0,
				110,
				0,
				97,
				0,
				108,
				0,
				121,
				0,
				115,
				0,
				105,
				0,
				115,
				0,
				240,
				7,
				67,
				0,
				111,
				0,
				109,
				0,
				109,
				0,
				97,
				0,
				110,
				0,
				100,
				0,
				239,
				5,
				0,
				13,
				245,
				248,
				10,
				240,
				57,
				104,
				0,
				116,
				0,
				116,
				0,
				112,
				0,
				58,
				0,
				47,
				0,
				47,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				46,
				0,
				109,
				0,
				105,
				0,
				99,
				0,
				114,
				0,
				111,
				0,
				115,
				0,
				111,
				0,
				102,
				0,
				116,
				0,
				46,
				0,
				99,
				0,
				111,
				0,
				109,
				0,
				47,
				0,
				97,
				0,
				110,
				0,
				97,
				0,
				108,
				0,
				121,
				0,
				115,
				0,
				105,
				0,
				115,
				0,
				115,
				0,
				101,
				0,
				114,
				0,
				118,
				0,
				105,
				0,
				99,
				0,
				101,
				0,
				115,
				0,
				47,
				0,
				50,
				0,
				48,
				0,
				48,
				0,
				51,
				0,
				47,
				0,
				101,
				0,
				110,
				0,
				103,
				0,
				105,
				0,
				110,
				0,
				101,
				0,
				240,
				9,
				73,
				0,
				109,
				0,
				97,
				0,
				103,
				0,
				101,
				0,
				76,
				0,
				111,
				0,
				97,
				0,
				100,
				0,
				239,
				14,
				0,
				15,
				248,
				11,
				246,
				2,
				14,
				57,
				104,
				0,
				116,
				0,
				116,
				0,
				112,
				0,
				58,
				0,
				47,
				0,
				47,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				46,
				0,
				109,
				0,
				105,
				0,
				99,
				0,
				114,
				0,
				111,
				0,
				115,
				0,
				111,
				0,
				102,
				0,
				116,
				0,
				46,
				0,
				99,
				0,
				111,
				0,
				109,
				0,
				47,
				0,
				97,
				0,
				110,
				0,
				97,
				0,
				108,
				0,
				121,
				0,
				115,
				0,
				105,
				0,
				115,
				0,
				115,
				0,
				101,
				0,
				114,
				0,
				118,
				0,
				105,
				0,
				99,
				0,
				101,
				0,
				115,
				0,
				47,
				0,
				50,
				0,
				48,
				0,
				48,
				0,
				51,
				0,
				47,
				0,
				101,
				0,
				110,
				0,
				103,
				0,
				105,
				0,
				110,
				0,
				101,
				0,
				240,
				14,
				65,
				0,
				108,
				0,
				108,
				0,
				111,
				0,
				119,
				0,
				79,
				0,
				118,
				0,
				101,
				0,
				114,
				0,
				119,
				0,
				114,
				0,
				105,
				0,
				116,
				0,
				101,
				0,
				239,
				0,
				0,
				16,
				246,
				12,
				14,
				4,
				116,
				0,
				114,
				0,
				117,
				0,
				101,
				0,
				240,
				12,
				68,
				0,
				97,
				0,
				116,
				0,
				97,
				0,
				98,
				0,
				97,
				0,
				115,
				0,
				101,
				0,
				78,
				0,
				97,
				0,
				109,
				0,
				101,
				0,
				239,
				14,
				0,
				17,
				245,
				248,
				13
			};
			XmlaClient.preDatabaseID = new byte[]
			{
				247,
				240,
				10,
				68,
				0,
				97,
				0,
				116,
				0,
				97,
				0,
				98,
				0,
				97,
				0,
				115,
				0,
				101,
				0,
				73,
				0,
				68,
				0,
				239,
				14,
				0,
				18,
				248,
				14
			};
			XmlaClient.preReadWriteMode = new byte[]
			{
				247,
				240,
				61,
				104,
				0,
				116,
				0,
				116,
				0,
				112,
				0,
				58,
				0,
				47,
				0,
				47,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				46,
				0,
				109,
				0,
				105,
				0,
				99,
				0,
				114,
				0,
				111,
				0,
				115,
				0,
				111,
				0,
				102,
				0,
				116,
				0,
				46,
				0,
				99,
				0,
				111,
				0,
				109,
				0,
				47,
				0,
				97,
				0,
				110,
				0,
				97,
				0,
				108,
				0,
				121,
				0,
				115,
				0,
				105,
				0,
				115,
				0,
				115,
				0,
				101,
				0,
				114,
				0,
				118,
				0,
				105,
				0,
				99,
				0,
				101,
				0,
				115,
				0,
				47,
				0,
				50,
				0,
				48,
				0,
				48,
				0,
				56,
				0,
				47,
				0,
				101,
				0,
				110,
				0,
				103,
				0,
				105,
				0,
				110,
				0,
				101,
				0,
				47,
				0,
				49,
				0,
				48,
				0,
				48,
				0,
				240,
				13,
				82,
				0,
				101,
				0,
				97,
				0,
				100,
				0,
				87,
				0,
				114,
				0,
				105,
				0,
				116,
				0,
				101,
				0,
				77,
				0,
				111,
				0,
				100,
				0,
				101,
				0,
				239,
				19,
				0,
				20,
				248,
				15,
				246,
				2,
				14,
				61,
				104,
				0,
				116,
				0,
				116,
				0,
				112,
				0,
				58,
				0,
				47,
				0,
				47,
				0,
				115,
				0,
				99,
				0,
				104,
				0,
				101,
				0,
				109,
				0,
				97,
				0,
				115,
				0,
				46,
				0,
				109,
				0,
				105,
				0,
				99,
				0,
				114,
				0,
				111,
				0,
				115,
				0,
				111,
				0,
				102,
				0,
				116,
				0,
				46,
				0,
				99,
				0,
				111,
				0,
				109,
				0,
				47,
				0,
				97,
				0,
				110,
				0,
				97,
				0,
				108,
				0,
				121,
				0,
				115,
				0,
				105,
				0,
				115,
				0,
				115,
				0,
				101,
				0,
				114,
				0,
				118,
				0,
				105,
				0,
				99,
				0,
				101,
				0,
				115,
				0,
				47,
				0,
				50,
				0,
				48,
				0,
				48,
				0,
				56,
				0,
				47,
				0,
				101,
				0,
				110,
				0,
				103,
				0,
				105,
				0,
				110,
				0,
				101,
				0,
				47,
				0,
				49,
				0,
				48,
				0,
				48,
				0,
				245
			};
			XmlaClient.preAbfContent = new byte[]
			{
				247,
				240,
				4,
				68,
				0,
				97,
				0,
				116,
				0,
				97,
				0,
				239,
				14,
				0,
				21,
				248,
				16,
				240,
				9,
				68,
				0,
				97,
				0,
				116,
				0,
				97,
				0,
				66,
				0,
				108,
				0,
				111,
				0,
				99,
				0,
				107,
				0,
				239,
				14,
				0,
				22,
				248,
				17
			};
			XmlaClient.localeBegin = new byte[]
			{
				247,
				247,
				247,
				247,
				240,
				10,
				80,
				0,
				114,
				0,
				111,
				0,
				112,
				0,
				101,
				0,
				114,
				0,
				116,
				0,
				105,
				0,
				101,
				0,
				115,
				0,
				239,
				5,
				0,
				23,
				248,
				18,
				240,
				12,
				80,
				0,
				114,
				0,
				111,
				0,
				112,
				0,
				101,
				0,
				114,
				0,
				116,
				0,
				121,
				0,
				76,
				0,
				105,
				0,
				115,
				0,
				116,
				0,
				239,
				5,
				0,
				24,
				248,
				19,
				240,
				16,
				76,
				0,
				111,
				0,
				99,
				0,
				97,
				0,
				108,
				0,
				101,
				0,
				73,
				0,
				100,
				0,
				101,
				0,
				110,
				0,
				116,
				0,
				105,
				0,
				102,
				0,
				105,
				0,
				101,
				0,
				114,
				0,
				239,
				5,
				0,
				25,
				248,
				20
			};
			XmlaClient.localeEnd = new byte[]
			{
				247
			};
			XmlaClient.activityIDBegin = new byte[]
			{
				240,
				20,
				68,
				0,
				98,
				0,
				112,
				0,
				114,
				0,
				111,
				0,
				112,
				0,
				77,
				0,
				115,
				0,
				109,
				0,
				100,
				0,
				65,
				0,
				99,
				0,
				116,
				0,
				105,
				0,
				118,
				0,
				105,
				0,
				116,
				0,
				121,
				0,
				73,
				0,
				68,
				0,
				239,
				5,
				0,
				26,
				248,
				21
			};
			XmlaClient.activityIDEnd = new byte[]
			{
				247
			};
			XmlaClient.requestIDBegin = new byte[]
			{
				240,
				19,
				68,
				0,
				98,
				0,
				112,
				0,
				114,
				0,
				111,
				0,
				112,
				0,
				77,
				0,
				115,
				0,
				109,
				0,
				100,
				0,
				82,
				0,
				101,
				0,
				113,
				0,
				117,
				0,
				101,
				0,
				115,
				0,
				116,
				0,
				73,
				0,
				68,
				0,
				239,
				5,
				0,
				27,
				248,
				22
			};
			XmlaClient.requestIDEnd = new byte[]
			{
				247
			};
			XmlaClient.currentActivityIDBegin = new byte[]
			{
				240,
				27,
				68,
				0,
				98,
				0,
				112,
				0,
				114,
				0,
				111,
				0,
				112,
				0,
				77,
				0,
				115,
				0,
				109,
				0,
				100,
				0,
				67,
				0,
				117,
				0,
				114,
				0,
				114,
				0,
				101,
				0,
				110,
				0,
				116,
				0,
				65,
				0,
				99,
				0,
				116,
				0,
				105,
				0,
				118,
				0,
				105,
				0,
				116,
				0,
				121,
				0,
				73,
				0,
				68,
				0,
				239,
				5,
				0,
				28,
				248,
				23
			};
			XmlaClient.currentActivityIDEnd = new byte[]
			{
				247
			};
			XmlaClient.endContent = new byte[]
			{
				247,
				247,
				247,
				247,
				247
			};
			ServicePointManager.DefaultConnectionLimit = 1000;
		}

		public XmlaClient() : this(new StringCollection())
		{
			this.ReadSettingsFromConfiguration();
		}

		private void ReadSettingsFromConfiguration()
		{
			int num = 100;
			//if (this.TryGetConfigValue<int>("AS_HttpStreamBufferSize", out num, (int val) => val >= 0 && val <= 10240))
			//{
				XmlaClient.HttpStreamBufferSize = num * 1024;
			//}
			//if (this.TryGetConfigValue<int>("AS_TcpStreamBufferSize", out num, (int val) => val >= 0 && val <= 10240))
			//{
				XmlaClient.BufferSizeForNetworkStream = num * 1024;
			//}
			//if (this.TryGetConfigValue<int>("AS_EndSessionTimeout", out num, (int val) => val >= 0 || val == -1))
			//{
				XmlaClient.EndSessionTimeout = num;
			//}
		}

		//private bool TryGetConfigValue<T>(string configName, out T result)
		//{
		//	return this.TryGetConfigValue<T>(configName, out result, null);
		//}

		//private bool TryGetConfigValue<T>(string configName, out T result, Predicate<T> validationFunc)
		//{
		//	result = default(T);
		//	try
		//	{
		//		string value = ConfigurationManager.AppSettings[configName];
		//		if (!string.IsNullOrEmpty(value))
		//		{
		//			result = (T)((object)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture));
		//			bool result2;
		//			if (validationFunc != null)
		//			{
		//				result2 = validationFunc(result);
		//				return result2;
		//			}
		//			result2 = true;
		//			return result2;
		//		}
		//	}
		//	catch
		//	{
		//	}
		//	return false;
		//}

		internal XmlaClient(StringCollection log)
		{
			ServicePointManager.UseNagleAlgorithm = false;
			if (log == null)
			{
				throw new ArgumentNullException("log");
			}
			this.captureLog = log;
			this.namespacesManager = new NamespacesMgr();
			this.nameTable = XmlaConstants.GetNameTable();
		}

		internal static bool CheckForError(XmlReader reader, XmlaResult xmlaResult, bool throwIfError)
		{
			return XmlaClient.CheckForSoapFault(reader, xmlaResult, throwIfError) | XmlaClient.CheckForException(reader, xmlaResult, throwIfError) | XmlaClient.CheckForRowsetError(reader, xmlaResult, throwIfError) | XmlaClient.CheckForDatasetError(reader, xmlaResult, throwIfError);
		}

		internal static bool CheckForSoapFault(XmlReader reader, XmlaResult xmlaResult, bool throwIfError)
		{
			if (!reader.IsStartElement("Fault", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				return false;
			}
			reader.ReadStartElement();
			XmlaClient.ReadFaultBody(reader, xmlaResult.Messages);
			reader.ReadEndElement();
			if (throwIfError)
			{
				throw XmlaResultCollection.ExceptionOnError(xmlaResult);
			}
			return true;
		}

		internal static bool CheckForException(XmlReader reader, XmlaResult xmlaResult, bool throwIfError)
		{
			if (!reader.IsStartElement("Exception", "urn:schemas-microsoft-com:xml-analysis:exception"))
			{
				return false;
			}
			reader.Skip();
			try
			{
				while (!reader.IsStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/") && !reader.IsStartElement("Messages", "urn:schemas-microsoft-com:xml-analysis:exception"))
				{
					reader.ReadEndElement();
				}
			}
			catch (XmlException innerException)
			{
				throw new AdomdUnknownResponseException(XmlaSR.AfterExceptionAllTagsShouldCloseUntilMessagesSection, innerException);
			}
			if (!reader.IsStartElement("Messages", "urn:schemas-microsoft-com:xml-analysis:exception"))
			{
				throw new AdomdUnknownResponseException(XmlaSR.AfterExceptionAllTagsShouldCloseUntilMessagesSection, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1}, got {2}", new object[]
				{
					"urn:schemas-microsoft-com:xml-analysis:exception",
					"Messages",
					reader.Name
				}));
			}
			if (xmlaResult == null)
			{
				xmlaResult = new XmlaResult();
			}
			XmlaClient.ReadXmlaMessages(reader, xmlaResult.Messages);
			if (!xmlaResult.ContainsErrors)
			{
				throw new AdomdUnknownResponseException(XmlaSR.ExceptionRequiresXmlaErrorsInMessagesSection, "No errors in XMLA result");
			}
			if (throwIfError)
			{
				throw XmlaResultCollection.ExceptionOnError(xmlaResult);
			}
			return true;
		}

		internal static bool CheckForRowsetError(XmlReader reader, XmlaResult xmlaResult, bool throwIfError)
		{
			return XmlaClient.CheckForInlineError(reader, xmlaResult, throwIfError, "urn:schemas-microsoft-com:xml-analysis:exception");
		}

		internal static bool CheckForDatasetError(XmlReader reader, XmlaResult xmlaResult, bool throwIfError)
		{
			return XmlaClient.CheckForInlineError(reader, xmlaResult, throwIfError, "urn:schemas-microsoft-com:xml-analysis:mddataset");
		}

		internal static XmlaError CheckAndGetRowsetError(XmlReader reader, bool throwIfError)
		{
			return XmlaClient.CheckAndGetInlineError(reader, throwIfError, "urn:schemas-microsoft-com:xml-analysis:exception");
		}

		internal static XmlaError CheckAndGetDatasetError(XmlReader reader)
		{
			return XmlaClient.CheckAndGetInlineError(reader, false, "urn:schemas-microsoft-com:xml-analysis:mddataset");
		}

		private static bool CheckForInlineError(XmlReader reader, XmlaResult xmlaResult, bool throwIfError, string errorNamespace)
		{
			if (!reader.IsStartElement("Error", errorNamespace))
			{
				return false;
			}
			XmlaError item = XmlaClient.ReadInlineError(reader, errorNamespace);
			xmlaResult.Messages.Add(item);
			if (throwIfError)
			{
				throw XmlaResultCollection.ExceptionOnError(xmlaResult);
			}
			return true;
		}

		private static XmlaError CheckAndGetInlineError(XmlReader reader, bool throwIfError, string errorNamespace)
		{
			if (!reader.IsStartElement("Error", errorNamespace))
			{
				return null;
			}
			XmlaError xmlaError = XmlaClient.ReadInlineError(reader, errorNamespace);
			if (throwIfError)
			{
				throw XmlaResultCollection.ExceptionOnError(new XmlaResult
				{
					Messages = 
					{
						xmlaError
					}
				});
			}
			return xmlaError;
		}

		private static XmlaError ReadInlineError(XmlReader reader, string errorNamespace)
		{
			if (reader.IsEmptyElement)
			{
				throw new AdomdUnknownResponseException(XmlaSR.ErrorCodeIsMissingFromDatasetError, "Empty Error element");
			}
			reader.ReadStartElement();
			string text = null;
			string text2 = null;
			string callStack = null;
			XmlaMessageLocation location = null;
			while (reader.IsStartElement())
			{
				if (reader.LocalName == "ErrorCode" && reader.NamespaceURI == errorNamespace)
				{
					text = reader.ReadElementString();
				}
				else if (reader.LocalName == "Description" && reader.NamespaceURI == errorNamespace)
				{
					text2 = reader.ReadElementString();
					XmlaReader xmlaReader = reader as XmlaReader;
					if (!xmlaReader.HasExtendedErrorInfoBeenRead)
					{
						text2 += xmlaReader.GetExtendedErrorInfo();
					}
				}
				else if (reader.LocalName == "Location" && reader.NamespaceURI == "http://schemas.microsoft.com/analysisservices/2003/engine")
				{
					location = XmlaClient.ReadXmlaMessageLocation(reader);
				}
				else if (reader.LocalName == "CallStack" && reader.NamespaceURI == "http://schemas.microsoft.com/analysisservices/2011/engine/300")
				{
					callStack = reader.ReadElementString();
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
			if (text == null)
			{
				throw new AdomdUnknownResponseException(XmlaSR.ErrorCodeIsMissingFromDatasetError, "Missing error code");
			}
			return new XmlaError((int)XmlConvert.ToUInt32(text), text2, null, null, location, callStack);
		}

		private static XmlaMessageLocation ReadXmlaMessageLocation(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				return null;
			}
			int startLine = -1;
			int endLine = -1;
			int startColumn = -1;
			int endColumn = -1;
			XmlaLocationReference sourceObject = null;
			XmlaLocationReference dependsOnObject = null;
			reader.ReadStartElement();
			XmlaClient.ReadPosition(reader, "Start", ref startLine, ref startColumn);
			XmlaClient.ReadPosition(reader, "End", ref endLine, ref endColumn);
			int lineOffset = XmlaClient.ReadIntElementIfAny(reader, "LineOffset", "http://schemas.microsoft.com/analysisservices/2003/engine");
			int textLength = XmlaClient.ReadIntElementIfAny(reader, "TextLength", "http://schemas.microsoft.com/analysisservices/2003/engine");
			if (reader.IsStartElement("SourceObject", "http://schemas.microsoft.com/analysisservices/2010/engine/200"))
			{
				sourceObject = XmlaClient.ReadXmlaLocationReference(reader);
			}
			if (reader.IsStartElement("DependsOnObject", "http://schemas.microsoft.com/analysisservices/2010/engine/200"))
			{
				dependsOnObject = XmlaClient.ReadXmlaLocationReference(reader);
			}
			long rowNumber = XmlaClient.ReadLongElementIfAny(reader, "RowNumber", "http://schemas.microsoft.com/analysisservices/2010/engine/200");
			reader.ReadEndElement();
			return new XmlaMessageLocation(startLine, startColumn, endLine, endColumn, lineOffset, textLength, sourceObject, dependsOnObject, rowNumber);
		}

		private static XmlaLocationReference ReadXmlaLocationReference(XmlReader reader)
		{
			reader.ReadStartElement();
			string dimension = null;
			string hierarchy = null;
			string attribute = null;
			string cube = null;
			string measureGroup = null;
			string memberName = null;
			string role = null;
			string tableName = null;
			string columnName = null;
			string partitionName = null;
			string measureName = null;
			string roleName = null;
			bool flag;
			do
			{
				flag = false;
				if (reader.IsStartElement("Dimension", "http://schemas.microsoft.com/analysisservices/2010/engine/200"))
				{
					dimension = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("Hierarchy", "http://schemas.microsoft.com/analysisservices/2011/engine/300"))
				{
					hierarchy = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("Attribute", "http://schemas.microsoft.com/analysisservices/2010/engine/200"))
				{
					attribute = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("Cube", "http://schemas.microsoft.com/analysisservices/2010/engine/200"))
				{
					cube = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("MeasureGroup", "http://schemas.microsoft.com/analysisservices/2010/engine/200"))
				{
					measureGroup = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("MemberName", "http://schemas.microsoft.com/analysisservices/2010/engine/200"))
				{
					memberName = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("Role", "http://schemas.microsoft.com/analysisservices/2011/engine/300/300"))
				{
					role = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("RoleName", "http://schemas.microsoft.com/analysisservices/2013/engine/500/500"))
				{
					roleName = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("TableName", "http://schemas.microsoft.com/analysisservices/2013/engine/500/500"))
				{
					tableName = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("ColumnName", "http://schemas.microsoft.com/analysisservices/2013/engine/500/500"))
				{
					columnName = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("PartitionName", "http://schemas.microsoft.com/analysisservices/2013/engine/500/500"))
				{
					partitionName = reader.ReadElementString();
					flag = true;
				}
				else if (reader.IsStartElement("MeasureName", "http://schemas.microsoft.com/analysisservices/2013/engine/500/500"))
				{
					measureName = reader.ReadElementString();
					flag = true;
				}
			}
			while (flag);
			reader.ReadEndElement();
			return new XmlaLocationReference(dimension, hierarchy, attribute, cube, measureGroup, memberName, role, tableName, columnName, partitionName, measureName, roleName);
		}

		private static void ReadPosition(XmlReader reader, string positionName, ref int line, ref int column)
		{
			if (reader.IsStartElement(positionName, "http://schemas.microsoft.com/analysisservices/2003/engine"))
			{
				if (reader.IsEmptyElement)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Empty {0} element", new object[]
					{
						positionName
					}));
				}
				reader.ReadStartElement();
				line = XmlaClient.ReadIntElementIfAny(reader, "Line", "http://schemas.microsoft.com/analysisservices/2003/engine");
				column = XmlaClient.ReadIntElementIfAny(reader, "Column", "http://schemas.microsoft.com/analysisservices/2003/engine");
				reader.ReadEndElement();
			}
		}

		private static int ReadIntElementIfAny(XmlReader reader, string elementName, string elementNamespace)
		{
			if (!reader.IsStartElement(elementName, elementNamespace))
			{
				return -1;
			}
			if (reader.IsEmptyElement)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Empty {0}:{1} element", new object[]
				{
					elementNamespace,
					elementName
				}));
			}
			return XmlConvert.ToInt32(reader.ReadElementString(elementName, elementNamespace));
		}

		private static long ReadLongElementIfAny(XmlReader reader, string elementName, string elementNamespace)
		{
			if (!reader.IsStartElement(elementName, elementNamespace))
			{
				return -1L;
			}
			if (reader.IsEmptyElement)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected non-empty {0}:{1} element, but it is empty", new object[]
				{
					elementNamespace,
					elementName
				}));
			}
			return XmlConvert.ToInt64(reader.ReadElementString(elementName, elementNamespace));
		}

		internal static bool CheckForMessages(XmlReader reader, ref XmlaMessageCollection xmlaMessages)
		{
			if (reader.IsStartElement("Messages", "urn:schemas-microsoft-com:xml-analysis:exception"))
			{
				if (xmlaMessages == null)
				{
					xmlaMessages = new XmlaMessageCollection();
				}
				XmlaClient.ReadXmlaMessages(reader, xmlaMessages);
				return true;
			}
			return false;
		}

		internal static bool CheckForMessages(XmlReader reader, XmlaMessageCollection xmlaMessages)
		{
			return XmlaClient.CheckForMessages(reader, ref xmlaMessages);
		}

		internal static void ReadFaultBody(XmlReader reader, XmlaMessageCollection xmlaMessages)
		{
			string text = null;
			while (!XmlaClient.IsStartDetailElement(reader) && reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.IsStartElement("faultstring"))
				{
					text = reader.ReadElementString();
				}
				else
				{
					reader.Skip();
				}
			}
			bool flag = false;
			if (XmlaClient.IsStartDetailElement(reader) && !reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.IsStartElement())
				{
					if (reader.LocalName == "Error")
					{
						flag = true;
						xmlaMessages.Add(XmlaClient.ReadXmlaError(reader));
					}
					else if (reader.LocalName == "Warning")
					{
						xmlaMessages.Add(XmlaClient.ReadXmlaWarning(reader));
					}
					else
					{
						reader.Skip();
					}
				}
				XmlaClient.CheckEndElement(reader, "detail");
				if (reader.NamespaceURI != "" && reader.NamespaceURI != "http://schemas.xmlsoap.org/soap/envelope/")
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Got {0}", new object[]
					{
						reader.Name
					}));
				}
				reader.ReadEndElement();
			}
			while (reader.IsStartElement())
			{
				reader.Skip();
			}
			if (!flag)
			{
				if (text == null)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Missing fault string");
				}
				xmlaMessages.Add(new XmlaError(0, text, null, null, null));
			}
		}

		private static bool IsStartDetailElement(XmlReader reader)
		{
			return reader.IsStartElement() && reader.LocalName == "detail" && (reader.NamespaceURI == "" || reader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/");
		}

		internal static void ReadXmlaMessages(XmlReader reader, XmlaMessageCollection xmlaMessages)
		{
			int num = 0;
			reader.ReadStartElement("Messages", "urn:schemas-microsoft-com:xml-analysis:exception");
			while (reader.IsStartElement())
			{
				if (reader.LocalName == "Error")
				{
					xmlaMessages.Add(XmlaClient.ReadXmlaError(reader));
				}
				else
				{
					if (!(reader.LocalName == "Warning"))
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnrecognizedElementInMessagesSection(reader.Name), XmlaSR.UnrecognizedElementInMessagesSection(reader.Name));
					}
					xmlaMessages.Add(XmlaClient.ReadXmlaWarning(reader));
				}
				num++;
			}
			reader.ReadEndElement();
			if (num == 0)
			{
				throw new AdomdUnknownResponseException(XmlaSR.MessagesSectionIsEmpty, XmlaSR.MessagesSectionIsEmpty);
			}
		}

		internal static XmlaError ReadXmlaError(XmlReader reader)
		{
			int errorCode = (int)XmlConvert.ToUInt32(reader.GetAttribute("ErrorCode"));
			string attribute = reader.GetAttribute("Source");
			string attribute2 = reader.GetAttribute("HelpFile");
			string callStack = null;
			XmlaMessageLocation location = null;
			string text = reader.GetAttribute("Description");
			XmlaReader xmlaReader = reader as XmlaReader;
			if (!xmlaReader.HasExtendedErrorInfoBeenRead)
			{
				text += xmlaReader.GetExtendedErrorInfo();
			}
			if (reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				reader.ReadStartElement();
				bool flag = true;
				if (string.Equals(reader.LocalName, "Location") && string.Equals(reader.NamespaceURI, "http://schemas.microsoft.com/analysisservices/2003/engine"))
				{
					location = XmlaClient.ReadXmlaMessageLocation(reader);
					flag = false;
				}
				if (string.Equals(reader.LocalName, "CallStack") && string.Equals(reader.NamespaceURI, "http://schemas.microsoft.com/analysisservices/2011/engine/300"))
				{
					callStack = reader.ReadElementContentAsString();
					flag = false;
				}
				if (flag)
				{
					reader.Skip();
				}
				reader.ReadEndElement();
			}
			return new XmlaError(errorCode, text, attribute, attribute2, location, callStack);
		}

		internal static XmlaWarning ReadXmlaWarning(XmlReader reader)
		{
			int warningCode = (int)XmlConvert.ToUInt32(reader.GetAttribute("WarningCode"));
			string attribute = reader.GetAttribute("Description");
			string attribute2 = reader.GetAttribute("Source");
			string attribute3 = reader.GetAttribute("HelpFile");
			XmlaMessageLocation location = null;
			if (reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				reader.ReadStartElement();
				if (reader.IsStartElement("Location", "http://schemas.microsoft.com/analysisservices/2003/engine"))
				{
					location = XmlaClient.ReadXmlaMessageLocation(reader);
				}
				else
				{
					reader.Skip();
				}
				reader.ReadEndElement();
			}
			return new XmlaWarning(warningCode, attribute, attribute2, attribute3, location);
		}

		private void CheckAndGetHttpStreamSoapFault()
		{
			if (!(this.xmlaStream is HttpStream) || ((HttpStream)this.xmlaStream).StreamException == null)
			{
				return;
			}
			HttpStream httpStream = (HttpStream)this.xmlaStream;
			if (!this.reader.IsStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, httpStream.StreamException);
			}
			this.reader.ReadStartElement();
			if (this.reader.IsStartElement("Header", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				this.reader.Skip();
			}
			if (!this.reader.IsStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, httpStream.StreamException);
			}
			this.reader.ReadStartElement();
			try
			{
				if (!XmlaClient.CheckForSoapFault(this.reader, new XmlaResult(), true))
				{
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken, httpStream.StreamException);
				}
			}
			catch (XmlaException)
			{
				throw;
			}
			catch (XmlException e)
			{
				throw new AdomdUnknownResponseException(e);
			}
			throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, httpStream.StreamException.InnerException);
		}

		public void Connect(ConnectionInfo connectionInfo, bool beginSession)
		{
			if (this.connected)
			{
				throw new InvalidOperationException(XmlaSR.AlreadyConnected);
			}
			if (connectionInfo == null)
			{
				throw new ArgumentNullException("connectionInfo");
			}
			if (connectionInfo.IsAsAzure || connectionInfo.IsInternalASAzure)
			{
				this.supportsActivityIDAndRequestID = true;
				this.supportsCurrentActivityID = true;
			}
			bool flag = this.captureXml;
			this.captureXml = false;
			bool flag2 = false;
			try
			{
				try
				{
					if (connectionInfo.IXMLAMode)
					{
						this.OpenIXMLAConnection(connectionInfo);
					}
					else
					{
						using (IdentityResolver.Resolve(connectionInfo))
						{
							connectionInfo.ResolveLinkFileDataSource();
							if (connectionInfo.UseEU)
							{
								if (!this.ConnectionInfo.AllowDelegation)
								{
									throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileCannotDelegate);
								}
								connectionInfo.TryAddEffectiveUserName();
							}
						}
						this.OpenConnection(connectionInfo, out flag2);
					}
					ConnectionInfo connectionInfo2 = this.connInfo;
					try
					{
						this.connInfo = connectionInfo;
						if (beginSession)
						{
							string sessionToken = string.Empty;
							if (flag2)
							{
								sessionToken = this.GetSessionToken(connectionInfo.ExtendedProperties, false);
							}
							this.CreateSession(connectionInfo.ExtendedProperties, false, sessionToken);
						}
					}
					finally
					{
						this.connInfo = connectionInfo2;
					}
					this.connInfo = connectionInfo;
					this.xmlaStream.IsCompressionEnabled = this.isCompressionEnabled;
					this.userOpened = true;
				}
				catch (IOException innerException)
				{
					this.CloseAll();
					throw new AdomdConnectionException(XmlaSR.CannotConnect, innerException);
				}
				catch (XmlException innerException2)
				{
					this.CloseAll();
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException2);
				}
				catch (AdomdConnectionException ex)
				{
					if (ex.Message == XmlaSR.ConnectionBroken)
					{
						throw new AdomdConnectionException(XmlaSR.CannotConnect, ex.InnerException);
					}
					throw;
				}
				catch
				{
					this.CloseAll();
					throw;
				}
				finally
				{
					this.captureXml = flag;
				}
				if (!this.connInfo.IsLightweightConnection && !this.connInfo.IsForSqlBrowser)
				{
					if (!this.supportsCurrentActivityID)
					{
						this.supportsCurrentActivityID = this.SupportsProperty("DbpropMsmdCurrentActivityID");
						this.supportsActivityIDAndRequestID = this.supportsCurrentActivityID;
					}
					if (!this.supportsActivityIDAndRequestID)
					{
						this.supportsActivityIDAndRequestID = this.SupportsProperty("DbpropMsmdActivityID");
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void OpenConnection(ConnectionInfo connectionInfo, out bool isSessionTokenNeeded)
		{
			//WindowsImpersonationContext windowsImpersonationContext = null;
			isSessionTokenNeeded = false;
			using (IdentityResolver.Resolve(connectionInfo))
			{
				try
				{
					if (connectionInfo.RevertToProcessAccountForConnection)
					{
						try
						{
							//windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
						}
						catch (Exception innerException)
						{
							throw new AdomdConnectionException(XmlaSR.ConnectionString_LinkFileCannotRevert, innerException);
						}
					}
					switch (connectionInfo.ConnectionType)
					{
					case ConnectionType.Native:
					{
						AdomdConnectionException ex = null;
						SecurityContextMode[] arg_86_0;
						if (!connectionInfo.IsSchannelSspi())
						{
							SecurityContextMode[] array = new SecurityContextMode[1];
							arg_86_0 = array;
						}
						else
						{
							arg_86_0 = new SecurityContextMode[]
							{
								SecurityContextMode.stream
							};
						}
						SecurityContextMode[] array2 = arg_86_0;
						SecurityContextMode[] array3 = array2;
						for (int i = 0; i < array3.Length; i++)
						{
							SecurityContextMode securityContextMode = array3[i];
							ex = null;
							try
							{
								this.OpenTcpConnection(connectionInfo, securityContextMode);
								break;
							}
							catch (AdomdConnectionException ex2)
							{
								ex = ex2;
							}
							catch (Win32Exception innerException2)
							{
								ex = new AdomdConnectionException(XmlaSR.Authentication_Failed, innerException2, ConnectionExceptionCause.AuthenticationFailed);
							}
						}
						if (ex != null)
						{
							throw ex;
						}
						break;
					}
					case ConnectionType.Http:
						this.OpenHttpConnection(connectionInfo, out isSessionTokenNeeded);
						break;
					case ConnectionType.LocalServer:
						this.OpenLocalServerConnection(connectionInfo);
						break;
					case ConnectionType.LocalCube:
						this.OpenLocalCubeConnection(connectionInfo);
						break;
					case ConnectionType.Wcf:
						this.OpenWcfConnection(connectionInfo);
						break;
					case ConnectionType.LocalFarm:
						this.OpenLocalFarmConnection(connectionInfo);
						break;
					case ConnectionType.OnPremFromCloudAccess:
						this.OpenOnPremFromCloudAccessConnection(connectionInfo, out isSessionTokenNeeded);
						break;
					default:
						throw new NotImplementedException();
					}
				}
				finally
				{
					//if (windowsImpersonationContext != null)
					//{
					//	windowsImpersonationContext.Undo();
					//	windowsImpersonationContext = null;
					//}
				}
			}
		}

		private bool IsCompressionDesired(ConnectionInfo connectionInfo)
		{
			return (connectionInfo.TransportCompression == TransportCompression.Compressed || connectionInfo.TransportCompression == TransportCompression.Default) && !connectionInfo.IsOnPremFromCloudAccess && XpressMethodsWrapper.XpressAvailable;
		}

		private bool IsBinaryDesired(ConnectionInfo connectionInfo)
		{
			return false;
		}

		private DataType GetDesiredRequestType(ConnectionInfo connectionInfo)
		{
			if (this.IsCompressionDesired(connectionInfo))
			{
				return DataType.CompressedXml;
			}
			return DataType.TextXml;
		}

		private DataType GetDesiredResponseType(ConnectionInfo connectionInfo)
		{
			if (this.IsCompressionDesired(connectionInfo) && this.IsBinaryDesired(connectionInfo))
			{
				return DataType.CompressedBinaryXml;
			}
			if (this.IsCompressionDesired(connectionInfo))
			{
				return DataType.CompressedXml;
			}
			if (this.IsBinaryDesired(connectionInfo))
			{
				return DataType.BinaryXml;
			}
			return DataType.TextXml;
		}

		private void OpenOnPremFromCloudAccessConnection(ConnectionInfo connectionInfo, out bool isSessionTokenNeeded)
		{
			try
			{
				isSessionTokenNeeded = false;
				int timeoutInSeconds = (connectionInfo.Timeout > 0) ? connectionInfo.Timeout : -1;
				string configurationProperties = null;
				string onPremConnectionString = connectionInfo.GetOnPremConnectionString(out configurationProperties);
				OnPremFromCloudAccessTranportClient onPremFromCloudAccessTranportClient = new OnPremFromCloudAccessTranportClient(timeoutInSeconds, configurationProperties);
				XmlaStream transportStream = onPremFromCloudAccessTranportClient.GetTransportStream(onPremConnectionString, this.GetDesiredRequestType(connectionInfo), this.GetDesiredResponseType(connectionInfo));
				this.xmlaStream = transportStream;
				this.connected = true;
			}
			catch (COMException innerException)
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
		}

		private void OpenTcpConnection(ConnectionInfo connectionInfo, SecurityContextMode securityContextMode)
		{
			DateTime now = DateTime.Now;
			int num = connectionInfo.ConnectTimeout * 1000;
			this.tcpClient = XmlaClient.GetTcpClient(connectionInfo);
			try
			{
				if (connectionInfo.ConnectTimeout > 0)
				{
					num = (int)((double)num - DateTime.Now.Subtract(now).TotalMilliseconds);
					if (num <= 0)
					{
						throw new AdomdConnectionException(XmlaSR.XmlaClient_ConnectTimedOut);
					}
					this.SetReadWriteTimeouts(num, num);
				}
				this.networkStream = new BufferedStream(this.tcpClient.GetStream(), XmlaClient.BufferSizeForNetworkStream);
				TcpStream tcpStream = new TcpStream(this.networkStream, connectionInfo.PacketSize, this.GetDesiredRequestType(connectionInfo), this.GetDesiredResponseType(connectionInfo));
				CompressedStream compressedStream = new CompressedStream(tcpStream, connectionInfo.CompressionLevel);
				this.xmlaStream = compressedStream;
				if (this.identityTransferToken != null)
				{
					this.connected = true;
					this.connInfo = connectionInfo;
					this.AuthenticateWithIdentityTransferToken(connectionInfo, now);
				}
				else if (!connectionInfo.IsSspiAnonymous && connectionInfo.ProtectionLevel != ProtectionLevel.None)
				{
					this.connected = true;
					this.connInfo = connectionInfo;
					SecurityContext securityContext = this.Authenticate(connectionInfo, now, securityContextMode);
					switch (connectionInfo.ProtectionLevel)
					{
					case ProtectionLevel.Connection:
						securityContext.Close();
						break;
					case ProtectionLevel.Integrity:
					{
						TcpSignedStream baseXmlaStream = new TcpSignedStream(tcpStream, securityContext);
						compressedStream.SetBaseXmlaStream(baseXmlaStream);
						break;
					}
					case ProtectionLevel.Privacy:
					{
						TcpEncryptedStream baseXmlaStream2 = new TcpEncryptedStream(tcpStream, securityContext);
						compressedStream.SetBaseXmlaStream(baseXmlaStream2);
						break;
					}
					}
				}
				if (connectionInfo.ConnectionType == ConnectionType.Native && !string.IsNullOrEmpty(connectionInfo.AuthenticationScheme))
				{
					this.SendExtAuth(connectionInfo);
				}
				this.SetReadWriteTimeouts(0, 0);
				this.connected = true;
			}
			catch
			{
				this.connected = false;
				this.connInfo = null;
				if (this.tcpClient != null)
				{
					this.tcpClient.Close();
					this.tcpClient = null;
				}
				throw;
			}
		}

		internal HttpWebResponse GetHttpResponse(string url, string soapAction, string user, string password)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = -1;
			httpWebRequest.Credentials = ((user == null) ? CredentialCache.DefaultCredentials : new NetworkCredential(user, password));
			httpWebRequest.UserAgent = "ADOMD.NET";
			if (soapAction != null)
			{
				httpWebRequest.Headers.Add(soapAction);
			}
			httpWebRequest.ContentLength = 0L;
			httpWebRequest.CookieContainer = this.cookieContainer;
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			HttpWebResponse result;
			try
			{
				if (httpWebResponse.ContentLength == 0L)
				{
					result = httpWebResponse;
				}
				else
				{
					using (XmlTextReader xmlTextReader = new XmlTextReader(httpWebResponse.GetResponseStream()))
					{
						try
						{
							xmlTextReader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
							if (xmlTextReader.IsStartElement("Header", "http://schemas.xmlsoap.org/soap/envelope/"))
							{
								xmlTextReader.Skip();
							}
							xmlTextReader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
							XmlaClient.CheckForSoapFault(xmlTextReader, new XmlaResult(), true);
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected soap:Fault, got {0}", new object[]
							{
								xmlTextReader.Name
							}));
						}
						catch (XmlException innerException)
						{
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
						}
						catch (IOException innerException2)
						{
							throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
						}
					}
				}
			}
			catch
			{
				httpWebResponse.Close();
				throw;
			}
			return result;
		}

		private void OpenLocalFarmConnection(ConnectionInfo connectionInfo)
		{
			IWorkbookSession workbookSession;
			if (string.IsNullOrEmpty(connectionInfo.ExcelSessionId))
			{
				workbookSession = XmlaClient.SPProxy.OpenWorkbookModel(connectionInfo.Server);
			}
			else
			{
				workbookSession = XmlaClient.SPProxy.OpenWorkbookSession(connectionInfo.Server, connectionInfo.ExcelSessionId);
			}
			ConnectionInfo connectionInfo2 = new ConnectionInfo(connectionInfo);
			if (workbookSession.Server == null || workbookSession.Database == null)
			{
				throw new AdomdConnectionException(XmlaSR.Connect_RedirectorDidntReturnDatabaseInfo);
			}
			connectionInfo2.ExtractDataSourceParts(workbookSession.Server, null);
			connectionInfo2.SetCatalog(workbookSession.Database);
			connectionInfo2.RevertToProcessAccountForConnection = true;
			bool flag;
			this.OpenConnection(connectionInfo2, out flag);
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add("LocaleIdentifier", Convert.ToString(CultureInfo.CurrentCulture.LCID, CultureInfo.InvariantCulture));
			this.SetAuthContext(workbookSession.UserName, workbookSession.Database, listDictionary);
			this.workbookSession = workbookSession;
		}

		internal void SetAuthContext(string token, string databaseID, IDictionary commandProperties)
		{
			if (token == null || databaseID == null)
			{
				throw new ArgumentNullException();
			}
			this.CheckConnection();
			try
			{
				this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"");
				this.WriteStartCommand(ref commandProperties);
				this.writer.WriteStartElement("SetAuthContext", "http://schemas.microsoft.com/analysisservices/2003/engine");
				this.writer.WriteElementString("Token", token);
				this.writer.WriteElementString("DatabaseID", databaseID);
				this.writer.WriteEndElement();
				this.WriteEndCommand(this.ConnectionInfo.ExtendedProperties, commandProperties, null);
				this.EndMessage();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			this.SendExecuteAndReadResponse(false, true);
		}

		private void OpenWcfConnection(ConnectionInfo connectionInfo)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			string empty5 = string.Empty;
			this.WCFConnect(connectionInfo.Server, connectionInfo.DataSourceVersion, ref empty5, ref empty, ref empty2, ref empty3, ref empty4);
			bool specificVersion = true;
			if (string.IsNullOrEmpty(connectionInfo.DataSourceVersion))
			{
				specificVersion = false;
				if (string.IsNullOrEmpty(empty5))
				{
					throw new AdomdConnectionException(XmlaSR.Connect_RedirectorDidntReturnDatabaseInfo);
				}
				connectionInfo.DataSourceVersion = empty5;
			}
			connectionInfo.SetCatalog(empty2);
			WcfStream wcfStream = new WcfStream(connectionInfo.Server, empty4, specificVersion, empty3, empty, this.GetDesiredRequestType(connectionInfo), this.GetDesiredResponseType(connectionInfo), connectionInfo.ApplicationName);
			this.xmlaStream = new CompressedStream(wcfStream, connectionInfo.CompressionLevel);
			this.connected = true;
		}

		private void OpenHttpConnection(ConnectionInfo connectionInfo, out bool isSessionTokenNeeded)
		{
			isSessionTokenNeeded = false;
			bool acceptCompressedResponses = false;
			if (this.cookieContainer == null)
			{
				this.cookieContainer = new CookieContainer();
			}
			Uri dataSourceUri;
			if (connectionInfo.IsForRedirector)
			{
				using (HttpWebResponse httpResponse = this.GetHttpResponse(connectionInfo.GetRedirectorUrlForDatabase(), null, connectionInfo.UserID, connectionInfo.Password))
				{
					string text = httpResponse.Headers["DatabaseId"];
					if (text != null)
					{
						text = text.Trim();
					}
					if (string.IsNullOrEmpty(text))
					{
						throw new AdomdConnectionException(XmlaSR.Connect_RedirectorDidntReturnDatabaseInfo);
					}
					string text2 = httpResponse.Headers["DatabaseName"];
					if (text2 != null)
					{
						text2 = text2.Trim();
					}
					if (string.IsNullOrEmpty(text2))
					{
						throw new AdomdConnectionException(XmlaSR.Connect_RedirectorDidntReturnDatabaseInfo);
					}
					bool specificVersion = true;
					if (string.IsNullOrEmpty(connectionInfo.DataSourceVersion))
					{
						specificVersion = false;
						string text3 = httpResponse.Headers["DataSourceVersion"];
						if (!string.IsNullOrEmpty(text3))
						{
							text3 = text3.Trim();
						}
						if (string.IsNullOrEmpty(text3))
						{
							throw new AdomdConnectionException(XmlaSR.Connect_RedirectorDidntReturnDatabaseInfo);
						}
						connectionInfo.DataSourceVersion = text3;
					}
					connectionInfo.SetCatalog(text2);
					dataSourceUri = new Uri(connectionInfo.GetRedirectorUrlForRedirect(text, specificVersion));
					httpResponse.Close();
					goto IL_126;
				}
			}
			dataSourceUri = new Uri(connectionInfo.Server);
			IL_126:
			if (connectionInfo.IsAsAzure)
			{
				connectionInfo.AadTokenHolder = AadAuthenticator.AcquireToken(dataSourceUri, connectionInfo.DataSource, connectionInfo.IdentityProvider, connectionInfo.UserID, connectionInfo.Password, connectionInfo.UseAdalCache);
				TimeoutUtils.TimeLeft timeLeft = TimeoutUtils.TimeLeft.FromSeconds(connectionInfo.ConnectTimeout);
				dataSourceUri = ASAzureUtility.ResolveActualClusterUri(dataSourceUri, connectionInfo.AsAzureServerName, connectionInfo.AadTokenHolder, ref timeLeft, delegate
				{
					throw new AdomdConnectionException(XmlaSR.XmlaClient_ConnectTimedOut);
				});
				connectionInfo.ConnectTimeout = timeLeft.TimeSec;
			}
			if (connectionInfo.ClientCertificateThumbprint != null || connectionInfo.IntegratedSecurity == IntegratedSecurity.Federated)
			{
				isSessionTokenNeeded = true;
			}
			int timeoutMs = (connectionInfo.Timeout > 0) ? (connectionInfo.Timeout * 1000) : -1;
			HttpStream httpStream = new HttpStream(dataSourceUri, acceptCompressedResponses, this.GetDesiredRequestType(connectionInfo), this.GetDesiredResponseType(connectionInfo), this.cookieContainer, timeoutMs, connectionInfo);
			this.xmlaStream = new CompressedStream(httpStream, connectionInfo.CompressionLevel);
			this.connected = true;
		}

		private void OpenLocalServerConnection(ConnectionInfo connectionInfo)
		{
			this.xmlaStream = new LocalServerStream();
			this.connected = true;
		}

		private void OpenLocalCubeConnection(ConnectionInfo connectionInfo)
		{
			if (connectionInfo.RestrictedClient)
			{
				throw new InvalidOperationException(XmlaSR.XmlaClient_CannotConnectToLocalCubeWithRestictedClient);
			}
			string cubeFile;
			string serverName;
			MsmdlocalWrapper.OpenFlags settings;
			if (connectionInfo.IsEmbedded)
			{
				cubeFile = null;
				serverName = connectionInfo.Location;
				settings = (MsmdlocalWrapper.OpenFlags.OpenExisting | MsmdlocalWrapper.OpenFlags.UseImbi);
			}
			else
			{
				cubeFile = connectionInfo.Server;
				serverName = null;
				settings = (connectionInfo.UseExistingFile ? MsmdlocalWrapper.OpenFlags.OpenExisting : MsmdlocalWrapper.OpenFlags.OpenOrCreate);
			}
			this.xmlaStream = new LocalCubeStream(cubeFile, settings, connectionInfo.Timeout, connectionInfo.EncryptionPassword, serverName);
			this.connected = true;
		}

		private void WriteIfNonEmptyElement(string elementName, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.writer.WriteStartElement(elementName);
				this.writer.WriteString(value);
				this.writer.WriteEndElement();
			}
		}

		private void WriteIfNonEmptyElement(StringBuilder sb, string elementName, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				sb.AppendFormat("<{0}>{1}</{0}>", elementName, value);
			}
		}

		private void SendExtAuth(ConnectionInfo connectionInfo)
		{
			this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"", true, true, false);
			XmlaResult xmlaResult = new XmlaResult();
			try
			{
				this.writer.WriteStartElement("Execute", "urn:schemas-microsoft-com:xml-analysis");
				this.writer.WriteStartElement("Command");
				this.writer.WriteStartElement("ExtAuth", "http://schemas.microsoft.com/analysisservices/2003/engine");
				this.WriteIfNonEmptyElement("AuthenticationScheme", connectionInfo.AuthenticationScheme);
				this.writer.WriteStartElement("ExtAuthInfo");
				if (!string.IsNullOrEmpty(connectionInfo.ExtAuthInfo))
				{
					this.writer.WriteString(connectionInfo.ExtAuthInfo);
				}
				else if (connectionInfo.AuthenticationScheme.Equals("ActAs", StringComparison.OrdinalIgnoreCase))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("<Properties>");
					this.WriteIfNonEmptyElement(stringBuilder, "IdentityProvider", this.EscapeXMLString(connectionInfo.IdentityProvider));
					this.WriteIfNonEmptyElement(stringBuilder, "UserName", this.EscapeXMLString(connectionInfo.UserID));
					this.WriteIfNonEmptyElement(stringBuilder, "BypassAuthorization", this.EscapeXMLString(connectionInfo.BypassAuthorization));
					if (connectionInfo.RestrictCatalog.Equals("true", StringComparison.OrdinalIgnoreCase))
					{
						this.WriteIfNonEmptyElement(stringBuilder, "RestrictCatalog", this.EscapeXMLString(connectionInfo.Catalog));
					}
					this.WriteIfNonEmptyElement(stringBuilder, "AccessMode", this.EscapeXMLString(connectionInfo.AccessMode));
					stringBuilder.Append("</Properties>");
					this.writer.WriteString(stringBuilder.ToString());
				}
				this.writer.WriteEndElement();
				this.writer.WriteEndElement();
				this.writer.WriteEndElement();
				this.WriteProperties(null, null);
				this.writer.WriteEndElement();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			this.EndMessage();
			this.EndRequest();
			try
			{
				this.reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
				this.ReadEnvelopeHeader(this.reader, false, true);
				this.reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
				XmlaClient.CheckForError(this.reader, xmlaResult, true);
				this.reader.ReadStartElement("ExecuteResponse", "urn:schemas-microsoft-com:xml-analysis");
				this.reader.ReadStartElement("return", "urn:schemas-microsoft-com:xml-analysis");
				if (!XmlaClient.IsEmptyResultS(this.reader))
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1}, got {2}", new object[]
					{
						"urn:schemas-microsoft-com:xml-analysis:empty",
						"root",
						this.reader.Name
					}));
				}
				this.reader.Skip();
				this.reader.ReadEndElement();
				this.reader.ReadEndElement();
				this.reader.ReadEndElement();
				XmlaClient.CheckEndElement(this.reader, "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
			}
			catch (XmlException innerException2)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException2);
			}
			catch (IOException innerException3)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException3);
			}
			catch (AdomdUnknownResponseException)
			{
				throw;
			}
			catch (XmlaException)
			{
				throw;
			}
			catch
			{
				this.CloseAll();
				throw;
			}
			finally
			{
				if (this.connected)
				{
					this.EndReceival(true);
				}
			}
		}

		private string EscapeXMLString(string xmlString)
		{
			return SecurityElement.Escape(xmlString);
		}

		private SecurityContext Authenticate(ConnectionInfo connectionInfo, DateTime startTime, SecurityContextMode securityContextMode)
		{
			SecurityContext securityContext;
			try
			{
				NTAuthentication nTAuthentication = new NTAuthentication(connectionInfo, securityContextMode);
				bool flag = false;
				byte[] array = null;
				XmlaResult xmlaResult = new XmlaResult();
				double num = (double)(connectionInfo.ConnectTimeout * 1000);
				bool flag2 = connectionInfo.ConnectTimeout > 0;
				while (!flag)
				{
					array = nTAuthentication.GetOutgoingBlob(array, out flag);
					if (array == null)
					{
						break;
					}
					if (flag2)
					{
						int num2 = (int)(num - DateTime.Now.Subtract(startTime).TotalMilliseconds);
						if (num2 <= 0)
						{
							throw new AdomdConnectionException(XmlaSR.XmlaClient_ConnectTimedOut);
						}
						this.SetReadWriteTimeouts(num2, num2);
					}
					this.StartRequest(null);
					this.writer.WriteStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
					this.writer.WriteStartElement("Body");
					this.writer.WriteStartElement("Authenticate", "http://schemas.microsoft.com/analysisservices/2003/ext");
					if (nTAuthentication.IsSchannel)
					{
						if (securityContextMode == SecurityContextMode.stream)
						{
							this.writer.WriteElementString("ClientVersion", "1");
						}
						this.writer.WriteElementString("AuthProtocol", "Schannel");
					}
					this.writer.WriteElementString("SspiHandshake", Convert.ToBase64String(array));
					this.writer.WriteEndElement();
					this.writer.WriteEndElement();
					this.writer.WriteEndElement();
					this.EndRequest();
					this.reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
					this.ReadEnvelopeHeader(this.reader, false, false);
					this.reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
					XmlaClient.CheckForError(this.reader, xmlaResult, true);
					this.reader.ReadStartElement("AuthenticateResponse", "http://schemas.microsoft.com/analysisservices/2003/ext");
					this.reader.ReadStartElement("return", "http://schemas.microsoft.com/analysisservices/2003/ext");
					this.reader.MoveToContent();
					object obj;
					if (this.reader.IsEmptyElement)
					{
						obj = string.Empty;
						this.reader.Skip();
					}
					else
					{
						obj = this.reader.ReadElementContentAsObject("SspiHandshake", "http://schemas.microsoft.com/analysisservices/2003/ext");
					}
					array = ((obj is string) ? Convert.FromBase64String((string)obj) : ((byte[])obj));
					if (flag && array != null && array.Length != 0)
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Non-empty SSPI token value");
					}
					this.reader.ReadEndElement();
					this.reader.ReadEndElement();
					this.reader.ReadEndElement();
					this.reader.ReadEndElement();
					this.EndReceival(true);
				}
				securityContext = nTAuthentication.SecurityContext;
			}
			catch (Win32Exception innerException)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_Failed, innerException, ConnectionExceptionCause.AuthenticationFailed);
			}
			return securityContext;
		}

		private void AuthenticateWithIdentityTransferToken(ConnectionInfo connectionInfo, DateTime startTime)
		{
			try
			{
				bool flag = false;
				XmlaResult xmlaResult = new XmlaResult();
				double num = (double)(connectionInfo.ConnectTimeout * 1000);
				bool flag2 = connectionInfo.ConnectTimeout > 0;
				while (!flag)
				{
					if (flag2)
					{
						int num2 = (int)(num - DateTime.Now.Subtract(startTime).TotalMilliseconds);
						if (num2 <= 0)
						{
							throw new AdomdConnectionException(XmlaSR.XmlaClient_ConnectTimedOut);
						}
						this.SetReadWriteTimeouts(num2, num2);
					}
					this.StartRequest(null);
					this.writer.WriteStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
					this.writer.WriteStartElement("Body");
					this.writer.WriteStartElement("Authenticate", "http://schemas.microsoft.com/analysisservices/2003/ext");
					string text = XmlaClient.BuildIdentityTransferToken(this.identityTransferToken);
					this.writer.WriteStartElement("AuthToken");
					this.writer.WriteAttributeString("TokenType", "IdentityTransferToken");
					this.writer.WriteString(text);
					this.writer.WriteEndElement();
					this.writer.WriteEndElement();
					this.writer.WriteEndElement();
					this.writer.WriteEndElement();
					this.EndRequest();
					this.reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
					this.ReadEnvelopeHeader(this.reader, false, false);
					this.reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
					XmlaClient.CheckForError(this.reader, xmlaResult, true);
					this.reader.ReadStartElement("AuthenticateResponse", "http://schemas.microsoft.com/analysisservices/2003/ext");
					this.reader.ReadStartElement("return", "http://schemas.microsoft.com/analysisservices/2003/ext");
					this.reader.MoveToContent();
					object obj;
					if (this.reader.IsEmptyElement)
					{
						obj = string.Empty;
						this.reader.Skip();
					}
					else
					{
						obj = this.reader.ReadElementContentAsObject("AuthToken", "http://schemas.microsoft.com/analysisservices/2003/ext");
					}
					obj.ToString();
					flag = true;
					this.reader.ReadEndElement();
					this.reader.ReadEndElement();
					this.reader.ReadEndElement();
					this.reader.ReadEndElement();
					this.EndReceival(true);
				}
			}
			catch (Win32Exception innerException)
			{
				throw new AdomdConnectionException(XmlaSR.Authentication_Failed, innerException, ConnectionExceptionCause.AuthenticationFailed);
			}
		}

		private static string BuildIdentityTransferToken(IdentityTransferToken token)
		{
			return token.GetTokenXml();
		}

		internal void CreateSession(ListDictionary properties, bool sendNamespaceCompatibility)
		{
			this.CreateSession(properties, sendNamespaceCompatibility, string.Empty);
		}

		internal void CreateSession(ListDictionary properties, bool sendNamespaceCompatibility, string sessionToken)
		{
			this.SessionID = null;
			IDictionary commandProperties = null;
			this.PopulateActivityIDAndRequestID(ref commandProperties, false);
			this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"", true, sendNamespaceCompatibility, false);
			XmlaResult xmlaResult = new XmlaResult();
			try
			{
				this.writer.WriteStartElement("Execute", "urn:schemas-microsoft-com:xml-analysis");
				this.writer.WriteStartElement("Command");
				if (string.IsNullOrEmpty(sessionToken))
				{
					this.writer.WriteElementString("Statement", string.Empty);
				}
				else
				{
					this.writer.WriteStartElement("ExtAuth");
					this.writer.WriteStartElement("AuthenticationScheme");
					this.writer.WriteString("DelegateToken");
					this.writer.WriteEndElement();
					this.writer.WriteStartElement("ExtAuthInfo");
					this.writer.WriteString(sessionToken);
					this.writer.WriteEndElement();
					this.writer.WriteEndElement();
				}
				this.writer.WriteEndElement();
				this.WriteProperties(properties, commandProperties);
				this.writer.WriteEndElement();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			this.EndMessage();
			this.EndRequest();
			try
			{
				this.reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
				this.ReadEnvelopeHeader(this.reader, true, sendNamespaceCompatibility);
				this.reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
				XmlaClient.CheckForError(this.reader, xmlaResult, true);
				this.reader.ReadStartElement("ExecuteResponse", "urn:schemas-microsoft-com:xml-analysis");
				this.reader.ReadStartElement("return", "urn:schemas-microsoft-com:xml-analysis");
				if (!XmlaClient.IsEmptyResultS(this.reader))
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1}, got {2}", new object[]
					{
						"urn:schemas-microsoft-com:xml-analysis:empty",
						"root",
						this.reader.Name
					}));
				}
				this.reader.Skip();
				this.reader.ReadEndElement();
				this.reader.ReadEndElement();
				this.reader.ReadEndElement();
				XmlaClient.CheckEndElement(this.reader, "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
			}
			catch (XmlException innerException2)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException2);
			}
			catch (IOException innerException3)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException3);
			}
			catch (AdomdUnknownResponseException)
			{
				throw;
			}
			catch (XmlaException)
			{
				throw;
			}
			catch
			{
				this.CloseAll();
				throw;
			}
			finally
			{
				if (this.connected)
				{
					this.EndReceival(true);
				}
			}
		}

		private string GetSessionToken(ListDictionary properties, bool sendNamespaceCompatibility)
		{
			string result;
			try
			{
				IDictionary commandProperties = null;
				this.PopulateActivityIDAndRequestID(ref commandProperties, false);
				this.xmlaStream.IsSessionTokenNeeded = true;
				this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"", false, sendNamespaceCompatibility, true);
				new XmlaResult();
				try
				{
					this.writer.WriteStartElement("Execute", "urn:schemas-microsoft-com:xml-analysis");
					this.writer.WriteStartElement("Command");
					this.writer.WriteElementString("Statement", string.Empty);
					this.writer.WriteEndElement();
					this.WriteProperties(properties, commandProperties);
					this.writer.WriteEndElement();
				}
				catch (IOException innerException)
				{
					this.CloseAll();
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
				}
				catch
				{
					this.HandleMessageCreationException();
					throw;
				}
				this.EndMessage();
				try
				{
					this.EndRequest();
					this.reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
					result = this.GetTokenFromEnvelopeHeader(this.reader);
				}
				catch (WebException ex)
				{
					HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse == null || httpWebResponse.StatusCode != HttpStatusCode.BadRequest)
					{
						throw new AdomdConnectionException(XmlaSR.ConnectionBroken, ex);
					}
					result = string.Empty;
				}
				catch (XmlException innerException2)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException2);
				}
				catch (IOException innerException3)
				{
					this.CloseAll();
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException3);
				}
				catch (AdomdUnknownResponseException)
				{
					throw;
				}
				catch (XmlaException)
				{
					throw;
				}
				catch
				{
					this.CloseAll();
					throw;
				}
				finally
				{
					if (this.connected)
					{
						this.EndReceival(true);
					}
				}
			}
			catch
			{
				if (this.xmlaStream == null)
				{
					throw;
				}
				result = string.Empty;
			}
			finally
			{
				if (this.xmlaStream != null)
				{
					this.xmlaStream.IsSessionTokenNeeded = false;
				}
			}
			return result;
		}

		public void Disconnect(bool endSession)
		{
			try
			{
				if (this.SessionID != null && endSession)
				{
					if (this.connected && this.connInfo != null)
					{
						int readTimeout = -1;
						try
						{
							if (this.xmlaStream.CanTimeout)
							{
								readTimeout = this.xmlaStream.ReadTimeout;
								this.xmlaStream.ReadTimeout = XmlaClient.EndSessionTimeout;
							}
							this.EndSession(this.connInfo.ExtendedProperties);
						}
						catch (AdomdConnectionException)
						{
						}
						catch (XmlaException)
						{
						}
						catch (AdomdUnknownResponseException)
						{
						}
						catch (SocketException)
						{
						}
						catch (XmlException)
						{
						}
						catch (IOException)
						{
						}
						catch (Exception)
						{
							throw;
						}
						finally
						{
							if (this.xmlaStream != null && this.xmlaStream.CanTimeout)
							{
								this.xmlaStream.ReadTimeout = readTimeout;
							}
						}
					}
					this.SessionID = null;
				}
			}
			finally
			{
				this.userOpened = false;
				this.CloseAll();
			}
		}

		internal void EndSession(ListDictionary properties)
		{
			if (this.SessionID == null || this.captureXml)
			{
				return;
			}
			IDictionary commandProperties = null;
			this.PopulateActivityIDAndRequestID(ref commandProperties, false);
			this.StartRequest("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"");
			try
			{
				this.writer.WriteStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
				this.writer.WriteStartElement("Header");
				this.writer.WriteStartElement("EndSession", "urn:schemas-microsoft-com:xml-analysis");
				this.writer.WriteAttributeString("soap", "mustUnderstand", "http://schemas.xmlsoap.org/soap/envelope/", "1");
				this.writer.WriteAttributeString("SessionId", this.SessionID);
				this.writer.WriteEndElement();
				this.writer.WriteEndElement();
				this.writer.WriteStartElement("Body");
				this.writer.WriteStartElement("Execute", "urn:schemas-microsoft-com:xml-analysis");
				this.writer.WriteStartElement("Command");
				this.writer.WriteElementString("Statement", string.Empty);
				this.writer.WriteEndElement();
				this.WriteProperties(properties, commandProperties);
				this.writer.WriteEndElement();
				this.writer.WriteEndElement();
				this.writer.WriteEndElement();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			this.SendExecuteAndReadResponse(true, true);
		}

		internal void CloseAll()
		{
			lock (this.lockForCloseAll)
			{
				if (this.workbookSession != null)
				{
					this.workbookSession.Dispose();
					this.workbookSession = null;
				}
				this.connected = false;
				this.connectionState = ConnectionState.Closed;
				if (this.cookieContainer != null)
				{
					this.cookieContainer = new CookieContainer();
				}
				try
				{
					if (this.tcpClient != null)
					{
						try
						{
							if (this.networkStream != null)
							{
								this.networkStream.Close();
							}
							this.tcpClient.Close();
						}
						catch (SocketException)
						{
						}
						catch (IOException)
						{
						}
						this.networkStream = null;
						this.tcpClient = null;
					}
					if (this.xmlaStream != null)
					{
						this.xmlaStream.Dispose();
						this.xmlaStream = null;
					}
					if (this.writer != null)
					{
						try
						{
							this.writer.Close();
						}
						catch (ObjectDisposedException)
						{
						}
						catch (InvalidOperationException)
						{
						}
						this.writer = null;
					}
					if (this.reader != null)
					{
						try
						{
							if (this.reader is XmlaReader)
							{
								((XmlaReader)this.reader).CloseWithoutEndReceival();
							}
							else
							{
								this.reader.Close();
							}
						}
						catch (InvalidOperationException)
						{
						}
						this.reader = null;
					}
				}
				catch (SocketException)
				{
				}
				catch (XmlException)
				{
				}
				catch (IOException)
				{
				}
				catch (WebException)
				{
				}
				catch (Win32Exception)
				{
				}
				catch (COMException)
				{
				}
			}
		}

		internal XmlWriter StartMessage(string action)
		{
			return this.StartMessage(action, false, false, false);
		}

		private XmlWriter StartMessage(string action, bool addCreateSession, bool sendNamespaceCompatibility, bool addGetSessionToken)
		{
			this.StartRequest(action);
			if (!this.captureXml)
			{
				try
				{
					this.writer.WriteStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
					this.writer.WriteStartElement("Header");
					if (this.SessionID != null)
					{
						this.WriteSessionId();
					}
					else
					{
						if (addGetSessionToken)
						{
							this.WriteBeginGetSessionToken();
						}
						if (addCreateSession)
						{
							this.WriteBeginSession();
						}
						this.WriteVersionHeader();
					}
					if (sendNamespaceCompatibility)
					{
						this.writer.WriteRaw("<NamespaceCompatibility xmlns=\"http://schemas.microsoft.com/analysisservices/2003/xmla\" mustUnderstand=\"0\"/>");
					}
					this.writer.WriteEndElement();
					this.writer.WriteStartElement("Body");
				}
				catch (IOException innerException)
				{
					this.CloseAll();
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
				}
				catch
				{
					this.HandleMessageCreationException();
					throw;
				}
			}
			return this.writer;
		}

		internal void EndMessage()
		{
			this.VerifyIfCanWrite();
			if (!this.captureXml)
			{
				try
				{
					this.writer.WriteEndElement();
					this.writer.WriteEndElement();
				}
				catch (IOException innerException)
				{
					this.CloseAll();
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
				}
				catch
				{
					this.HandleMessageCreationException();
					throw;
				}
			}
		}

		private void VerifyIfCanWrite()
		{
			this.VerifyIfCanWrite(false);
		}

		private void VerifyIfCanWrite(bool useBinaryXml)
		{
			this.CheckConnection();
			if (this.reader != null)
			{
				throw new InvalidOperationException(XmlaSR.XmlaClient_SendRequest_ThereIsAnotherPendingResponse);
			}
			if (!useBinaryXml && this.writer == null)
			{
				throw new InvalidOperationException(XmlaSR.XmlaClient_SendRequest_NoRequestWasCreated);
			}
		}

		internal XmlWriter StartRequest(string action)
		{
			this.CheckConnection();
			if (this.writer != null)
			{
				throw new InvalidOperationException(XmlaSR.XmlaClient_StartRequest_ThereIsAnotherPendingRequest);
			}
			if (this.reader != null)
			{
				throw new InvalidOperationException(XmlaSR.XmlaClient_StartRequest_ThereIsAnotherPendingResponse);
			}
			XmlWriter result;
			try
			{
				if (this.captureXml)
				{
					this.logEntry = new StringWriter(CultureInfo.InvariantCulture);
					XmlTextWriter xmlTextWriter = this.writer = new XmlTextWriter(this.logEntry);
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
				}
				else
				{
					this.xmlaStream.WriteSoapActionHeader(action);
					DataType requestDataType = this.xmlaStream.GetRequestDataType();
					if (requestDataType == DataType.BinaryXml)
					{
						throw new NotSupportedException();
					}
					XmlTextWriter xmlTextWriter2 = this.writer = new XmlTextWriter(this.xmlaStream, this.connInfo.CharacterEncoding);
					xmlTextWriter2.Formatting = Formatting.Indented;
					xmlTextWriter2.Indentation = 2;
				}
				this.connectionState = ConnectionState.Executing;
				result = this.writer;
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			return result;
		}

		private void WriteEndOfMessage()
		{
			this.WriteEndOfMessage(false);
		}

		private void WriteEndOfMessage(bool callBaseDirect)
		{
			if (this.workbookSession != null)
			{
				try
				{
					this.workbookSession.BeginActivity();
				}
				catch (Exception innerException)
				{
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
				}
			}
			bool arg_2C_0 = XmlaClient.TRACESWITCH.TraceVerbose;
			if (callBaseDirect)
			{
				((CompressedStream)this.xmlaStream).BaseXmlaStream.WriteEndOfMessage();
			}
			else
			{
				this.xmlaStream.WriteEndOfMessage();
			}
			this.connectionState = ConnectionState.Fetching;
		}

		internal XmlReader EndRequest()
		{
			return this.EndRequest(false);
		}

		internal XmlReader EndRequest(bool useBinaryXml)
		{
			this.VerifyIfCanWrite(useBinaryXml);
			bool arg_11_0 = XmlaClient.TRACESWITCH.TraceVerbose;
			XmlReader result;
			try
			{
				lock (this.lockForCloseAll)
				{
					if (this.writer != null)
					{
						this.writer.Flush();
						this.writer.Close();
						this.writer = null;
					}
					if (useBinaryXml)
					{
						this.xmlaStream.Flush();
						this.xmlaStream.Close();
					}
				}
				if (this.captureXml)
				{
					this.captureLog.Add(this.logEntry.ToString());
					this.logEntry.Close();
					this.logEntry = null;
					this.connectionState = ConnectionState.Open;
					result = null;
					return result;
				}
				this.WriteEndOfMessage();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			try
			{
				DataType responseDataType = this.xmlaStream.GetResponseDataType();
				bool arg_E9_0 = XmlaClient.TRACESWITCH.TraceVerbose;
				XmlReader xmlReader;
				switch (responseDataType)
				{
				case DataType.TextXml:
				case DataType.CompressedXml:
				{
					Encoding encoding = Encoding.UTF8;
					bool detectEncodingFromByteOrderMarks = true;
					if (this.ConnectionInfo.IXMLAMode)
					{
						encoding = Encoding.Unicode;
						detectEncodingFromByteOrderMarks = false;
					}
					xmlReader = new XmlTextReader(new StreamReader(this.xmlaStream, encoding, detectEncodingFromByteOrderMarks), this.nameTable);
					((XmlTextReader)xmlReader).WhitespaceHandling = WhitespaceHandling.None;
					break;
				}
				case DataType.BinaryXml:
				case DataType.CompressedBinaryXml:
					xmlReader = SqlBinaryReaderCreator.CreateReader(this.xmlaStream);
					break;
				default:
					this.CloseAll();
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken);
				}
				if (this.connInfo.IXMLAMode)
				{
					this.reader = new XmlaReader(xmlReader, this, null)
					{
						SkipElements = false
					};
				}
				else
				{
					this.reader = new XmlaReader(xmlReader, this, this.namespacesManager);
				}
				this.CheckAndGetHttpStreamSoapFault();
				result = this.reader;
			}
			catch (AdomdConnectionException)
			{
				throw;
			}
			catch (IOException innerException2)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch
			{
				this.CloseAll();
				throw;
			}
			return result;
		}

		private void WriteStatement(string command)
		{
			this.CheckConnection();
			try
			{
				this.writer.WriteStartElement("Statement");
				this.writer.WriteString(command);
				this.writer.WriteEndElement();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
		}

		private void WriteCommandContent(string command)
		{
			try
			{
				this.writer.WriteRaw(command);
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
		}

		internal void WriteStartCommand(ref IDictionary commandProperties)
		{
			this.CheckConnection();
			try
			{
				if (!this.captureXml)
				{
					this.PopulateActivityIDAndRequestID(ref commandProperties, true);
					if (commandProperties == null)
					{
						commandProperties = new ListDictionary();
					}
					this.writer.WriteStartElement("Execute", "urn:schemas-microsoft-com:xml-analysis");
					this.writer.WriteStartElement("Command");
				}
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
		}

		private void PopulateActivityIDAndRequestID(ref IDictionary commandProperties, bool isCommand)
		{
			if (this.SupportsActivityIDAndRequestID)
			{
				if (commandProperties == null)
				{
					commandProperties = new ListDictionary();
				}
				bool flag = this.SupportsCurrentActivityID;
				object key = null;
				object key2 = null;
				object key3;
				if (commandProperties is AdomdPropertyCollectionInternal)
				{
					key3 = new XmlaPropertyKey("DbpropMsmdActivityID", null);
					key = new XmlaPropertyKey("DbpropMsmdRequestID", null);
					key2 = new XmlaPropertyKey("DbpropMsmdCurrentActivityID", null);
				}
				else
				{
					key3 = "DbpropMsmdActivityID";
					key = "DbpropMsmdRequestID";
					key2 = "DbpropMsmdCurrentActivityID";
				}
				if (isCommand)
				{
					if (!commandProperties.Contains(key3))
					{
						commandProperties.Add(key3, this.connInfo.ClientActivityID);
					}
					if (flag && !commandProperties.Contains(key2))
					{
						commandProperties.Add(key2, this.connInfo.CurrentActivityID);
					}
				}
				else
				{
					commandProperties[key3] = this.connInfo.ClientActivityID;
					if (flag)
					{
						commandProperties[key2] = this.connInfo.CurrentActivityID;
					}
				}
				try
				{
					if (commandProperties[key3] is AdomdProperty)
					{
						AdomdProperty adomdProperty = (AdomdProperty)commandProperties[key3];
						this.xmlaStream.ActivityID = (Guid)adomdProperty.Value;
					}
					else
					{
						this.xmlaStream.ActivityID = (Guid)commandProperties[key3];
					}
				}
				catch
				{
					this.xmlaStream.ActivityID = Guid.Empty;
				}
				if (flag)
				{
					try
					{
						if (commandProperties[key2] is AdomdProperty)
						{
							AdomdProperty adomdProperty2 = (AdomdProperty)commandProperties[key2];
							this.xmlaStream.CurrentActivityID = (Guid)adomdProperty2.Value;
						}
						else
						{
							this.xmlaStream.CurrentActivityID = (Guid)commandProperties[key2];
						}
					}
					catch
					{
						this.xmlaStream.CurrentActivityID = Guid.Empty;
					}
				}
				if (!commandProperties.Contains(key))
				{
					Guid guid = Guid.NewGuid();
					commandProperties[key] = guid;
					this.xmlaStream.RequestID = guid;
				}
			}
		}

		private bool SupportsProperty(string propName)
		{
			bool result;
			try
			{
				ListDictionary listDictionary = new ListDictionary();
				listDictionary.Add("PropertyName", propName);
				XmlReader xmlReader = this.Discover("DISCOVER_PROPERTIES", null, this.ConnectionInfo.ExtendedProperties, listDictionary, false, null);
				xmlReader.ReadStartElement("DiscoverResponse", "urn:schemas-microsoft-com:xml-analysis");
				xmlReader.ReadStartElement("return");
				xmlReader.ReadStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset");
				if (!xmlReader.IsStartElement("schema", "http://www.w3.org/2001/XMLSchema"))
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1}, got {2}", new object[]
					{
						"http://www.w3.org/2001/XMLSchema",
						"schema",
						xmlReader.Name
					}));
				}
				xmlReader.Skip();
				if (xmlReader.IsStartElement("row", "urn:schemas-microsoft-com:xml-analysis:rowset"))
				{
					xmlReader.ReadStartElement();
					if (xmlReader.IsStartElement("PropertyName", "urn:schemas-microsoft-com:xml-analysis:rowset"))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			finally
			{
				this.EndReceival(true);
			}
			return result;
		}

		internal void WriteEndCommand(IDictionary connectionProperties, IDictionary commandProperties, IDataParameterCollection parameters)
		{
			this.CheckConnection();
			try
			{
				if (!this.captureXml)
				{
					this.writer.WriteEndElement();
					this.WriteProperties(connectionProperties, commandProperties);
					this.WriteParameters(parameters);
					this.writer.WriteEndElement();
				}
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
		}

		internal virtual void WriteProperties(IDictionary connectionProperties, IDictionary commandProperties)
		{
			this.writer.WriteStartElement("Properties");
			this.writer.WriteStartElement("PropertyList");
			if (connectionProperties != null && connectionProperties.Count > 0)
			{
				foreach (DictionaryEntry propertyEntry in connectionProperties)
				{
					if (propertyEntry.Value != null && (commandProperties == null || commandProperties.Count <= 0 || !commandProperties.Contains(propertyEntry.Key)))
					{
						this.WriteVersionSafeProperty(propertyEntry);
					}
				}
			}
			if (commandProperties != null && commandProperties.Count > 0)
			{
				foreach (DictionaryEntry propertyEntry2 in commandProperties)
				{
					if (propertyEntry2.Value != null)
					{
						this.WriteVersionSafeProperty(propertyEntry2);
					}
				}
			}
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
		}

		private void WriteVersionSafeProperty(DictionaryEntry propertyEntry)
		{
			string text = (string)propertyEntry.Key;
			if (text.Equals("DbpropMsmdCurrentActivityID", StringComparison.OrdinalIgnoreCase))
			{
				if (this.SupportsCurrentActivityID)
				{
					this.writer.WriteElementString(text, FormattersHelpers.ConvertToXml(propertyEntry.Value));
					return;
				}
			}
			else if (text.Equals("DbpropMsmdRequestID", StringComparison.OrdinalIgnoreCase))
			{
				if (this.SupportsActivityIDAndRequestID)
				{
					this.writer.WriteElementString(text, FormattersHelpers.ConvertToXml(propertyEntry.Value));
					return;
				}
			}
			else if (text.Equals("DbpropMsmdActivityID", StringComparison.OrdinalIgnoreCase))
			{
				if (this.SupportsActivityIDAndRequestID)
				{
					this.writer.WriteElementString(text, FormattersHelpers.ConvertToXml(propertyEntry.Value));
					return;
				}
			}
			else if (text.Equals("DbPropmsmdRequestMemoryLimit", StringComparison.OrdinalIgnoreCase))
			{
				if (!this.ConnectionInfo.IsOnPremFromCloudAccess)
				{
					this.writer.WriteElementString(text, FormattersHelpers.ConvertToXml(propertyEntry.Value));
					return;
				}
			}
			else
			{
				this.writer.WriteElementString(text, FormattersHelpers.ConvertToXml(propertyEntry.Value));
			}
		}

		internal virtual void WriteXmlaProperty(DictionaryEntry entry)
		{
			this.writer.WriteElementString((string)entry.Key, FormattersHelpers.ConvertToXml(entry.Value));
		}

		private void WriteParameters(IDataParameterCollection parameters)
		{
			if (parameters != null && parameters.Count > 0)
			{
				this.writer.WriteStartElement("Parameters");
				this.writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
				this.writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
				foreach (IDataParameter dataParameter in parameters)
				{
					if (dataParameter.Value != null)
					{
						this.writer.WriteStartElement("Parameter");
						this.writer.WriteElementString("Name", dataParameter.ParameterName);
						if (typeof(IDataReader).IsInstanceOfType(dataParameter.Value) || typeof(DataTable).IsInstanceOfType(dataParameter.Value))
						{
							this.WriteTabularParameterValue(dataParameter.Value);
						}
						else
						{
							this.writer.WriteStartElement("Value");
							if (typeof(DBNull) == dataParameter.Value.GetType())
							{
								this.writer.WriteAttributeString("xsi:nil", "true");
							}
							else
							{
								this.writer.WriteAttributeString("xsi:type", XmlTypeMapper.GetXmlType(dataParameter.Value.GetType()));
								this.writer.WriteString(FormattersHelpers.ConvertToXml(dataParameter.Value));
							}
							this.writer.WriteEndElement();
						}
						this.writer.WriteEndElement();
					}
				}
				this.writer.WriteEndElement();
			}
		}

		private void WriteTabularParameterValue(object value)
		{
			IDataReader dataReader = null;
			if (typeof(DataTable).IsInstanceOfType(value))
			{
				DataTable dataTable = (DataTable)value;
				dataReader = dataTable.CreateDataReader();
			}
			if (typeof(IDataReader).IsInstanceOfType(value))
			{
				dataReader = (IDataReader)value;
			}
			this.writer.WriteStartElement("Value");
			this.writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:xml-analysis:rowset");
			this.writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
			this.writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
			this.WriteTabularSchema(dataReader);
			while (dataReader.Read())
			{
				this.writer.WriteStartElement("row");
				for (int i = 0; i < dataReader.FieldCount; i++)
				{
					object value2 = dataReader.GetValue(i);
					if (value2 != null && !dataReader.IsDBNull(i))
					{
						this.writer.WriteStartElement(XmlConvert.EncodeLocalName(dataReader.GetName(i)));
						this.writer.WriteString(FormattersHelpers.ConvertToXml(value2));
						this.writer.WriteEndElement();
					}
				}
				this.writer.WriteEndElement();
			}
			this.writer.WriteEndElement();
		}

		private void WriteTabularSchema(IDataReader dataReader)
		{
			DataTable schemaTable = dataReader.GetSchemaTable();
			this.writer.WriteStartElement("schema", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("targetNamespace", "urn:schemas-microsoft-com:xml-analysis:rowset");
			this.writer.WriteAttributeString("elementFormDefault", "qualified");
			this.writer.WriteAttributeString("xmlns:sql", "urn:schemas-microsoft-com:xml-sql");
			this.writer.WriteStartElement("element", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("name", "root");
			this.writer.WriteStartElement("complexType", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteStartElement("sequence", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("minOccurs", "0");
			this.writer.WriteAttributeString("maxOccurs", "unbounded");
			this.writer.WriteStartElement("element", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("name", "row");
			this.writer.WriteAttributeString("type", "row");
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
			this.writer.WriteStartElement("simpleType", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("name", "uuid");
			this.writer.WriteStartElement("restriction", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("base", "xsd:string");
			this.writer.WriteStartElement("pattern", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("value", "[0-9a-zA-Z]{8}-[0-9a-zA-Z]{4}-[0-9a-zA-Z]{4}-[0-9a-zA-Z]{4}-[0-9a-zA-Z]{12}");
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
			this.writer.WriteStartElement("complexType", "http://www.w3.org/2001/XMLSchema");
			this.writer.WriteAttributeString("name", "row");
			this.writer.WriteStartElement("sequence", "http://www.w3.org/2001/XMLSchema");
			for (int i = 0; i < schemaTable.Rows.Count; i++)
			{
				DataRow dataRow = schemaTable.Rows[i];
				this.writer.WriteStartElement("element", "http://www.w3.org/2001/XMLSchema");
				this.writer.WriteAttributeString("sql:field", dataRow["ColumnName"].ToString());
				this.writer.WriteAttributeString("name", XmlConvert.EncodeLocalName(dataRow["ColumnName"].ToString()));
				this.writer.WriteAttributeString("type", XmlTypeMapper.GetXmlType((Type)dataRow["DataType"]));
				if ((bool)dataRow["AllowDBNull"])
				{
					this.writer.WriteAttributeString("minOccurs", "0");
				}
				this.writer.WriteEndElement();
			}
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
			this.writer.WriteEndElement();
		}

		private void WriteRestrictions(IDictionary restrictions)
		{
			this.writer.WriteStartElement("Restrictions");
			if (restrictions != null && restrictions.Count > 0)
			{
				this.writer.WriteStartElement("RestrictionList");
				foreach (DictionaryEntry entry in restrictions)
				{
					if (entry.Value != null)
					{
						this.WriteXmlaProperty(entry);
					}
				}
				this.writer.WriteEndElement();
			}
			this.writer.WriteEndElement();
		}

		internal void WriteStartDiscover(string requestType, string requestNamespace)
		{
			this.CheckConnection();
			try
			{
				this.writer.WriteStartElement("Discover", "urn:schemas-microsoft-com:xml-analysis");
				if (requestNamespace == null || requestNamespace.Length == 0)
				{
					this.writer.WriteElementString("RequestType", requestType);
				}
				else
				{
					this.writer.WriteStartElement("RequestType");
					this.writer.WriteAttributeString("xmlns", "rt", null, requestNamespace);
					this.writer.WriteString("rt:" + requestType);
					this.writer.WriteEndElement();
				}
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
		}

		internal void WriteEndDiscover(ListDictionary properties)
		{
			this.WriteEndDiscover(properties, null);
		}

		internal void WriteEndDiscover(ListDictionary properties, IDictionary requestProperties)
		{
			this.CheckConnection();
			try
			{
				IDictionary dictionary = properties;
				this.PopulateActivityIDAndRequestID(ref dictionary, false);
				this.WriteProperties(properties, requestProperties);
				this.writer.WriteEndElement();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
		}

		private XmlReader Discover(string requestType, string requestNamespace, ListDictionary properties, IDictionary restrictions, bool sendNamespacesCompatibility, IDictionary requestProperties)
		{
			this.CheckConnection();
			try
			{
				this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Discover\"", false, sendNamespacesCompatibility, false);
				this.WriteStartDiscover(requestType, requestNamespace);
				this.WriteRestrictions(restrictions);
				this.WriteEndDiscover(properties, requestProperties);
				this.EndMessage();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			return this.SendMessage(true, false, sendNamespacesCompatibility);
		}

		internal static XmlaResult ReadToXmlaResponse(XmlReader reader)
		{
			new XmlaResultCollection();
			reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
			if (reader.IsStartElement("Header", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				reader.Skip();
			}
			reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
			XmlaResult xmlaResult = new XmlaResult();
			if (XmlaClient.CheckForSoapFault(reader, xmlaResult, false))
			{
				return xmlaResult;
			}
			return null;
		}

		internal static XmlaResultCollection ReadResponse(XmlReader reader, bool skipResults, bool throwIfError)
		{
			XmlaResultCollection xmlaResultCollection = new XmlaResultCollection();
			XmlaResult xmlaResult = new XmlaResult();
			reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
			if (reader.IsStartElement("Header", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				reader.Skip();
			}
			reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
			if (XmlaClient.CheckForSoapFault(reader, xmlaResult, false))
			{
				xmlaResultCollection.Add(xmlaResult);
			}
			else
			{
				XmlaClient.ReadExecuteResponsePrivate(reader, true, xmlaResultCollection, xmlaResult);
			}
			XmlaClient.CheckEndElement(reader, "Body", "http://schemas.xmlsoap.org/soap/envelope/");
			reader.ReadEndElement();
			XmlaClient.CheckEndElement(reader, "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
			reader.ReadEndElement();
			if (throwIfError && xmlaResultCollection.ContainsErrors)
			{
				throw XmlaResultCollection.ExceptionOnError(xmlaResultCollection);
			}
			return xmlaResultCollection;
		}

		internal XmlaResultCollection SendExecuteAndReadResponse(bool skipResults, bool throwIfError)
		{
			return this.SendExecuteAndReadResponse(skipResults, throwIfError, false);
		}

		internal XmlaResultCollection SendExecuteAndReadResponse(bool skipResults, bool throwIfError, bool useBinaryXml)
		{
			bool arg_0A_0 = XmlaClient.TRACESWITCH.TraceVerbose;
			this.EndRequest(useBinaryXml);
			if (this.captureXml)
			{
				return null;
			}
			XmlaResultCollection result;
			try
			{
				XmlaResultCollection xmlaResultCollection = new XmlaResultCollection();
				XmlaResult xmlaResult = new XmlaResult();
				this.reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
				if (this.reader.IsStartElement("Header", "http://schemas.xmlsoap.org/soap/envelope/"))
				{
					this.reader.Skip();
				}
				this.reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
				if (XmlaClient.CheckForSoapFault(this.reader, xmlaResult, false))
				{
					xmlaResultCollection.Add(xmlaResult);
				}
				else
				{
					XmlaClient.ReadExecuteResponsePrivate(this.reader, skipResults, xmlaResultCollection, xmlaResult);
				}
				XmlaClient.CheckEndElement(this.reader, "Body", "http://schemas.xmlsoap.org/soap/envelope/");
				this.reader.ReadEndElement();
				XmlaClient.CheckEndElement(this.reader, "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
				this.reader.ReadEndElement();
				if (throwIfError && xmlaResultCollection.ContainsErrors)
				{
					throw XmlaResultCollection.ExceptionOnError(xmlaResultCollection);
				}
				result = xmlaResultCollection;
			}
			catch (XmlException innerException)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (AdomdUnknownResponseException)
			{
				throw;
			}
			catch (XmlaException)
			{
				throw;
			}
			catch
			{
				this.CloseAll();
				throw;
			}
			finally
			{
				if (this.connected)
				{
					this.EndReceival(true);
				}
			}
			return result;
		}

		private static void ReadExecuteResponsePrivate(XmlReader reader, bool skipResults, XmlaResultCollection results, XmlaResult xmlaResult)
		{
			reader.ReadStartElement("ExecuteResponse", "urn:schemas-microsoft-com:xml-analysis");
			reader.ReadStartElement("return", "urn:schemas-microsoft-com:xml-analysis");
			if (reader.IsStartElement("results", "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults"))
			{
				XmlaClient.ReadMultipleResults(reader, results, skipResults);
			}
			else
			{
				XmlaClient.ReadRoot(reader, xmlaResult, skipResults);
				results.Add(xmlaResult);
			}
			XmlaClient.CheckEndElement(reader, "return", "urn:schemas-microsoft-com:xml-analysis");
			reader.ReadEndElement();
			XmlaClient.CheckEndElement(reader, "ExecuteResponse", "urn:schemas-microsoft-com:xml-analysis");
			reader.ReadEndElement();
		}

		internal static bool IsMultipleResult(XmlReader reader)
		{
			return reader.IsStartElement("results", "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
		}

		internal static bool IsAffectedObjects(XmlReader reader)
		{
			return reader.IsStartElement("AffectedObjects", "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
		}

		private static void ReadMultipleResults(XmlReader reader, XmlaResultCollection results, bool skipResults)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			string localname = XmlaClient.IsAffectedObjects(reader) ? "AffectedObjects" : "results";
			reader.ReadStartElement(localname, "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
			while (reader.IsStartElement())
			{
				XmlaResult xmlaResult = new XmlaResult();
				XmlaClient.ReadRoot(reader, xmlaResult, skipResults);
				results.Add(xmlaResult);
			}
			XmlaClient.CheckEndElement(reader, localname, "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
			reader.ReadEndElement();
		}

		private static void ReadRoot(XmlReader reader, XmlaResult xmlaResult, bool skipResults)
		{
			if (reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:empty"))
			{
				XmlaClient.ReadEmptyRoot(reader, xmlaResult, skipResults);
				return;
			}
			if (reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset"))
			{
				XmlaClient.ReadRowsetRoot(reader, xmlaResult, skipResults);
				return;
			}
			if (reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:mddataset"))
			{
				XmlaClient.ReadDatasetRoot(reader, xmlaResult, skipResults);
				return;
			}
			throw new AdomdUnknownResponseException(XmlaSR.UnexpectedElement(reader.LocalName, reader.NamespaceURI), "Expected root element");
		}

		private static void ReadEmptyRoot(XmlReader reader, XmlaResult xmlaResult, bool skipResults)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				if (!skipResults)
				{
					xmlaResult.SetValue(string.Empty);
					return;
				}
			}
			else
			{
				reader.ReadStartElement("root", "urn:schemas-microsoft-com:xml-analysis:empty");
				XmlaClient.CheckAndSkipXsdSchema(reader);
				if (!XmlaClient.CheckForException(reader, xmlaResult, false))
				{
					XmlaClient.CheckForMessages(reader, xmlaResult.Messages);
				}
				if (!XmlaClient.IsEndElement(reader, "root", "urn:schemas-microsoft-com:xml-analysis:empty"))
				{
					throw new AdomdUnknownResponseException(XmlaSR.EmptyRootIsNotEmpty, string.Format(CultureInfo.InvariantCulture, "Expected end of {0}:{1} element, got {2}", new object[]
					{
						"urn:schemas-microsoft-com:xml-analysis:empty",
						"root",
						reader.Name
					}));
				}
				reader.ReadEndElement();
			}
		}

		private static bool IsEndElement(XmlReader reader, string localName, string ns)
		{
			reader.MoveToContent();
			return reader.NodeType == XmlNodeType.EndElement && reader.LocalName == localName && reader.NamespaceURI == ns;
		}

		private static bool CheckAndSkipXsdSchema(XmlReader reader)
		{
			if (reader.IsStartElement("schema", "http://www.w3.org/2001/XMLSchema"))
			{
				reader.Skip();
				return true;
			}
			return false;
		}

		private static void ReadRowsetRoot(XmlReader reader, XmlaResult xmlaResult, bool skipResults)
		{
			if (skipResults)
			{
				reader.Skip();
				return;
			}
			xmlaResult.SetValue(reader.ReadInnerXml());
		}

		private static void ReadDatasetRoot(XmlReader reader, XmlaResult xmlaResult, bool skipResults)
		{
			if (skipResults)
			{
				reader.Skip();
				return;
			}
			xmlaResult.SetValue(reader.ReadInnerXml());
		}

		public void CancelCommand(string sessionID)
		{
			if (sessionID == null || sessionID.Length == 0)
			{
				throw new ArgumentException(XmlaSR.Cancel_SessionIDNotSpecified);
			}
			this.CheckConnection();
			string text = this.SessionID;
			try
			{
				this.SessionID = sessionID;
				this.StartMessage("SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"");
				IDictionary commandProperties = null;
				this.WriteStartCommand(ref commandProperties);
				this.writer.WriteStartElement("Cancel", "http://schemas.microsoft.com/analysisservices/2003/engine");
				this.writer.WriteEndElement();
				this.WriteEndCommand(this.ConnectionInfo.ExtendedProperties, commandProperties, null);
				this.EndMessage();
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.HandleMessageCreationException();
				throw;
			}
			finally
			{
				this.SessionID = text;
			}
			this.SendExecuteAndReadResponse(false, true);
		}

		private string PopulateActivityIDAndRequestIDIntoProperties(string properties, IDictionary commandProperties, bool propertiesXmlIsComplete)
		{
			if (this.SupportsActivityIDAndRequestID)
			{
				if (propertiesXmlIsComplete)
				{
					if (string.IsNullOrEmpty(properties))
					{
						properties = string.Format(CultureInfo.InvariantCulture, "<{0}></{0}>", new object[]
						{
							"PropertyList"
						});
					}
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(properties);
					XmlNode xmlNode = xmlDocument.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "/{0}", new object[]
					{
						"PropertyList"
					}));
					if (xmlNode != null && commandProperties != null && commandProperties.Contains("DbpropMsmdActivityID"))
					{
						XmlElement xmlElement = xmlDocument.CreateElement("DbpropMsmdActivityID");
						xmlElement.InnerText = commandProperties["DbpropMsmdActivityID"].ToString();
						XmlElement xmlElement2 = xmlDocument.CreateElement("DbpropMsmdRequestID");
						xmlElement2.InnerText = commandProperties["DbpropMsmdRequestID"].ToString();
						xmlNode.AppendChild(xmlElement);
						xmlNode.AppendChild(xmlElement2);
						if (this.SupportsCurrentActivityID && commandProperties.Contains("DbpropMsmdCurrentActivityID"))
						{
							XmlElement newChild = xmlDocument.CreateElement("DbpropMsmdCurrentActivityID");
							xmlElement2.InnerText = commandProperties["DbpropMsmdCurrentActivityID"].ToString();
							xmlNode.AppendChild(newChild);
						}
					}
					return xmlDocument.OuterXml;
				}
				if (commandProperties != null && commandProperties.Contains("DbpropMsmdActivityID"))
				{
					string text = string.Format(CultureInfo.InvariantCulture, "<{0}>{1}</{0}>", new object[]
					{
						"DbpropMsmdActivityID",
						commandProperties["DbpropMsmdActivityID"].ToString()
					});
					string text2 = string.Format(CultureInfo.InvariantCulture, "<{0}>{1}</{0}>", new object[]
					{
						"DbpropMsmdRequestID",
						commandProperties["DbpropMsmdRequestID"].ToString()
					});
					properties = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
					{
						properties,
						text,
						text2
					});
					if (this.SupportsCurrentActivityID && commandProperties.Contains("DbpropMsmdCurrentActivityID"))
					{
						string text3 = string.Format(CultureInfo.InvariantCulture, "<{0}>{1}</{0}>", new object[]
						{
							"DbpropMsmdCurrentActivityID",
							commandProperties["DbpropMsmdCurrentActivityID"].ToString()
						});
						properties = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
						{
							properties,
							text3
						});
					}
				}
			}
			return properties;
		}

		internal XmlReader SendMessage(bool endReceivalIfException, bool readSession, bool readNamespaceCompatibility)
		{
			if (XmlaClient.TRACESWITCH.TraceVerbose)
			{
				StackTrace stackTrace = new StackTrace();
				stackTrace.GetFrame(1).GetMethod();
			}
			this.EndRequest();
			if (this.captureXml)
			{
				if (XmlaClient.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace2 = new StackTrace();
					stackTrace2.GetFrame(1).GetMethod();
				}
				return null;
			}
			XmlReader result;
			try
			{
				this.reader.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
				this.ReadEnvelopeHeader(this.reader, readSession, readNamespaceCompatibility);
				this.reader.ReadStartElement("Body", "http://schemas.xmlsoap.org/soap/envelope/");
				XmlaClient.CheckForError(this.reader, new XmlaResult(), true);
				if (XmlaClient.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace3 = new StackTrace();
					stackTrace3.GetFrame(1).GetMethod();
				}
				result = this.reader;
			}
			catch (XmlException innerException)
			{
				if (endReceivalIfException)
				{
					this.EndReceival(true);
				}
				if (XmlaClient.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace4 = new StackTrace();
					stackTrace4.GetFrame(1).GetMethod();
				}
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
			catch (IOException innerException2)
			{
				this.CloseAll();
				if (XmlaClient.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace5 = new StackTrace();
					stackTrace5.GetFrame(1).GetMethod();
				}
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
			}
			catch (AdomdUnknownResponseException)
			{
				if (endReceivalIfException)
				{
					this.EndReceival(true);
				}
				if (XmlaClient.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace6 = new StackTrace();
					stackTrace6.GetFrame(1).GetMethod();
				}
				throw;
			}
			catch (XmlaException)
			{
				if (endReceivalIfException)
				{
					this.EndReceival(true);
				}
				if (XmlaClient.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace7 = new StackTrace();
					stackTrace7.GetFrame(1).GetMethod();
				}
				throw;
			}
			catch (Exception)
			{
				this.CloseAll();
				if (XmlaClient.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace8 = new StackTrace();
					stackTrace8.GetFrame(1).GetMethod();
				}
				throw;
			}
			return result;
		}

		internal static void CheckEndElement(XmlReader reader, string localname)
		{
			reader.MoveToContent();
			if (reader.LocalName != localname)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected end of {0} element, got {1}", new object[]
				{
					localname,
					reader.Name
				}));
			}
		}

		internal static void CheckEndElement(XmlReader reader, string localname, string ns)
		{
			reader.MoveToContent();
			if (reader.LocalName != localname || reader.NamespaceURI != ns)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Exected end of {0}:{1} element, got {2}", new object[]
				{
					ns,
					localname,
					reader.Name
				}));
			}
		}

		internal static void CheckAndSkipEmptyElement(XmlReader reader, string localname, string ns)
		{
			if (!reader.IsStartElement(localname, ns))
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1} element, got {2}", new object[]
				{
					ns,
					localname,
					reader.Name
				}));
			}
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			if (!XmlaClient.IsEndElement(reader, localname, ns))
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected end of {0}:{1} element, got {2}", new object[]
				{
					ns,
					localname,
					reader.Name
				}));
			}
			reader.ReadEndElement();
		}

		internal static bool IsEmptyResultS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:empty");
		}

		internal static bool IsExecuteResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("ExecuteResponse", "urn:schemas-microsoft-com:xml-analysis");
		}

		internal static bool IsDiscoverResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("DiscoverResponse", "urn:schemas-microsoft-com:xml-analysis");
		}

		internal static bool IsDatasetResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:mddataset");
		}

		internal static bool IsRowsetResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset");
		}

		internal static bool IsMultipleResultResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("results", "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
		}

		internal static bool IsAffectedObjectsResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("AffectedObjects", "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
		}

		internal static void ReadUptoRoot(XmlReader reader)
		{
			if (XmlaClient.IsExecuteResponseS(reader))
			{
				XmlaClient.StartExecuteResponseS(reader);
			}
			else if (XmlaClient.IsDiscoverResponseS(reader))
			{
				XmlaClient.StartDiscoverResponseS(reader);
			}
			else if (!XmlaClient.IsEmptyResultS(reader) && !XmlaClient.IsRootElementS(reader))
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected execute response, discover response, empty result, or root, got {0}", new object[]
				{
					reader.Name
				}));
			}
			if (!reader.EOF)
			{
				reader.MoveToContent();
			}
		}

		private static void SkipXmlaMessages(XmlReader reader)
		{
			if (reader.IsStartElement("Messages", "urn:schemas-microsoft-com:xml-analysis:exception"))
			{
				XmlaMessageCollection xmlaMessages = new XmlaMessageCollection();
				XmlaClient.ReadXmlaMessages(reader, xmlaMessages);
			}
		}

		internal static void ReadEmptyRootS(XmlReader reader)
		{
			XmlaResult xmlaResult = new XmlaResult();
			XmlaClient.ReadEmptyRoot(reader, xmlaResult, true);
			if (xmlaResult.ContainsErrors)
			{
				throw XmlaResultCollection.ExceptionOnError(xmlaResult);
			}
		}

		internal static void StartElementS(XmlReader reader, string element, string xmlNamespace)
		{
			XmlaClient.CheckForException(reader, null, true);
			reader.ReadStartElement(element, xmlNamespace);
		}

		internal static void EndElementS(XmlReader reader, string element, string xmlNamespace)
		{
			XmlaClient.CheckForException(reader, null, true);
			XmlaClient.ReadEndElementS(reader, element, xmlNamespace);
		}

		internal static void StartExecuteResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			reader.ReadStartElement("ExecuteResponse", "urn:schemas-microsoft-com:xml-analysis");
			XmlaClient.CheckForException(reader, null, true);
			reader.ReadStartElement("return", "urn:schemas-microsoft-com:xml-analysis");
		}

		internal static void EndExecuteResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			XmlaClient.ReadEndElementS(reader, "return", "urn:schemas-microsoft-com:xml-analysis");
			XmlaClient.CheckForException(reader, null, true);
			XmlaClient.ReadEndElementS(reader, "ExecuteResponse", "urn:schemas-microsoft-com:xml-analysis");
		}

		internal static void StartDiscoverResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			reader.ReadStartElement("DiscoverResponse", "urn:schemas-microsoft-com:xml-analysis");
			XmlaClient.CheckForException(reader, null, true);
			reader.ReadStartElement("return", "urn:schemas-microsoft-com:xml-analysis");
		}

		internal static void EndDiscoverResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			XmlaClient.ReadEndElementS(reader, "return", "urn:schemas-microsoft-com:xml-analysis");
			XmlaClient.CheckForException(reader, null, true);
			XmlaClient.ReadEndElementS(reader, "DiscoverResponse", "urn:schemas-microsoft-com:xml-analysis");
		}

		internal static void StartDatasetResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			reader.ReadStartElement("root", "urn:schemas-microsoft-com:xml-analysis:mddataset");
		}

		internal static void EndDatasetResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			XmlaClient.SkipXmlaMessages(reader);
			XmlaClient.ReadEndElementS(reader, "root", "urn:schemas-microsoft-com:xml-analysis:mddataset");
		}

		internal static void StartRowsetResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			reader.ReadStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset");
		}

		internal static void EndRowsetResponseS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			XmlaClient.SkipXmlaMessages(reader);
			XmlaClient.ReadEndElementS(reader, "root", "urn:schemas-microsoft-com:xml-analysis:rowset");
		}

		internal static void ReadEndElementS(XmlReader reader, string name, string ns)
		{
			XmlaClient.CheckForException(reader, null, true);
			if (reader.MoveToContent() != XmlNodeType.EndElement || reader.LocalName != name || reader.NamespaceURI != ns)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1}, got {2}", new object[]
				{
					ns,
					name,
					reader.Name
				}));
			}
			reader.ReadEndElement();
		}

		internal static bool IsRootElementS(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
			return reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:empty") || reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset") || reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:mddataset");
		}

		internal void EndReceival()
		{
			this.EndReceival(true);
		}

		internal void EndReceival(bool closeReader)
		{
			try
			{
				if (this.xmlaStream != null && this.connected && this.connectionState == ConnectionState.Fetching)
				{
					this.xmlaStream.Skip();
					this.connectionState = ConnectionState.Open;
				}
				lock (this.lockForCloseAll)
				{
					if (this.reader != null)
					{
						if (closeReader)
						{
							this.reader.Close();
						}
						this.reader = null;
					}
					if (this.workbookSession != null)
					{
						this.workbookSession.EndActivity();
					}
				}
			}
			catch (IOException innerException)
			{
				this.CloseAll();
				throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException);
			}
			catch
			{
				this.CloseAll();
				throw;
			}
		}

		private static int GetInstancePort(ConnectionInfo connectionInfo)
		{
			int result;
			if (connectionInfo.InstanceName == null)
			{
				if (connectionInfo.Port == null)
				{
					return 2383;
				}
				try
				{
					result = int.Parse(connectionInfo.Port, CultureInfo.InvariantCulture);
					return result;
				}
				catch (FormatException innerException)
				{
					throw new AdomdConnectionException(XmlaSR.ConnectionString_Invalid, innerException);
				}
				catch (OverflowException innerException2)
				{
					throw new AdomdConnectionException(XmlaSR.ConnectionString_Invalid, innerException2);
				}
			}
			ConnectionInfo connectionInfo2 = connectionInfo.CloneForInstanceLookup();
			XmlaClient xmlaClient = new XmlaClient();
			xmlaClient.Connect(connectionInfo2, false);
			try
			{
				ListDictionary listDictionary = new ListDictionary();
				listDictionary.Add("INSTANCE_NAME", connectionInfo.InstanceName);
				XmlReader xmlReader = xmlaClient.Discover("DISCOVER_INSTANCES", null, connectionInfo2.ExtendedProperties, listDictionary, false, null);
				xmlReader.ReadStartElement("DiscoverResponse", "urn:schemas-microsoft-com:xml-analysis");
				xmlReader.ReadStartElement("return");
				xmlReader.ReadStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset");
				if (!xmlReader.IsStartElement("schema", "http://www.w3.org/2001/XMLSchema"))
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1} element, got {2}", new object[]
					{
						"http://www.w3.org/2001/XMLSchema",
						"schema",
						xmlReader.Name
					}));
				}
				xmlReader.Skip();
				if (xmlReader.IsStartElement("row", "urn:schemas-microsoft-com:xml-analysis:rowset"))
				{
					xmlReader.ReadStartElement();
					while (xmlReader.IsStartElement())
					{
						if (xmlReader.IsStartElement("INSTANCE_PORT_NUMBER", "urn:schemas-microsoft-com:xml-analysis:rowset"))
						{
							int num;
							if (int.TryParse(xmlReader.ReadElementString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
							{
								result = num;
								return result;
							}
							break;
						}
						else
						{
							xmlReader.Skip();
						}
					}
				}
				throw new AdomdConnectionException(XmlaSR.Instance_NotFound(connectionInfo.InstanceName, connectionInfo.Server));
			}
			finally
			{
				xmlaClient.Disconnect(false);
			}
			return result;
		}

		private static TcpClient GetTcpClient(ConnectionInfo connectionInfo)
		{
			int instancePort = XmlaClient.GetInstancePort(connectionInfo);
			string text = connectionInfo.Location;
			if (text == null)
			{
				text = (connectionInfo.IsServerLocal ? "localhost" : connectionInfo.Server);
			}
			if (string.Compare(text, Environment.MachineName, StringComparison.OrdinalIgnoreCase) == 0 && XmlaClient.InternalGetComputerName(ComputerNameFormat.ComputerNameNetBIOS) == XmlaClient.InternalGetComputerName(ComputerNameFormat.ComputerNamePhysicalNetBIOS))
			{
				text = "localhost";
			}
			TcpClient tcpClient = null;
			try
			{
				IPAddress iPAddress;
				if (IPAddress.TryParse(text, out iPAddress))
				{
					tcpClient = new TcpClient(iPAddress.AddressFamily);
					tcpClient.Connect(iPAddress, instancePort);
				}
				else
				{
					tcpClient = new TcpClient(text, instancePort);
				}
			}
			catch (ArgumentNullException innerException)
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_Invalid, innerException);
			}
			catch (ArgumentOutOfRangeException innerException2)
			{
				throw new AdomdConnectionException(XmlaSR.ConnectionString_Invalid, innerException2);
			}
			catch (ArgumentException innerException3)
			{
				throw new AdomdConnectionException(connectionInfo.IsForSqlBrowser ? XmlaSR.CannotConnectToRedirector : XmlaSR.CannotConnect, innerException3);
			}
			catch (SocketException innerException4)
			{
				throw new AdomdConnectionException(connectionInfo.IsForSqlBrowser ? XmlaSR.CannotConnectToRedirector : XmlaSR.CannotConnect, innerException4);
			}
			tcpClient.NoDelay = true;
			tcpClient.ReceiveBufferSize = XmlaClient.BufferSizeForNetworkStream;
			tcpClient.SendBufferSize = XmlaClient.BufferSizeForNetworkStream;
			return tcpClient;
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GetComputerNameEx([MarshalAs(UnmanagedType.I4)] [In] int nameFormat, [MarshalAs(UnmanagedType.LPTStr)] [Out] StringBuilder name, [In] [Out] ref int size);

		private static string InternalGetComputerName(ComputerNameFormat nameFormat)
		{
			int capacity = 64;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			if (!XmlaClient.GetComputerNameEx((int)nameFormat, stringBuilder, ref capacity))
			{
				int lastError = (int)UnsafeNclNativeMethods.GetLastError();
				throw new AdomdConnectionException(XmlaSR.CannotConnect, new Win32Exception(lastError));
			}
			return stringBuilder.ToString();
		}

		private void SetReadWriteTimeouts(int readTimeoutMilliseconds, int writeTimeoutMilliseconds)
		{
			this.tcpClient.ReceiveTimeout = ((writeTimeoutMilliseconds == -1 || writeTimeoutMilliseconds < 0) ? 0 : writeTimeoutMilliseconds);
			this.tcpClient.SendTimeout = ((readTimeoutMilliseconds == -1 || readTimeoutMilliseconds < 0) ? 0 : readTimeoutMilliseconds);
		}

		internal void CheckConnection()
		{
			if (this.connected || this.captureXml)
			{
				this.CheckIfReaderDetached();
				return;
			}
			if (this.userOpened)
			{
				throw new AdomdConnectionException(XmlaSR.NotConnected);
			}
			throw new InvalidOperationException(XmlaSR.NotConnected);
		}

		private void CheckIfReaderDetached()
		{
			if (this.IsReaderDetached)
			{
				throw new InvalidOperationException(XmlaSR.ConnectionCannotBeUsedWhileXmlReaderOpened);
			}
		}

		internal static XmlReader GetReaderToReturnToPublic(XmlReader reader)
		{
			XmlaReader xmlaReader = reader as XmlaReader;
			if (xmlaReader != null && !xmlaReader.IsReaderDetached)
			{
				xmlaReader.DetachReader();
			}
			return reader;
		}

		internal void HandleMessageCreationException()
		{
			if (this.captureXml)
			{
				this.connectionState = ConnectionState.Closed;
				if (this.writer != null)
				{
					this.writer.Close();
					this.writer = null;
				}
				if (this.logEntry != null)
				{
					this.logEntry.Close();
					this.logEntry = null;
					return;
				}
			}
			else
			{
				this.CloseAll();
			}
		}

		private void WriteBeginSession()
		{
			this.writer.WriteStartElement("BeginSession", "urn:schemas-microsoft-com:xml-analysis");
			this.writer.WriteAttributeString("soap", "mustUnderstand", "http://schemas.xmlsoap.org/soap/envelope/", "1");
			this.writer.WriteEndElement();
		}

		private void WriteBeginGetSessionToken()
		{
			this.writer.WriteStartElement("BeginGetSessionToken", "http://schemas.microsoft.com/analysisservices/2003/xmla");
			this.writer.WriteAttributeString("soap", "mustUnderstand", "http://schemas.xmlsoap.org/soap/envelope/", "1");
			this.writer.WriteEndElement();
		}

		private void WriteSessionId()
		{
			this.writer.WriteStartElement("XA", "Session", "urn:schemas-microsoft-com:xml-analysis");
			this.writer.WriteAttributeString("soap", "mustUnderstand", "http://schemas.xmlsoap.org/soap/envelope/", "1");
			this.writer.WriteAttributeString("SessionId", this.SessionID);
			this.writer.WriteEndElement();
		}

		private void WriteVersionHeader()
		{
			this.writer.WriteStartElement("Version", "http://schemas.microsoft.com/analysisservices/2003/engine/2");
			this.writer.WriteAttributeString("Sequence", "500");
			this.writer.WriteEndElement();
		}

		private void ReadEnvelopeHeader(XmlReader reader, bool readSession, bool readNamepsaceCompatibility)
		{
			if (reader.IsStartElement("Header", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				reader.ReadStartElement();
				while (reader.IsStartElement())
				{
					if (reader.IsStartElement("Session", "urn:schemas-microsoft-com:xml-analysis"))
					{
						if (readSession)
						{
							this.SessionID = reader.GetAttribute("SessionId");
							XmlaClient.CheckAndSkipEmptyElement(reader, "Session", "urn:schemas-microsoft-com:xml-analysis");
						}
						else
						{
							reader.Skip();
						}
					}
					else if (reader.IsStartElement("Session", "http://schemas.xmlsoap.org/soap/envelope/"))
					{
						if (readSession)
						{
							this.SessionID = reader.GetAttribute("SessionId");
							XmlaClient.CheckAndSkipEmptyElement(reader, "Session", "http://schemas.xmlsoap.org/soap/envelope/");
						}
						else
						{
							reader.Skip();
						}
					}
					else if (reader.IsStartElement("NamespaceCompatibility", "http://schemas.microsoft.com/analysisservices/2003/engine"))
					{
						if (readNamepsaceCompatibility)
						{
							this.namespacesManager.PopulateIgnorableNamespaces(reader);
						}
						else
						{
							reader.Skip();
						}
					}
					else
					{
						string attribute = reader.GetAttribute("mustUnderstand");
						if (attribute != null && !(attribute.Trim() == "0"))
						{
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Unexpected value of MustUnderstand attribute: '{0}'", new object[]
							{
								attribute
							}));
						}
						reader.Skip();
					}
				}
				reader.ReadEndElement();
			}
		}

		private string GetTokenFromEnvelopeHeader(XmlReader reader)
		{
			string result = string.Empty;
			if (reader.IsStartElement("Header", "http://schemas.xmlsoap.org/soap/envelope/"))
			{
				reader.ReadStartElement();
				while (reader.IsStartElement())
				{
					if (reader.IsStartElement("SessionToken", "http://schemas.microsoft.com/analysisservices/2003/xmla"))
					{
						string text = reader.ReadOuterXml();
						int num = text.IndexOf(">", StringComparison.OrdinalIgnoreCase);
						if (num == -1)
						{
							break;
						}
						int num2 = text.IndexOf(string.Format(CultureInfo.InvariantCulture, "</{0}>", new object[]
						{
							"SessionToken"
						}), StringComparison.OrdinalIgnoreCase);
						if (num2 != -1)
						{
							result = text.Substring(num + 1, num2 - num - 1).Trim();
							break;
						}
						break;
					}
				}
			}
			return result;
		}

		internal static void TraceVerbose(string message, params object[] args)
		{
			XmlaClient.SPProxy.TraceVerbose(message, args);
		}

		internal static void TraceWarning(string message, params object[] args)
		{
			XmlaClient.SPProxy.TraceWarning(message, args);
		}

		internal static void TraceError(string message, params object[] args)
		{
			XmlaClient.SPProxy.TraceError(message, args);
		}
	}
}
