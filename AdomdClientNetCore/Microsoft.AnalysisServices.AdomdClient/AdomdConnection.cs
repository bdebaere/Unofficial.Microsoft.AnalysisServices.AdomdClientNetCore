using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdConnection : Component, IDbConnection, IDisposable, ICloneable
	{
		private interface IXmlaClientProviderEx
		{
			string ConnectionString
			{
				get;
				set;
			}

			bool IsExternalSession
			{
				get;
			}

			string SessionID
			{
				get;
				set;
			}

			bool ShowHiddenObjects
			{
				get;
				set;
			}

			int ConnectionTimeout
			{
				get;
			}

			string ServerName
			{
				get;
			}

			string InstanceName
			{
				get;
			}

			ConnectTo ConnectTo
			{
				get;
			}

			ConnectionType ConnectionType
			{
				get;
			}

			uint AutoSyncPeriod
			{
				get;
			}

			SafetyOptions SafetyOptions
			{
				get;
			}

			bool IsXmlaClientConnected
			{
				get;
			}

			bool IsIXMLAMode
			{
				get;
			}

			bool UseEU
			{
				get;
				set;
			}

			string ServerVersion
			{
				get;
			}

			string ProviderVersion
			{
				get;
			}

			RowsetFormatter Discover(string requestType, IDictionary restrictions, InlineErrorHandlingType inlineErrorHandling, bool sendNamespaceCompatibility);

			RowsetFormatter Discover(string requestType, string requestnNamespace, IDictionary restrictions, InlineErrorHandlingType inlineErrorHandling, IDictionary requestProperties);

			RowsetFormatter DiscoverSchema(string requestType, IDictionary restrictions, InlineErrorHandlingType inlineErrorHandling);

			RowsetFormatter DiscoverWithCreateSession(string requestType, bool sendNamespaceCompatibility);

			void CreateSession(bool sendNamespaceCompatibility);

			void EndSession();

			void ConnectXmla();

			void ConnectIXmla();

			void Connect();

			void Disconnect(bool endSession);

			void CancelCommand(string sessionID);

			string GetPropertyFromServer(string propName, bool sendNSCompatibility);

			void SetXmlaProperty(string propertyName, string propertyValue);

			string GetXmlaProperty(string propertyName);

			void ResetSession();

			void ResetInternalState();

			bool IsPostYukonProvider();

			void MarkConnectionStringRestricted();
		}

		private class XmlaClientProvider : IDiscoverProvider, IExecuteProvider, AdomdConnection.IXmlaClientProviderEx
		{
			private class StringCommandContentProvider : ICommandContentProvider
			{
				private string commandText = string.Empty;

				private bool isMdx = true;

				Stream ICommandContentProvider.CommandStream
				{
					get
					{
						return null;
					}
				}

				string ICommandContentProvider.CommandText
				{
					get
					{
						return this.commandText;
					}
				}

				bool ICommandContentProvider.IsContentMdx
				{
					get
					{
						return this.isMdx;
					}
				}

				internal StringCommandContentProvider(string commandText, bool isMdx)
				{
					this.commandText = commandText;
					this.isMdx = isMdx;
				}
			}

			private const string contentProperty = "Content";

			private const string dataContent = "Data";

			private const string schemaContent = "Schema";

			private const string schemaDataContent = "SchemaData";

			private const string metadataContent = "Metadata";

			private const string noneContent = "None";

			private const string axisFormatProperty = "AxisFormat";

			private const string tupleFormat = "TupleFormat";

			private const string formatProperty = "Format";

			private const string tabularFormat = "Tabular";

			private const string multidimensionalFormat = "Multidimensional";

			private const string nativeFormat = "Native";

			private const string beginRange = "BeginRange";

			private const string endRange = "EndRange";

			private const string executionModeProperty = "ExecutionMode";

			private const string prepare = "Prepare";

			private const string providerVersionProp = "ProviderVersion";

			private const string serverVersionProp = "DBMSVersion";

			private const string propertyNameRest = "PropertyName";

			private const string discoverPropertiesType = "DISCOVER_PROPERTIES";

			private const int PFE_MDX_CREATE_LOCAL_CUBE = -1056309049;

			private const string fileNameStr = "FILENAME|";

			private const string ddlStr = "|DDL|";

			private static readonly XmlaPropertyKey ContentPropertyXmlaKey;

			private static readonly Dictionary<string, Dictionary<string, bool>> localTimeConversionMap;

			private ListDictionary discoverProperties;

			private ListDictionary executeProperties;

			private ConnectionInfo connectionInfo;

			private string sessionID;

			private bool showHiddenObjects;

			private string originalConnectionStringShowHiddenCubePropertyValue;

			private XmlaClient client;

			private Version serverVersionObject;

			private string providerVersion;

			private string serverVersion;

			string AdomdConnection.IXmlaClientProviderEx.ConnectionString
			{
				get
				{
					if (this.connectionInfo == null)
					{
						return null;
					}
					return this.connectionInfo.ConnectionString;
				}
				set
				{
					if (this.connectionInfo == null)
					{
						this.connectionInfo = new ConnectionInfo(value);
					}
					else
					{
						this.connectionInfo.ConnectionString = value;
					}
					if (!string.IsNullOrEmpty(this.connectionInfo.SessionID))
					{
						this.SessionID = this.connectionInfo.SessionID;
					}
					this.originalConnectionStringShowHiddenCubePropertyValue = ((AdomdConnection.IXmlaClientProviderEx)this).GetXmlaProperty("ShowHiddenCubes");
					if (this.showHiddenObjects)
					{
						this.UpdateShowHiddenCubesProperty(true);
					}
					this.discoverProperties = null;
					this.executeProperties = null;
				}
			}

			bool AdomdConnection.IXmlaClientProviderEx.IsExternalSession
			{
				get
				{
					return this.sessionID != null;
				}
			}

			private string SessionID
			{
				get
				{
					if (this.client != null)
					{
						return this.client.SessionID;
					}
					return this.sessionID;
				}
				set
				{
					this.sessionID = value;
				}
			}

			string AdomdConnection.IXmlaClientProviderEx.SessionID
			{
				get
				{
					return this.SessionID;
				}
				set
				{
					this.SessionID = value;
				}
			}

			bool AdomdConnection.IXmlaClientProviderEx.ShowHiddenObjects
			{
				get
				{
					return this.showHiddenObjects;
				}
				set
				{
					if (this.showHiddenObjects != value)
					{
						this.showHiddenObjects = value;
						this.UpdateShowHiddenCubesProperty(value);
					}
				}
			}

			int AdomdConnection.IXmlaClientProviderEx.ConnectionTimeout
			{
				get
				{
					if (this.connectionInfo == null)
					{
						return 60;
					}
					return this.connectionInfo.ConnectTimeout;
				}
			}

			string AdomdConnection.IXmlaClientProviderEx.ServerName
			{
				get
				{
					return this.connectionInfo.Server;
				}
			}

			string AdomdConnection.IXmlaClientProviderEx.InstanceName
			{
				get
				{
					return this.connectionInfo.InstanceName;
				}
			}

			ConnectTo AdomdConnection.IXmlaClientProviderEx.ConnectTo
			{
				get
				{
					return this.connectionInfo.ConnectTo;
				}
			}

			ConnectionType AdomdConnection.IXmlaClientProviderEx.ConnectionType
			{
				get
				{
					return this.connectionInfo.ConnectionType;
				}
			}

			uint AdomdConnection.IXmlaClientProviderEx.AutoSyncPeriod
			{
				get
				{
					if (this.connectionInfo == null)
					{
						return 10000u;
					}
					return this.connectionInfo.AutoSyncPeriod;
				}
			}

			SafetyOptions AdomdConnection.IXmlaClientProviderEx.SafetyOptions
			{
				get
				{
					return this.connectionInfo.SafetyOptions;
				}
			}

			private bool IsXmlaClientConnected
			{
				get
				{
					return this.client != null && this.client.IsConnected;
				}
			}

			bool AdomdConnection.IXmlaClientProviderEx.IsXmlaClientConnected
			{
				get
				{
					return this.IsXmlaClientConnected;
				}
			}

			bool AdomdConnection.IXmlaClientProviderEx.IsIXMLAMode
			{
				get
				{
					return this.connectionInfo.IXMLAMode;
				}
			}

			string AdomdConnection.IXmlaClientProviderEx.ServerVersion
			{
				get
				{
					return this.ServerVersion;
				}
			}

			string AdomdConnection.IXmlaClientProviderEx.ProviderVersion
			{
				get
				{
					return this.ProviderVersion;
				}
			}

			bool AdomdConnection.IXmlaClientProviderEx.UseEU
			{
				get
				{
					return this.connectionInfo != null && this.connectionInfo.UseEU;
				}
				set
				{
					if (this.connectionInfo != null)
					{
						this.connectionInfo.UseEU = value;
					}
				}
			}

			private ListDictionary DiscoverProperties
			{
				get
				{
					if (this.discoverProperties == null)
					{
						this.discoverProperties = new ListDictionary();
						foreach (string key in this.connectionInfo.ExtendedProperties.Keys)
						{
							this.discoverProperties[key] = this.connectionInfo.ExtendedProperties[key];
						}
						if (this.discoverProperties.Contains("AxisFormat"))
						{
							this.discoverProperties.Remove("AxisFormat");
						}
						this.discoverProperties["Content"] = "SchemaData";
						this.discoverProperties["Format"] = "Tabular";
					}
					return this.discoverProperties;
				}
			}

			private ListDictionary ExecuteProperties
			{
				get
				{
					if (this.executeProperties == null)
					{
						this.executeProperties = new ListDictionary();
						foreach (string text in this.connectionInfo.ExtendedProperties.Keys)
						{
							if (text != "AxisFormat" && text != "Content" && text != "Format" && text != "ExecutionMode")
							{
								this.executeProperties[text] = this.connectionInfo.ExtendedProperties[text];
							}
						}
					}
					return this.executeProperties;
				}
			}

			private Version ServerVersionObject
			{
				get
				{
					if (null == this.serverVersionObject)
					{
						this.serverVersionObject = AdomdUtils.ConvertVersionStringToVersionObject(this.ServerVersion);
					}
					return this.serverVersionObject;
				}
			}

			private bool IsPostYukonProvider
			{
				get
				{
					return AdomdUtils.IsPostYukonVersion(this.ServerVersionObject);
				}
			}

			private string ServerVersion
			{
				get
				{
					if (this.serverVersion == null)
					{
						try
						{
							this.serverVersion = ((AdomdConnection.IXmlaClientProviderEx)this).GetPropertyFromServer("DBMSVersion", false);
						}
						catch (NotSupportedException)
						{
							this.serverVersion = this.ProviderVersion;
						}
					}
					return this.serverVersion;
				}
			}

			private string ProviderVersion
			{
				get
				{
					if (this.providerVersion == null)
					{
						this.providerVersion = ((AdomdConnection.IXmlaClientProviderEx)this).GetPropertyFromServer("ProviderVersion", false);
					}
					return this.providerVersion;
				}
			}

			private Dictionary<string, bool> GetTimeConversionMap(string discoverType)
			{
				Dictionary<string, bool> result = null;
				if (AdomdConnection.XmlaClientProvider.localTimeConversionMap.TryGetValue(discoverType, out result) && !this.IsPostYukonProvider)
				{
					result = null;
				}
				return result;
			}

			static XmlaClientProvider()
			{
				AdomdConnection.XmlaClientProvider.ContentPropertyXmlaKey = new XmlaPropertyKey("Content", null);
				AdomdConnection.XmlaClientProvider.localTimeConversionMap = new Dictionary<string, Dictionary<string, bool>>(4);
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				dictionary["DATE_MODIFIED"] = true;
				AdomdConnection.XmlaClientProvider.localTimeConversionMap["DBSCHEMA_CATALOGS"] = dictionary;
				dictionary = new Dictionary<string, bool>();
				dictionary["DATE_MODIFIED"] = true;
				AdomdConnection.XmlaClientProvider.localTimeConversionMap["DBSCHEMA_TABLES"] = dictionary;
				dictionary = new Dictionary<string, bool>();
				dictionary["LAST_SCHEMA_UPDATE"] = true;
				dictionary["LAST_DATA_UPDATE"] = true;
				AdomdConnection.XmlaClientProvider.localTimeConversionMap["MDSCHEMA_CUBES"] = dictionary;
				dictionary = new Dictionary<string, bool>();
				dictionary["CREATED_ON"] = true;
				dictionary["LAST_SCHEMA_UPDATE"] = true;
				AdomdConnection.XmlaClientProvider.localTimeConversionMap["MDSCHEMA_INPUT_DATASOURCES"] = dictionary;
				dictionary = new Dictionary<string, bool>();
				dictionary["DATE_CREATED"] = true;
				dictionary["DATE_MODIFIED"] = true;
				dictionary["LAST_PROCESSED"] = true;
				AdomdConnection.XmlaClientProvider.localTimeConversionMap["DMSCHEMA_MINING_MODELS"] = dictionary;
				dictionary = new Dictionary<string, bool>();
				dictionary["DATE_CREATED"] = true;
				dictionary["DATE_MODIFIED"] = true;
				dictionary["LAST_PROCESSED"] = true;
				AdomdConnection.XmlaClientProvider.localTimeConversionMap["DMSCHEMA_MINING_STRUCTURES"] = dictionary;
			}

			internal XmlaClientProvider()
			{
			}

			internal XmlaClientProvider(AdomdConnection.XmlaClientProvider provider) : this(new ConnectionInfo(provider.connectionInfo))
			{
			}

			internal XmlaClientProvider(ConnectionInfo info)
			{
				this.connectionInfo = info;
			}

			RowsetFormatter AdomdConnection.IXmlaClientProviderEx.Discover(string requestType, IDictionary restrictions, InlineErrorHandlingType inlineErrorHandling, bool sendNamespaceCompatibility)
			{
				RowsetFormatter result;
				try
				{
					Dictionary<string, bool> timeConversionMap = this.GetTimeConversionMap(requestType);
					XmlaReader reader = (XmlaReader)this.client.Discover(requestType, this.DiscoverProperties, restrictions, sendNamespaceCompatibility);
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					ResultsetFormatter resultsetFormatter = soapFormatter.ReadDiscoverResponse(reader, inlineErrorHandling, null, timeConversionMap);
					if (!(resultsetFormatter is RowsetFormatter))
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, "");
					}
					result = (RowsetFormatter)resultsetFormatter;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				return result;
			}

			RowsetFormatter AdomdConnection.IXmlaClientProviderEx.Discover(string requestType, string requestNamespace, IDictionary restrictions, InlineErrorHandlingType inlineErrorHandling, IDictionary requestProperties)
			{
				RowsetFormatter result;
				try
				{
					Dictionary<string, bool> timeConversionMap = this.GetTimeConversionMap(requestType);
					XmlaReader reader = (XmlaReader)this.client.Discover(requestType, requestNamespace, this.DiscoverProperties, restrictions, requestProperties);
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					ResultsetFormatter resultsetFormatter = soapFormatter.ReadDiscoverResponse(reader, inlineErrorHandling, null, timeConversionMap);
					if (!(resultsetFormatter is RowsetFormatter))
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, "");
					}
					result = (RowsetFormatter)resultsetFormatter;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				return result;
			}

			RowsetFormatter AdomdConnection.IXmlaClientProviderEx.DiscoverSchema(string requestType, IDictionary restrictions, InlineErrorHandlingType inlineErrorHandling)
			{
				this.DiscoverProperties["Content"] = "Schema";
				RowsetFormatter result;
				try
				{
					Dictionary<string, bool> timeConversionMap = this.GetTimeConversionMap(requestType);
					XmlaReader reader = (XmlaReader)this.client.Discover(requestType, this.DiscoverProperties, restrictions);
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					ResultsetFormatter resultsetFormatter = soapFormatter.ReadDiscoverResponse(reader, inlineErrorHandling, null, timeConversionMap);
					if (!(resultsetFormatter is RowsetFormatter))
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, "");
					}
					result = (RowsetFormatter)resultsetFormatter;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				finally
				{
					this.DiscoverProperties["Content"] = "SchemaData";
				}
				return result;
			}

			RowsetFormatter AdomdConnection.IXmlaClientProviderEx.DiscoverWithCreateSession(string requestType, bool sendNamespaceCompatibility)
			{
				RowsetFormatter result;
				try
				{
					Dictionary<string, bool> timeConversionMap = this.GetTimeConversionMap(requestType);
					XmlaReader reader = (XmlaReader)this.client.DiscoverWithCreateSession(requestType, this.DiscoverProperties, sendNamespaceCompatibility);
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					ResultsetFormatter resultsetFormatter = soapFormatter.ReadDiscoverResponse(reader, InlineErrorHandlingType.StoreInCell, null, timeConversionMap);
					if (!(resultsetFormatter is RowsetFormatter))
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, "");
					}
					result = (RowsetFormatter)resultsetFormatter;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				return result;
			}

			void AdomdConnection.IXmlaClientProviderEx.CreateSession(bool sendNamespaceCompatibility)
			{
				try
				{
					this.client.CreateSession(this.connectionInfo.ExtendedProperties, sendNamespaceCompatibility);
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			void AdomdConnection.IXmlaClientProviderEx.EndSession()
			{
				try
				{
					this.client.EndSession(this.connectionInfo.ExtendedProperties);
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			void AdomdConnection.IXmlaClientProviderEx.ConnectXmla()
			{
				this.Connect(false);
			}

			void AdomdConnection.IXmlaClientProviderEx.ConnectIXmla()
			{
				this.Connect(true);
			}

			private void Connect(bool toIXMLA)
			{
				try
				{
					this.connectionInfo.IXMLAMode = toIXMLA;
					this.discoverProperties = null;
					this.executeProperties = null;
					if (this.client == null)
					{
						this.client = new AdomdClient();
					}
					this.client.Connect(this.connectionInfo, false);
					this.client.SessionID = this.sessionID;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			void AdomdConnection.IXmlaClientProviderEx.Connect()
			{
				try
				{
					if (this.connectionInfo.IXMLAMode)
					{
						((AdomdConnection.IXmlaClientProviderEx)this).ConnectIXmla();
					}
					else
					{
						((AdomdConnection.IXmlaClientProviderEx)this).ConnectXmla();
					}
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			void AdomdConnection.IXmlaClientProviderEx.Disconnect(bool endSession)
			{
				try
				{
					if (this.client != null)
					{
						this.client.Disconnect(endSession);
					}
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			void AdomdConnection.IXmlaClientProviderEx.CancelCommand(string sessionID)
			{
				try
				{
					this.client.CancelCommand(sessionID);
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			string AdomdConnection.IXmlaClientProviderEx.GetPropertyFromServer(string propName, bool sendNSCompatibility)
			{
				RowsetFormatter rowsetFormatter = ((AdomdConnection.IXmlaClientProviderEx)this).Discover("DISCOVER_PROPERTIES", new ListDictionary
				{
					{
						"PropertyName",
						propName
					}
				}, InlineErrorHandlingType.StoreInCell, sendNSCompatibility);
				DataRowCollection rows = rowsetFormatter.MainRowsetTable.Rows;
				if (rows.Count != 1)
				{
					throw new NotSupportedException(SR.Connection_InvalidProperty(propName));
				}
				DataRow dataRow = rows[0];
				return dataRow["Value"].ToString();
			}

			void AdomdConnection.IXmlaClientProviderEx.SetXmlaProperty(string propertyName, string propertyValue)
			{
				if (propertyValue == null)
				{
					if (!this.connectionInfo.ExtendedProperties.Contains(propertyName))
					{
						return;
					}
					this.connectionInfo.ExtendedProperties.Remove(propertyName);
				}
				else
				{
					this.connectionInfo.ExtendedProperties[propertyName] = propertyValue;
				}
				this.discoverProperties = null;
				this.executeProperties = null;
			}

			string AdomdConnection.IXmlaClientProviderEx.GetXmlaProperty(string propertyName)
			{
				return this.connectionInfo.ExtendedProperties[propertyName] as string;
			}

			void AdomdConnection.IXmlaClientProviderEx.ResetSession()
			{
				this.client.SessionID = this.sessionID;
			}

			void AdomdConnection.IXmlaClientProviderEx.MarkConnectionStringRestricted()
			{
				if (this.connectionInfo != null)
				{
					this.connectionInfo.RestrictConnectionString = true;
				}
			}

			void AdomdConnection.IXmlaClientProviderEx.ResetInternalState()
			{
				this.serverVersionObject = null;
				this.serverVersion = null;
				this.providerVersion = null;
			}

			bool AdomdConnection.IXmlaClientProviderEx.IsPostYukonProvider()
			{
				return this.IsPostYukonProvider;
			}

			void IDiscoverProvider.Discover(string requestType, IDictionary restrictions, DataTable table)
			{
				try
				{
					Dictionary<string, bool> timeConversionMap = this.GetTimeConversionMap(requestType);
					XmlaReader reader = (XmlaReader)this.client.Discover(requestType, this.DiscoverProperties, restrictions);
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					soapFormatter.ReadDiscoverResponse(reader, InlineErrorHandlingType.StoreInCell, table, false, timeConversionMap);
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			void IDiscoverProvider.DiscoverData(string requestType, IDictionary restrictions, DataTable table)
			{
				this.DiscoverProperties["Content"] = "Data";
				try
				{
					Dictionary<string, bool> timeConversionMap = this.GetTimeConversionMap(requestType);
					XmlaReader reader = (XmlaReader)this.client.Discover(requestType, this.DiscoverProperties, restrictions);
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					soapFormatter.ReadDiscoverResponse(reader, InlineErrorHandlingType.StoreInCell, table, false, timeConversionMap);
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				finally
				{
					this.DiscoverProperties["Content"] = "SchemaData";
				}
			}

			RowsetFormatter IDiscoverProvider.Discover(string requestType, IDictionary restrictions)
			{
				RowsetFormatter result;
				try
				{
					Dictionary<string, bool> timeConversionMap = this.GetTimeConversionMap(requestType);
					XmlaReader reader = (XmlaReader)this.client.Discover(requestType, this.DiscoverProperties, restrictions);
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					ResultsetFormatter resultsetFormatter = soapFormatter.ReadDiscoverResponse(reader, InlineErrorHandlingType.StoreInCell, null, false, timeConversionMap);
					if (!(resultsetFormatter is RowsetFormatter))
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, "");
					}
					result = (RowsetFormatter)resultsetFormatter;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				return result;
			}

			MDDatasetFormatter IExecuteProvider.ExecuteMultidimensional(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters)
			{
				MDDatasetFormatter result;
				try
				{
					IDictionary executionCommandProperties = this.GetExecutionCommandProperties(commandProperties, new AdomdPropertyCollection
					{
						{
							"Format",
							"Multidimensional"
						},
						{
							"AxisFormat",
							"TupleFormat"
						},
						{
							"Content",
							this.GetContentAtLeastMetadata(commandProperties)
						}
					});
					XmlaReader reader;
					if (contentProvider.CommandText != null)
					{
						reader = (XmlaReader)this.client.ExecuteStatement(contentProvider.CommandText, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
					else
					{
						reader = (XmlaReader)this.client.ExecuteStream(contentProvider.CommandStream, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
					SoapFormatter soapFormatter = new SoapFormatter(this.client);
					ResultsetFormatter resultsetFormatter = soapFormatter.ReadResponse(reader);
					if (!(resultsetFormatter is MDDatasetFormatter))
					{
						throw new AdomdUnknownResponseException(SR.Resultset_IsNotDataset, "");
					}
					result = (MDDatasetFormatter)resultsetFormatter;
				}
				catch (AdomdErrorResponseException ex)
				{
					if (ex.ErrorCode == -1056309049)
					{
						throw new AdomdUnknownResponseException(SR.Resultset_IsNotDataset, ex);
					}
					throw;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					AdomdErrorResponseException ex2 = new AdomdErrorResponseException(innerException);
					if (ex2.ErrorCode == -1056309049)
					{
						throw new AdomdUnknownResponseException(SR.Resultset_IsNotDataset, ex2);
					}
					throw ex2;
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				return result;
			}

			XmlaReader IExecuteProvider.ExecuteTabular(CommandBehavior behavior, ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters)
			{
				XmlaReader result;
				try
				{
					AdomdPropertyCollection adomdPropertyCollection = new AdomdPropertyCollection();
					adomdPropertyCollection.Add("Format", "Tabular");
					if ((behavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default)
					{
						adomdPropertyCollection.Add("Content", "Schema");
					}
					else
					{
						adomdPropertyCollection.Add("Content", this.GetContentAtLeastSchema(commandProperties));
						if ((behavior & CommandBehavior.SingleRow) != CommandBehavior.Default)
						{
							adomdPropertyCollection.Add("BeginRange", -1);
							adomdPropertyCollection.Add("EndRange", 0);
						}
					}
					IDictionary executionCommandProperties = this.GetExecutionCommandProperties(commandProperties, adomdPropertyCollection);
					if (contentProvider.CommandText != null)
					{
						result = (XmlaReader)this.client.ExecuteStatement(contentProvider.CommandText, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
					else
					{
						result = (XmlaReader)this.client.ExecuteStream(contentProvider.CommandStream, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
				}
				catch (AdomdErrorResponseException ex)
				{
					if (ex.ErrorCode == -1056309049)
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, ex);
					}
					throw;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					AdomdErrorResponseException ex2 = new AdomdErrorResponseException(innerException);
					if (ex2.ErrorCode == -1056309049)
					{
						throw new AdomdUnknownResponseException(XmlaSR.Resultset_IsNotRowset, ex2);
					}
					throw ex2;
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				return result;
			}

			void IExecuteProvider.ExecuteAny(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters)
			{
				try
				{
					IDictionary executionCommandProperties = this.GetExecutionCommandProperties(commandProperties, new AdomdPropertyCollection
					{
						{
							"Format",
							"Native"
						}
					});
					XmlaReader xmlaReader = null;
					if (contentProvider.CommandText != null)
					{
						xmlaReader = (XmlaReader)this.client.ExecuteStatement(contentProvider.CommandText, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
					else
					{
						xmlaReader = (XmlaReader)this.client.ExecuteStream(contentProvider.CommandStream, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
					try
					{
						if (!XmlaClient.IsExecuteResponseS(xmlaReader))
						{
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected element {0}:{1}, got {2}", new object[]
							{
								"urn:schemas-microsoft-com:xml-analysis",
								"ExecuteResponse",
								xmlaReader.Name
							}));
						}
						XmlaClient.ReadExecuteResponse(xmlaReader);
					}
					catch (XmlException innerException)
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
					}
					catch (COMException innerException2)
					{
						if (this.client != null)
						{
							this.client.Disconnect(false);
						}
						throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
					}
					catch (IOException innerException3)
					{
						if (this.client != null)
						{
							this.client.Disconnect(false);
						}
						throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException3);
					}
					finally
					{
						xmlaReader.Close();
					}
				}
				catch (AdomdErrorResponseException ex)
				{
					if (ex.ErrorCode != -1056309049)
					{
						throw;
					}
					this.HandleCreateLocalCube(ex);
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException4)
				{
					AdomdErrorResponseException ex2 = new AdomdErrorResponseException(innerException4);
					if (ex2.ErrorCode != -1056309049)
					{
						throw ex2;
					}
					this.HandleCreateLocalCube(ex2);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			XmlaReader IExecuteProvider.Execute(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters)
			{
				XmlaReader result;
				try
				{
					IDictionary executionCommandProperties = this.GetExecutionCommandProperties(commandProperties, new AdomdPropertyCollection
					{
						{
							"Format",
							"Native"
						},
						{
							"AxisFormat",
							"TupleFormat"
						},
						{
							"Content",
							this.GetContentAtLeastMetadata(commandProperties)
						}
					});
					if (contentProvider.CommandText != null)
					{
						result = (XmlaReader)this.client.ExecuteStatement(contentProvider.CommandText, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
					else
					{
						result = (XmlaReader)this.client.ExecuteStream(contentProvider.CommandStream, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
					}
				}
				catch (AdomdErrorResponseException ex)
				{
					if (ex.ErrorCode != -1056309049)
					{
						throw;
					}
					this.HandleCreateLocalCube(ex);
					result = null;
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					AdomdErrorResponseException ex2 = new AdomdErrorResponseException(innerException);
					if (ex2.ErrorCode != -1056309049)
					{
						throw ex2;
					}
					this.HandleCreateLocalCube(ex2);
					result = null;
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
				return result;
			}

			void IExecuteProvider.Prepare(ICommandContentProvider contentProvider, AdomdPropertyCollection commandProperties, IDataParameterCollection parameters)
			{
				try
				{
					IDictionary executionCommandProperties = this.GetExecutionCommandProperties(commandProperties, new AdomdPropertyCollection
					{
						{
							"Format",
							"Native"
						},
						{
							"Content",
							"Metadata"
						},
						{
							"ExecutionMode",
							"Prepare"
						}
					});
					if (contentProvider.CommandText != null)
					{
						XmlaReader xmlaReader = (XmlaReader)this.client.ExecuteStatement(contentProvider.CommandText, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
						xmlaReader.Close();
					}
					else
					{
						XmlaReader xmlaReader2 = (XmlaReader)this.client.ExecuteStream(contentProvider.CommandStream, this.ExecuteProperties, executionCommandProperties, parameters, contentProvider.IsContentMdx);
						xmlaReader2.Close();
					}
				}
				catch (AdomdException)
				{
					throw;
				}
				catch (XmlaException innerException)
				{
					throw new AdomdErrorResponseException(innerException);
				}
				catch
				{
					if (this.client != null)
					{
						this.client.Disconnect(false);
					}
					throw;
				}
			}

			private void UpdateShowHiddenCubesProperty(bool newValue)
			{
				if (this.connectionInfo != null)
				{
					if (newValue)
					{
						((AdomdConnection.IXmlaClientProviderEx)this).SetXmlaProperty("ShowHiddenCubes", "true");
						return;
					}
					((AdomdConnection.IXmlaClientProviderEx)this).SetXmlaProperty("ShowHiddenCubes", this.originalConnectionStringShowHiddenCubePropertyValue);
				}
			}

			private IDictionary GetExecutionCommandProperties(AdomdPropertyCollection commandProperties, AdomdPropertyCollection overrides)
			{
				if (commandProperties == null && overrides == null)
				{
					return null;
				}
				if (commandProperties == null)
				{
					return overrides.InternalCollection;
				}
				if (overrides == null)
				{
					return commandProperties.InternalCollection;
				}
				AdomdPropertyCollection.Enumerator enumerator = commandProperties.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AdomdProperty current = enumerator.Current;
					if (!overrides.InternalCollection.Contains(current))
					{
						overrides.Add(new AdomdProperty(current.Name, current.Namespace, current.Value));
					}
				}
				return overrides.InternalCollection;
			}

			private string GetContentAtLeastMetadata(AdomdPropertyCollection commandProperties)
			{
				string text = "SchemaData";
				if (commandProperties != null && commandProperties.InternalCollection.Contains(AdomdConnection.XmlaClientProvider.ContentPropertyXmlaKey))
				{
					IXmlaProperty xmlaProperty = commandProperties.InternalCollection[AdomdConnection.XmlaClientProvider.ContentPropertyXmlaKey] as IXmlaProperty;
					if (xmlaProperty != null && xmlaProperty.Value != null)
					{
						text = (xmlaProperty.Value as string);
					}
				}
				else if (this.connectionInfo.ExtendedProperties.Contains("Content"))
				{
					text = (this.connectionInfo.ExtendedProperties["Content"] as string);
				}
				if (text == "Data")
				{
					text = "SchemaData";
				}
				else if (text != "SchemaData")
				{
					text = "Metadata";
				}
				return text;
			}

			private string GetContentAtLeastSchema(AdomdPropertyCollection commandProperties)
			{
				string text = "SchemaData";
				if (commandProperties != null && commandProperties.InternalCollection.Contains(AdomdConnection.XmlaClientProvider.ContentPropertyXmlaKey))
				{
					IXmlaProperty xmlaProperty = commandProperties.InternalCollection[AdomdConnection.XmlaClientProvider.ContentPropertyXmlaKey] as IXmlaProperty;
					if (xmlaProperty != null && xmlaProperty.Value != null)
					{
						text = (xmlaProperty.Value as string);
					}
				}
				else if (this.connectionInfo.ExtendedProperties.Contains("Content"))
				{
					text = (this.connectionInfo.ExtendedProperties["Content"] as string);
				}
				if (text == "Data")
				{
					text = "SchemaData";
				}
				else if (text != "Metadata" && text != "SchemaData")
				{
					text = "Schema";
				}
				return text;
			}

			private void HandleCreateLocalCube(AdomdErrorResponseException ex)
			{
				string dataSource = null;
				string commandText = null;
				if (!AdomdConnection.XmlaClientProvider.ParseCreateGlobalCubeErrorMessage(ex.Message, out dataSource, out commandText))
				{
					throw new AdomdUnknownResponseException(ex);
				}
				AdomdConnection.IXmlaClientProviderEx xmlaClientProviderEx = new AdomdConnection.XmlaClientProvider(ConnectionInfo.GetModifiedConnectionInfo(this.connectionInfo, dataSource));
				xmlaClientProviderEx.Connect();
				try
				{
					IExecuteProvider executeProvider = xmlaClientProviderEx as IExecuteProvider;
					XmlaReader xmlaReader = executeProvider.Execute(new AdomdConnection.XmlaClientProvider.StringCommandContentProvider(commandText, false), null, null);
					try
					{
						if (!XmlaClient.IsExecuteResponseS(xmlaReader))
						{
							throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected element {0}:{1}, got {2}", new object[]
							{
								"urn:schemas-microsoft-com:xml-analysis",
								"ExecuteResponse",
								xmlaReader.Name
							}));
						}
						XmlaClient.ReadExecuteResponse(xmlaReader);
					}
					finally
					{
						xmlaReader.Close();
					}
				}
				catch (XmlException innerException)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
				}
				catch (IOException innerException2)
				{
					throw new AdomdConnectionException(XmlaSR.ConnectionBroken, innerException2);
				}
				catch (XmlaException innerException3)
				{
					throw new AdomdErrorResponseException(innerException3);
				}
				finally
				{
					if (xmlaClientProviderEx != null && xmlaClientProviderEx.IsXmlaClientConnected)
					{
						xmlaClientProviderEx.Disconnect(false);
					}
				}
			}

			private static bool ParseCreateGlobalCubeErrorMessage(string message, out string filename, out string ddl)
			{
				filename = null;
				ddl = null;
				if (string.IsNullOrEmpty(message))
				{
					return false;
				}
				int num = message.IndexOf("FILENAME|", 0, StringComparison.Ordinal);
				if (num == -1)
				{
					return false;
				}
				num += "FILENAME|".Length;
				int num2 = message.IndexOf("|DDL|", num, StringComparison.Ordinal);
				if (num2 == -1)
				{
					return false;
				}
				filename = message.Substring(num, num2 - num);
				num2 += "|DDL|".Length;
				ddl = message.Substring(num2);
				return true;
			}
		}

		internal class XmlaMDSchemas
		{
			private const string MembersSchemaName = "MDSCHEMA_MEMBERS";

			private Hashtable schemasByName;

			private Hashtable schemasByGuid;

			internal XmlaMDSchemas(RowsetFormatter formatter)
			{
				this.PopulateSchemasInfos(formatter);
			}

			internal XmlaMDSchemas()
			{
				this.schemasByGuid = AdomdConnection.ShilohSchemas.SchemasByGuid;
				this.schemasByName = AdomdConnection.ShilohSchemas.SchemasByName;
			}

			internal AdomdConnection.XmlaMDSchema GetSchemaInfo(Guid schemaGuid)
			{
				if (!this.schemasByGuid.ContainsKey(schemaGuid))
				{
					throw new ArgumentException(SR.Schema_InvalidGuid, "schemaGuid");
				}
				return (AdomdConnection.XmlaMDSchema)this.schemasByGuid[schemaGuid];
			}

			private void PopulateSchemasInfos(RowsetFormatter formatter)
			{
				this.schemasByGuid = new Hashtable(formatter.MainRowsetTable.Rows.Count);
				this.schemasByName = new Hashtable(formatter.MainRowsetTable.Rows.Count);
				foreach (DataRow dataRow in formatter.MainRowsetTable.Rows)
				{
					string text = AdomdUtils.GetProperty(dataRow, "SchemaName") as string;
					if (text == null)
					{
						throw new AdomdUnknownResponseException(SR.Schema_PropertyIsMissingOrOfAnUnexpectedType("DISCOVER_SCHEMA_ROWSETS", "SchemaName"), "");
					}
					Guid guid = Guid.Empty;
					object obj = null;
					if (formatter.MainRowsetTable.Columns.Contains("SchemaGuid"))
					{
						obj = AdomdUtils.GetProperty(dataRow, "SchemaGuid");
						if (obj is DBNull)
						{
							obj = null;
						}
					}
					if (obj != null && obj is Guid)
					{
						guid = (Guid)obj;
					}
					DataRow[] childRows = dataRow.GetChildRows(formatter.MainRowsetTable.TableName + "Restrictions");
					AdomdConnection.XmlaMDSchemaRestriction[] array = new AdomdConnection.XmlaMDSchemaRestriction[childRows.Length];
					int num = 0;
					DataRow[] array2 = childRows;
					for (int i = 0; i < array2.Length; i++)
					{
						DataRow dataRow2 = array2[i];
						string text2 = AdomdUtils.GetProperty(dataRow2, "Name") as string;
						if (text2 == null)
						{
							throw new AdomdUnknownResponseException(SR.Schema_PropertyIsMissingOrOfAnUnexpectedType("DISCOVER_SCHEMA_ROWSETS", "Name"), "");
						}
						string text3 = AdomdUtils.GetProperty(dataRow2, "Type") as string;
						if (text3 == null)
						{
							throw new AdomdUnknownResponseException(SR.Schema_PropertyIsMissingOrOfAnUnexpectedType("DISCOVER_SCHEMA_ROWSETS", "Type"), "");
						}
						array[num] = new AdomdConnection.XmlaMDSchemaRestriction(text2, NetTypeMapper.GetNetTypeWithPrefix(text3));
						num++;
					}
					AdomdConnection.XmlaMDSchema xmlaMDSchema = new AdomdConnection.XmlaMDSchema(text, guid, array);
					this.schemasByName[xmlaMDSchema.SchemaName] = xmlaMDSchema;
					if (guid != Guid.Empty)
					{
						this.schemasByGuid[xmlaMDSchema.SchemaGuid] = xmlaMDSchema;
					}
				}
			}

			internal static void ConvertOleDbRestrictionsToXmlA(AdomdConnection.XmlaMDSchema schemaInfo, object[] restrictions, ListDictionary xmlaRestrictions)
			{
				if (restrictions.Length > schemaInfo.Restrictions.Length)
				{
					throw new ArgumentException(SR.Schema_RestOutOfRange, "restrictions");
				}
				for (int i = 0; i < restrictions.Length; i++)
				{
					object obj = restrictions[i];
					if (obj != null)
					{
						try
						{
							obj = Convert.ChangeType(obj, schemaInfo.Restrictions[i].Type, CultureInfo.InvariantCulture);
						}
						catch (InvalidCastException innerException)
						{
							throw new ArgumentException(SR.Restrictions_TypesMismatch(schemaInfo.Restrictions[i].Name, schemaInfo.Restrictions[i].Type.FullName, restrictions[i].GetType().FullName), innerException);
						}
					}
					if (obj != null)
					{
						xmlaRestrictions.Add(schemaInfo.Restrictions[i].Name, obj);
					}
				}
			}

			internal static void MungeMembersSchemaColumnNames(string schemaName, string schemaNamespace, DataSet dataSet)
			{
				if (dataSet != null && dataSet.Tables.Count == 1 && schemaName == "MDSCHEMA_MEMBERS" && (schemaNamespace == null || schemaNamespace.Length == 0))
				{
					DataTable dataTable = dataSet.Tables[0];
					foreach (DataColumn dataColumn in dataTable.Columns)
					{
						try
						{
							dataColumn.ColumnName = dataColumn.Caption;
						}
						catch (DuplicateNameException)
						{
						}
					}
				}
			}
		}

		private static class ShilohSchemas
		{
			private static AdomdConnection.XmlaMDSchema[] shilohSchemas;

			private static Hashtable schemasByGuid;

			private static Hashtable schemasByName;

			internal static Hashtable SchemasByGuid
			{
				get
				{
					return AdomdConnection.ShilohSchemas.schemasByGuid;
				}
			}

			internal static Hashtable SchemasByName
			{
				get
				{
					return AdomdConnection.ShilohSchemas.schemasByName;
				}
			}

			static ShilohSchemas()
			{
				AdomdConnection.ShilohSchemas.shilohSchemas = new AdomdConnection.XmlaMDSchema[]
				{
					new AdomdConnection.XmlaMDSchema("DBSCHEMA_CATALOGS", new Guid("{C8B52211-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DBSCHEMA_TABLES", new Guid("{C8B52229-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_TYPE", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DBSCHEMA_TABLES_INFO", new Guid("{C8B522E0-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_TYPE", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DBSCHEMA_COLUMNS", new Guid("{C8B52214-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TABLE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("COLUMN_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DBSCHEMA_PROVIDER_TYPES", new Guid("{C8B5222C-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("DATA_TYPE", typeof(ushort)),
						new AdomdConnection.XmlaMDSchemaRestriction("BEST_MATCH", typeof(bool))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_CUBES", new Guid("{C8B522D8-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_DIMENSIONS", new Guid("{C8B522D9-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("DIMENSION_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("DIMENSION_UNIQUE_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_HIERARCHIES", new Guid("{C8B522DA-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("DIMENSION_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("HIERARCHY_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("HIERARCHY_UNIQUE_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_LEVELS", new Guid("{C8B522DB-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("DIMENSION_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("HIERARCHY_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("LEVEL_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("LEVEL_UNIQUE_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_MEASURES", new Guid("{C8B522DC-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MEASURE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MEASURE_UNIQUE_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_PROPERTIES", new Guid("{C8B522DD-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("DIMENSION_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("HIERARCHY_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("LEVEL_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MEMBER_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("PROPERTY_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("PROPERTY_TYPE", typeof(short))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_MEMBERS", new Guid("{C8B522DE-5CF3-11CE-ADE5-00AA0044773D}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("DIMENSION_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("HIERARCHY_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("LEVEL_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("LEVEL_NUMBER", typeof(uint)),
						new AdomdConnection.XmlaMDSchemaRestriction("MEMBER_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MEMBER_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MEMBER_CAPTION", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MEMBER_TYPE", typeof(int)),
						new AdomdConnection.XmlaMDSchemaRestriction("TREE_OP", typeof(int))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_FUNCTIONS", new Guid("{A07CCD07-8148-11D0-87BB-00C04FC33942}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("LIBRARY_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("INTERFACE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("FUNCTION_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("ORIGIN", typeof(int))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_ACTIONS", new Guid("{A07CCD08-8148-11D0-87BB-00C04FC33942}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("ACTION_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("ACTION_TYPE", typeof(int)),
						new AdomdConnection.XmlaMDSchemaRestriction("COORDINATE", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("COORDINATE_TYPE", typeof(int)),
						new AdomdConnection.XmlaMDSchemaRestriction("INVOCATION", typeof(int))
					}),
					new AdomdConnection.XmlaMDSchema("MDSCHEMA_SETS", new Guid("{A07CCD0B-8148-11D0-87BB-00C04FC33942}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("CATALOG_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCHEMA_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("CUBE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SET_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SCOPE", typeof(int))
					}),
					new AdomdConnection.XmlaMDSchema("DMSCHEMA_MINING_SERVICES", new Guid("{3ADD8A95-D8B9-11D2-8D2A-00E029154FDE}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("SERVICE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SERVICE_TYPE_ID", typeof(uint))
					}),
					new AdomdConnection.XmlaMDSchema("DMSCHEMA_MINING_SERVICE_PARAMETERS", new Guid("{3ADD8A75-D8B9-11D2-8D2A-00E029154FDE}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("SERVICE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("PARAMETER_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DMSCHEMA_MINING_MODEL_CONTENT", new Guid("{3ADD8A76-D8B9-11D2-8D2A-00E029154FDE}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("ATTRIBUTE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("NODE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("NODE_UNIQUE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("NODE_TYPE", typeof(int)),
						new AdomdConnection.XmlaMDSchemaRestriction("NODE_GUID", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("NODE_CAPTION", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("TREE_OPERATION", typeof(uint))
					}),
					new AdomdConnection.XmlaMDSchema("DMSCHEMA_MINING_MODELS", new Guid("{3ADD8A77-D8B9-11D2-8D2A-00E029154FDE}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_TYPE", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SERVICE_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("SERVICE_TYPE_ID", typeof(uint))
					}),
					new AdomdConnection.XmlaMDSchema("DMSCHEMA_MINING_COLUMNS", new Guid("{3ADD8A78-D8B9-11D2-8D2A-00E029154FDE}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("COLUMN_NAME", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DMSCHEMA_MINING_MODEL_XML", new Guid("{4290B2D5-0E9C-4AA7-9369-98C95CFD9D13}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_TYPE", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DMSCHEMA_MINING_MODEL_PMML", new Guid("{4290B2D5-0E9C-4AA7-9369-98C95CFD9D13}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_CATALOG", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_SCHEMA", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_NAME", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("MODEL_TYPE", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DISCOVER_DATASOURCES", new Guid("{06C03D41-F66D-49F3-B1B8-987F7AF4CF18}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("DataSourceName", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("URL", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("ProviderName", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("ProviderType", typeof(string)),
						new AdomdConnection.XmlaMDSchemaRestriction("AuthenticationMode", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DISCOVER_PROPERTIES", new Guid("{4B40ADFB-8B09-4758-97BB-636E8AE97BCF}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("PropertyName", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DISCOVER_SCHEMA_ROWSETS", new Guid("{EEA0302B-7922-4992-8991-0E605D0E5593}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("SchemaName", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DISCOVER_ENUMERATORS", new Guid("{55A9E78B-ACCB-45B4-95A6-94C5065617A7}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("EnumName", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DISCOVER_KEYWORDS", new Guid("{1426C443-4CDD-4A40-8F45-572FAB9BBAA1}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("Keyword", typeof(string))
					}),
					new AdomdConnection.XmlaMDSchema("DISCOVER_LITERALS", new Guid("{C3EF5ECB-0A07-4665-A140-B075722DBDC2}"), new AdomdConnection.XmlaMDSchemaRestriction[]
					{
						new AdomdConnection.XmlaMDSchemaRestriction("LiteralName", typeof(string))
					})
				};
				AdomdConnection.ShilohSchemas.schemasByGuid = new Hashtable(AdomdConnection.ShilohSchemas.shilohSchemas.Length);
				AdomdConnection.ShilohSchemas.schemasByName = new Hashtable(AdomdConnection.ShilohSchemas.shilohSchemas.Length);
				AdomdConnection.XmlaMDSchema[] array = AdomdConnection.ShilohSchemas.shilohSchemas;
				for (int i = 0; i < array.Length; i++)
				{
					AdomdConnection.XmlaMDSchema xmlaMDSchema = array[i];
					AdomdConnection.ShilohSchemas.schemasByGuid[xmlaMDSchema.SchemaGuid] = xmlaMDSchema;
					AdomdConnection.ShilohSchemas.schemasByName[xmlaMDSchema.SchemaName] = xmlaMDSchema;
				}
			}
		}

		internal struct XmlaMDSchema
		{
			private string schemaName;

			private Guid schemaGuid;

			private AdomdConnection.XmlaMDSchemaRestriction[] restictions;

			internal string SchemaName
			{
				get
				{
					return this.schemaName;
				}
			}

			internal Guid SchemaGuid
			{
				get
				{
					return this.schemaGuid;
				}
			}

			internal AdomdConnection.XmlaMDSchemaRestriction[] Restrictions
			{
				get
				{
					return this.restictions;
				}
			}

			internal XmlaMDSchema(string schemaName, Guid schemaGuid, AdomdConnection.XmlaMDSchemaRestriction[] restrictions)
			{
				this.schemaName = schemaName;
				this.schemaGuid = schemaGuid;
				this.restictions = restrictions;
			}
		}

		internal struct XmlaMDSchemaRestriction
		{
			private string name;

			private Type type;

			internal string Name
			{
				get
				{
					return this.name;
				}
			}

			internal Type Type
			{
				get
				{
					return this.type;
				}
			}

			internal XmlaMDSchemaRestriction(string name, Type type)
			{
				this.name = name;
				this.type = type;
			}
		}

		private const string propertyValueColumn = "Value";

		private const string catalogPropertyName = "Catalog";

		private const string providerType = "ProviderType";

		private const string discoverDatasources = "DISCOVER_DATASOURCES";

		private const string dataSourceInfo = "DataSourceInfo";

		private const string discoverSchemaRowsets = "DISCOVER_SCHEMA_ROWSETS";

		private const string schemaNameProp = "SchemaName";

		private const string schemaGuidProp = "SchemaGuid";

		private const string restrictionsProp = "Restrictions";

		private const string nameProp = "Name";

		private const string typeProp = "Type";

		private const string actionsSchemaRowsetName = "MDSCHEMA_ACTIONS";

		private const string actionsTypeRestrictionName = "ACTION_TYPE";

		private const string actionsContentName = "CONTENT";

		private const string showHiddenCubesPropery = "ShowHiddenCubes";

		private const int MDACTION_TYPE_URL = 1;

		private const int MDACTION_TYPE_HTML = 2;

		private const int MDACTION_TYPE_COMMANDLINE = 32;

		private const string multidimentionalProviderRestriction = "<MDP/>";

		private CubeCollection cubes;

		private AdomdConnection.XmlaClientProvider xmlaClientProvider;

		private MiningModelCollection miningModels;

		private MiningStructureCollection miningStructures;

		private MiningServiceCollection miningServices;

		private string providerVersion;

		private string serverVersion;

		private AdomdConnection.XmlaMDSchemas schemasInfos;

		private DataSet cachedActionsDataSet;

		private string clientVersion;

		private object openedReader;

		private bool userOpened;

		public string SessionID
		{
			get
			{
				return this.XmlaClientProviderEx.SessionID;
			}
			set
			{
				if (ConnectionState.Open == this.State)
				{
					throw new InvalidOperationException(SR.Connection_SessionID_SessionIsAlreadyOpen);
				}
				this.XmlaClientProviderEx.SessionID = value;
			}
		}

		public bool ShowHiddenObjects
		{
			get
			{
				return this.XmlaClientProviderEx.ShowHiddenObjects;
			}
			set
			{
				if (this.State == ConnectionState.Open)
				{
					throw new InvalidOperationException(SR.Connection_ShowHiddenObjects_ConnectionAlreadyOpen);
				}
				this.XmlaClientProviderEx.ShowHiddenObjects = value;
			}
		}

		[Browsable(false)]
		public CubeCollection Cubes
		{
			get
			{
				AdomdUtils.CheckConnectionOpened(this);
				if (this.cubes == null)
				{
					this.cubes = new CubeCollection(this);
				}
				else
				{
					this.cubes.CollectionInternal.CheckCache();
				}
				return this.cubes;
			}
		}

		[Browsable(false)]
		public MiningModelCollection MiningModels
		{
			get
			{
				AdomdUtils.CheckConnectionOpened(this);
				if (this.miningModels == null)
				{
					this.miningModels = new MiningModelCollection(this);
				}
				else
				{
					this.miningModels.CollectionInternal.CheckCache();
				}
				return this.miningModels;
			}
		}

		[Browsable(false)]
		public MiningStructureCollection MiningStructures
		{
			get
			{
				AdomdUtils.CheckConnectionOpened(this);
				if (this.miningStructures == null)
				{
					this.miningStructures = new MiningStructureCollection(this);
				}
				else
				{
					this.miningStructures.CollectionInternal.CheckCache();
				}
				return this.miningStructures;
			}
		}

		[Browsable(false)]
		public MiningServiceCollection MiningServices
		{
			get
			{
				AdomdUtils.CheckConnectionOpened(this);
				if (this.miningServices == null)
				{
					this.miningServices = new MiningServiceCollection(this);
				}
				else
				{
					this.miningServices.CollectionInternal.CheckCache();
				}
				return this.miningServices;
			}
		}

		public string ConnectionString
		{
			get
			{
				return this.XmlaClientProviderEx.ConnectionString;
			}
			set
			{
				if (this.State == ConnectionState.Open)
				{
					throw new InvalidOperationException(SR.Server_IsAlreadyConnected);
				}
				this.XmlaClientProviderEx.ConnectionString = value;
			}
		}

		[Browsable(false)]
		public int ConnectionTimeout
		{
			get
			{
				return this.XmlaClientProviderEx.ConnectionTimeout;
			}
		}

		[Browsable(false)]
		public string Database
		{
			get
			{
				if (this.State != ConnectionState.Open)
				{
					return this.XmlaClientProviderEx.GetXmlaProperty("Catalog");
				}
				string empty = string.Empty;
				return this.GetProperty("Catalog");
			}
		}

		[Browsable(false)]
		public ConnectionState State
		{
			get
			{
				if (this.XmlaClientProviderEx == null || !this.XmlaClientProviderEx.IsXmlaClientConnected)
				{
					return ConnectionState.Closed;
				}
				return ConnectionState.Open;
			}
		}

		[Browsable(false)]
		public string ProviderVersion
		{
			get
			{
				if (this.providerVersion == null)
				{
					AdomdUtils.CheckConnectionOpened(this);
					this.providerVersion = this.XmlaClientProviderEx.ProviderVersion;
				}
				return this.providerVersion;
			}
		}

		[Browsable(false)]
		public string ServerVersion
		{
			get
			{
				if (this.serverVersion == null)
				{
					AdomdUtils.CheckConnectionOpened(this);
					this.serverVersion = this.XmlaClientProviderEx.ServerVersion;
				}
				return this.serverVersion;
			}
		}

		public string ClientVersion
		{
			get
			{
				if (this.clientVersion == null)
				{
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
					this.clientVersion = versionInfo.FileVersion;
				}
				return this.clientVersion;
			}
		}

		internal object OpenedReader
		{
			set
			{
				this.openedReader = value;
			}
		}

		internal bool UserOpened
		{
			get
			{
				return this.userOpened;
			}
		}

		internal IDiscoverProvider IDiscoverProvider
		{
			get
			{
				return this.xmlaClientProvider;
			}
		}

		internal IExecuteProvider IExecuteProvider
		{
			get
			{
				return this.xmlaClientProvider;
			}
		}

		internal string CatalogConnectionStringProperty
		{
			get
			{
				return this.XmlaClientProviderEx.GetXmlaProperty("Catalog");
			}
		}

		private uint AutoSyncPeriod
		{
			get
			{
				return this.XmlaClientProviderEx.AutoSyncPeriod;
			}
		}

		private AdomdConnection.IXmlaClientProviderEx XmlaClientProviderEx
		{
			get
			{
				return this.xmlaClientProvider;
			}
		}

		public AdomdConnection()
		{
			this.xmlaClientProvider = new AdomdConnection.XmlaClientProvider();
		}

		public AdomdConnection(string connectionString) : this()
		{
			this.ConnectionString = connectionString;
		}

		public AdomdConnection(AdomdConnection connection)
		{
			this.xmlaClientProvider = new AdomdConnection.XmlaClientProvider(connection.xmlaClientProvider);
		}

		public AdomdTransaction BeginTransaction()
		{
			return this.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public AdomdTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			AdomdUtils.CheckConnectionOpened(this);
			if (isolationLevel == IsolationLevel.ReadCommitted)
			{
				return new AdomdTransaction(this);
			}
			throw new NotSupportedException();
		}

		public void ChangeEffectiveUser(string effectiveUserName)
		{
			if (string.IsNullOrEmpty(effectiveUserName))
			{
				throw new ArgumentException(SR.Connection_EffectiveUserNameEmpty);
			}
			AdomdUtils.CheckConnectionOpened(this);
			this.ResetInternalState();
			this.CloseOpenedReader();
			try
			{
				this.XmlaClientProviderEx.EndSession();
				this.XmlaClientProviderEx.SetXmlaProperty("EffectiveUserName", effectiveUserName);
				this.XmlaClientProviderEx.CreateSession(false);
			}
			catch (AdomdException)
			{
				if (this.XmlaClientProviderEx.IsXmlaClientConnected)
				{
					this.XmlaClientProviderEx.Disconnect(true);
				}
				throw;
			}
		}

		public void ChangeDatabase(string database)
		{
			if (database == null || database.Trim().Length == 0)
			{
				throw new ArgumentException(SR.Connection_DatabaseNameEmpty);
			}
			AdomdUtils.CheckConnectionOpened(this);
			if (this.IsPostYukonProvider())
			{
				this.SetProperty("Catalog", database);
			}
			else
			{
				string database2 = this.Database;
				try
				{
					if (database2 != database)
					{
						if (this.XmlaClientProviderEx.SessionID != null && !this.XmlaClientProviderEx.IsExternalSession)
						{
							this.XmlaClientProviderEx.EndSession();
						}
						else
						{
							this.XmlaClientProviderEx.SessionID = null;
						}
						this.XmlaClientProviderEx.SetXmlaProperty("Catalog", database);
						this.XmlaClientProviderEx.CreateSession(false);
					}
				}
				catch (AdomdException)
				{
					if (database2 == null || database2.Length != 0)
					{
						this.XmlaClientProviderEx.SetXmlaProperty("Catalog", database2);
					}
					if (this.XmlaClientProviderEx.IsXmlaClientConnected)
					{
						if (this.XmlaClientProviderEx.IsExternalSession)
						{
							this.XmlaClientProviderEx.ResetSession();
						}
						else
						{
							this.XmlaClientProviderEx.CreateSession(false);
						}
					}
					throw;
				}
			}
			this.ResetInternalState();
		}

		public void Close()
		{
			this.Close(true);
		}

		public void Close(bool endSession)
		{
			switch (this.State)
			{
			case ConnectionState.Closed:
				if (this.XmlaClientProviderEx != null)
				{
					this.XmlaClientProviderEx.Disconnect(false);
				}
				break;
			case ConnectionState.Open:
				this.CloseOpenedReader();
				this.XmlaClientProviderEx.Disconnect(endSession);
				break;
			}
			this.userOpened = false;
		}

		public AdomdCommand CreateCommand()
		{
			return new AdomdCommand
			{
				Connection = this
			};
		}

		public void Open()
		{
			if (this.State == ConnectionState.Open)
			{
				throw new InvalidOperationException(SR.Server_IsAlreadyConnected);
			}
			if (this.XmlaClientProviderEx.ConnectionString == null)
			{
				throw new InvalidOperationException(SR.Connection_ConnectionString_NotInitialized);
			}
			if (this.XmlaClientProviderEx.ConnectionType == ConnectionType.LocalServer)
			{
				throw new InvalidOperationException(SR.Connection_ConnectionToLocalServerNotSupported);
			}
			this.cachedActionsDataSet = null;
			string serverName = this.XmlaClientProviderEx.ServerName;
			if (serverName == null || serverName.Length == 0)
			{
				throw new InvalidOperationException(SR.Server_NoServerName);
			}
			bool flag = !this.XmlaClientProviderEx.IsExternalSession;
			bool flag2 = ConnectionInfo.IsHttpAddress(serverName);
			bool flag3 = ConnectionInfo.IsBism(serverName);
			try
			{
				if (this.XmlaClientProviderEx.ConnectTo != ConnectTo.Shiloh)
				{
					try
					{
						this.ConnectToXMLA(flag, flag2);
						goto IL_12B;
					}
					catch (AdomdConnectionException ex)
					{
						if (this.XmlaClientProviderEx.ConnectTo == ConnectTo.Default && ((!flag2 && this.XmlaClientProviderEx.InstanceName == null) || (flag2 && !AdomdConnection.AddressHasExtension(serverName)) || this.XmlaClientProviderEx.ConnectionType == ConnectionType.LocalCube))
						{
							bool flag4 = true;
							try
							{
								this.ConnectToIXMLA(flag);
							}
							catch (AdomdException)
							{
								flag4 = false;
							}
							if (!flag4)
							{
								throw;
							}
						}
						else
						{
							if (!(ex.InnerException is IOException) || !flag3)
							{
								throw;
							}
							this.XmlaClientProviderEx.UseEU = true;
							this.ConnectToXMLA(flag, flag2);
						}
						goto IL_12B;
					}
				}
				this.ConnectToIXMLA(flag);
				IL_12B:;
			}
			catch (AdomdException)
			{
				if (this.XmlaClientProviderEx.IsXmlaClientConnected)
				{
					this.XmlaClientProviderEx.Disconnect(flag);
				}
				throw;
			}
			this.ResetInternalState();
			this.XmlaClientProviderEx.MarkConnectionStringRestricted();
			this.userOpened = true;
		}

		public void Open(string path)
		{
		}

		public DataSet GetSchemaDataSet(Guid schema, object[] restrictions, bool throwOnInlineErrors)
		{
			AdomdUtils.CheckConnectionOpened(this);
			if (this.schemasInfos == null)
			{
				if (this.XmlaClientProviderEx.IsIXMLAMode)
				{
					this.schemasInfos = new AdomdConnection.XmlaMDSchemas();
				}
				else
				{
					this.RetrieveSchemaRowsets(false);
				}
			}
			AdomdConnection.XmlaMDSchema schemaInfo = this.schemasInfos.GetSchemaInfo(schema);
			ListDictionary listDictionary = new ListDictionary();
			if (restrictions != null)
			{
				AdomdConnection.XmlaMDSchemas.ConvertOleDbRestrictionsToXmlA(schemaInfo, restrictions, listDictionary);
			}
			bool flag = false;
			DataSet dataSet = this.CheckOnActionsAndSafety(schemaInfo.SchemaName, null, ref flag, listDictionary);
			if (dataSet == null)
			{
				dataSet = this.GetSchemaDataSet(schemaInfo.SchemaName, null, listDictionary, throwOnInlineErrors, null);
				if (flag)
				{
					this.FilterActionsOnSafety(dataSet);
				}
				return dataSet;
			}
			return dataSet;
		}

		public DataSet GetSchemaDataSet(string schemaName, AdomdRestrictionCollection restrictions, bool throwOnInlineErrors)
		{
			return this.GetSchemaDataSet(schemaName, null, restrictions, throwOnInlineErrors);
		}

		public DataSet GetSchemaDataSet(string schemaName, string schemaNamespace, AdomdRestrictionCollection restrictions, bool throwOnInlineErrors)
		{
			return this.GetSchemaDataSet(schemaName, schemaNamespace, restrictions, throwOnInlineErrors, null);
		}

		public DataSet GetSchemaDataSet(string schemaName, string schemaNamespace, AdomdRestrictionCollection restrictions, bool throwOnInlineErrors, AdomdPropertyCollection requestProperties)
		{
			if (string.IsNullOrEmpty(schemaName))
			{
				throw new ArgumentNullException("schemaName");
			}
			AdomdUtils.CheckConnectionOpened(this);
			bool flag = false;
			DataSet dataSet;
			if (restrictions == null)
			{
				dataSet = this.CheckOnActionsAndSafety(schemaName, schemaNamespace, ref flag, null);
			}
			else
			{
				dataSet = this.CheckOnActionsAndSafety(schemaName, schemaNamespace, ref flag, restrictions.InternalCollection);
			}
			if (dataSet == null)
			{
				AdomdRestrictionCollectionInternal adomdRestrictions = (restrictions == null) ? null : restrictions.InternalCollection;
				AdomdPropertyCollectionInternal requestProperties2 = (requestProperties == null) ? null : requestProperties.InternalCollection;
				dataSet = this.GetSchemaDataSet(schemaName, schemaNamespace, adomdRestrictions, throwOnInlineErrors, requestProperties2);
				if (flag)
				{
					this.FilterActionsOnSafety(dataSet);
				}
				return dataSet;
			}
			return dataSet;
		}

		public DataSet GetSchemaDataSet(Guid schema, object[] restrictions)
		{
			return this.GetSchemaDataSet(schema, restrictions, true);
		}

		public DataSet GetSchemaDataSet(string schemaName, AdomdRestrictionCollection restrictions)
		{
			return this.GetSchemaDataSet(schemaName, null, restrictions, true);
		}

		public DataSet GetSchemaDataSet(string schemaName, string schemaNamespace, AdomdRestrictionCollection restrictions)
		{
			return this.GetSchemaDataSet(schemaName, schemaNamespace, restrictions, true);
		}

		public void RefreshMetadata()
		{
			this.ClearMetadataCache();
		}

		public AdomdConnection Clone()
		{
			return new AdomdConnection(this);
		}

		IDbCommand IDbConnection.CreateCommand()
		{
			return this.CreateCommand();
		}

		IDbTransaction IDbConnection.BeginTransaction()
		{
			return this.BeginTransaction();
		}

		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel isolationLevel)
		{
			return this.BeginTransaction(isolationLevel);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		internal bool IsPostYukonProvider()
		{
			return this.XmlaClientProviderEx.IsPostYukonProvider();
		}

		internal object GetObjectData(SchemaObjectType schemaObjectType, string cubeName, string uniqueName)
		{
			CubeDef cubeDef = this.Cubes[cubeName];
			return cubeDef.InternalGetSchemaObject(schemaObjectType, uniqueName);
		}

		internal static void CancelCommand(AdomdConnection originalConnection)
		{
			if (originalConnection.XmlaClientProviderEx.IsIXMLAMode)
			{
				return;
			}
			AdomdConnection.IXmlaClientProviderEx xmlaClientProviderEx = new AdomdConnection.XmlaClientProvider(originalConnection.xmlaClientProvider);
			xmlaClientProviderEx.Connect();
			try
			{
				xmlaClientProviderEx.CancelCommand(originalConnection.SessionID);
			}
			finally
			{
				xmlaClientProviderEx.Disconnect(false);
			}
		}

		private string GetProperty(string propName)
		{
			return this.GetProperty(propName, false);
		}

		private string GetProperty(string propName, bool sendNSCompatibility)
		{
			return this.XmlaClientProviderEx.GetPropertyFromServer(propName, sendNSCompatibility);
		}

		internal bool HasAutoSyncTimeElapsed(DateTime origTime, DateTime nowTime)
		{
			return this.AutoSyncPeriod > 0u && (nowTime - origTime).TotalMilliseconds > this.AutoSyncPeriod;
		}

		internal void MarkCacheNeedsCheckForValidness()
		{
			if (this.cubes != null)
			{
				this.cubes.CollectionInternal.MarkCacheAsNeedCheckForValidness();
			}
			if (this.miningModels != null)
			{
				this.miningModels.CollectionInternal.MarkCacheAsNeedCheckForValidness();
			}
			if (this.miningServices != null)
			{
				this.miningServices.CollectionInternal.MarkCacheAsNeedCheckForValidness();
			}
			if (this.miningStructures != null)
			{
				this.miningStructures.CollectionInternal.MarkCacheAsNeedCheckForValidness();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					try
					{
						this.Close();
					}
					catch (AdomdException)
					{
					}
					this.xmlaClientProvider = null;
					this.userOpened = false;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void ConnectToXMLA(bool createSession, bool isHTTP)
		{
			try
			{
				this.XmlaClientProviderEx.ConnectXmla();
				if (isHTTP)
				{
					this.ReadDataSourceInfo();
				}
				if (createSession)
				{
					this.XmlaClientProviderEx.CreateSession(true);
				}
				else
				{
					this.GetProperty("Catalog", true);
				}
			}
			catch
			{
				if (this.XmlaClientProviderEx.IsXmlaClientConnected)
				{
					this.XmlaClientProviderEx.Disconnect(createSession);
				}
				throw;
			}
		}

		private void ConnectToIXMLA(bool createSession)
		{
			try
			{
				this.XmlaClientProviderEx.ConnectIXmla();
				if (createSession)
				{
					this.XmlaClientProviderEx.CreateSession(false);
				}
				else
				{
					this.GetProperty("Catalog");
				}
			}
			catch
			{
				if (this.XmlaClientProviderEx.IsXmlaClientConnected)
				{
					this.XmlaClientProviderEx.Disconnect(createSession);
				}
				throw;
			}
		}

		private void ReadDataSourceInfo()
		{
			ListDictionary listDictionary = new ListDictionary();
			listDictionary["ProviderType"] = "<MDP/>";
			RowsetFormatter rowsetFormatter = this.XmlaClientProviderEx.Discover("DISCOVER_DATASOURCES", null, InlineErrorHandlingType.StoreInCell, false);
			DataRowCollection rows = rowsetFormatter.MainRowsetTable.Rows;
			if (rows.Count <= 0)
			{
				throw new AdomdUnknownResponseException(SR.Connection_NoInformationAboutDataSourcesReturned, "");
			}
			DataRow dataRow = rows[0];
			if (!rowsetFormatter.MainRowsetTable.Columns.Contains("DataSourceInfo"))
			{
				throw new AdomdUnknownResponseException(SR.Connection_NoInformationAboutDataSourcesReturned, "");
			}
			if (this.XmlaClientProviderEx.GetXmlaProperty("DataSourceInfo") == null)
			{
				string text = AdomdUtils.GetProperty(dataRow, "DataSourceInfo") as string;
				if (text != null)
				{
					this.XmlaClientProviderEx.SetXmlaProperty("DataSourceInfo", text);
				}
			}
		}

		private void RetrieveSchemaRowsets(bool createSession)
		{
			RowsetFormatter rowsetFormatter;
			if (createSession)
			{
				rowsetFormatter = this.XmlaClientProviderEx.DiscoverWithCreateSession("DISCOVER_SCHEMA_ROWSETS", false);
			}
			else
			{
				rowsetFormatter = this.XmlaClientProviderEx.Discover("DISCOVER_SCHEMA_ROWSETS", null, InlineErrorHandlingType.StoreInCell, false);
			}
			if (rowsetFormatter.RowsetDataset.Tables.Count <= 1)
			{
				throw new AdomdUnknownResponseException(SR.Schema_UnexpectedResponseForSchema("DISCOVER_SCHEMA_ROWSETS"), "");
			}
			this.schemasInfos = new AdomdConnection.XmlaMDSchemas(rowsetFormatter);
		}

		private DataSet GetSchemaDataSet(string schemaName, string schemaNamespace, IDictionary adomdRestrictions, bool throwOnInlineErrors, IDictionary requestProperties)
		{
			RowsetFormatter rowsetFormatter = this.XmlaClientProviderEx.Discover(schemaName, schemaNamespace, adomdRestrictions, throwOnInlineErrors ? InlineErrorHandlingType.Throw : InlineErrorHandlingType.StoreInErrorsCollection, requestProperties);
			DataSet rowsetDataset = rowsetFormatter.RowsetDataset;
			AdomdConnection.XmlaMDSchemas.MungeMembersSchemaColumnNames(schemaName, schemaNamespace, rowsetDataset);
			return rowsetDataset;
		}

		private void SetProperty(string propertyName, string propValue)
		{
			if (propertyName == null || propertyName.Trim().Length == 0)
			{
				throw new ArgumentException(SR.Connection_PropertyNameEmpty, "propertyName");
			}
			string xmlaProperty = this.XmlaClientProviderEx.GetXmlaProperty(propertyName);
			this.XmlaClientProviderEx.SetXmlaProperty(propertyName, propValue);
			try
			{
				if (this.GetProperty(propertyName) != propValue)
				{
					throw new NotSupportedException(SR.Connection_FailedToSetProperty(propertyName, propValue));
				}
			}
			catch
			{
				this.XmlaClientProviderEx.SetXmlaProperty(propertyName, xmlaProperty);
				throw;
			}
		}

		private static bool AddressHasExtension(string serverName)
		{
			bool result = false;
			try
			{
				string extension = Path.GetExtension(new Uri(serverName).LocalPath);
				result = (extension != null && extension.Length > 0);
			}
			catch (ArgumentNullException)
			{
				result = false;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			catch (UriFormatException)
			{
				result = false;
			}
			return result;
		}

		private void CloseOpenedReader()
		{
			if (this.openedReader != null)
			{
				if (this.openedReader is AdomdDataReader)
				{
					AdomdDataReader adomdDataReader = this.openedReader as AdomdDataReader;
					adomdDataReader.Close();
				}
				else if (this.openedReader is XmlReader)
				{
					XmlReader xmlReader = this.openedReader as XmlReader;
					xmlReader.Close();
				}
			}
			this.openedReader = null;
		}

		private void ResetInternalState()
		{
			this.ClearMetadataCache();
			this.providerVersion = null;
			this.serverVersion = null;
			this.XmlaClientProviderEx.ResetInternalState();
		}

		private void ClearMetadataCache()
		{
			if (this.cubes != null)
			{
				this.cubes.CollectionInternal.AbandonCache();
				this.cubes = null;
			}
			if (this.miningModels != null)
			{
				this.miningModels.CollectionInternal.AbandonCache();
				this.miningModels = null;
			}
			if (this.miningServices != null)
			{
				this.miningServices.CollectionInternal.AbandonCache();
				this.miningServices = null;
			}
			if (this.miningStructures != null)
			{
				this.miningStructures.CollectionInternal.AbandonCache();
				this.miningStructures = null;
			}
		}

		private DataSet CheckOnActionsAndSafety(string schemaName, string schemaNamespace, ref bool restrictActionsOnSafe, IDictionary restrictions)
		{
			if (this.XmlaClientProviderEx.SafetyOptions != SafetyOptions.All && schemaName == "MDSCHEMA_ACTIONS" && (schemaNamespace == null || schemaNamespace.Length == 0))
			{
				if (this.XmlaClientProviderEx.SafetyOptions == SafetyOptions.None)
				{
					return this.GetEmptyActionsDataSet(restrictions);
				}
				restrictActionsOnSafe = true;
			}
			else
			{
				restrictActionsOnSafe = false;
			}
			return null;
		}

		private void FilterActionsOnSafety(DataSet dataSet)
		{
			if (dataSet.Tables.Count != 1)
			{
				throw new AdomdUnknownResponseException(SR.Schema_UnexpectedResponseForSchema("MDSCHEMA_ACTIONS"), "");
			}
			DataTable dataTable = dataSet.Tables[0];
			string address = string.Empty;
			int i = 0;
			while (i < dataTable.Rows.Count)
			{
				string columnError = dataTable.Rows[i].GetColumnError("ACTION_TYPE");
				if (!string.IsNullOrEmpty(columnError))
				{
					throw new AdomdErrorResponseException(columnError);
				}
				int num = (int)dataTable.Rows[i]["ACTION_TYPE"];
				if (num == 2 || num == 32)
				{
					dataTable.Rows.RemoveAt(i);
				}
				else
				{
					if (num == 1)
					{
						columnError = dataTable.Rows[i].GetColumnError("CONTENT");
						if (!string.IsNullOrEmpty(columnError))
						{
							throw new AdomdErrorResponseException(columnError);
						}
						address = (dataTable.Rows[i]["CONTENT"] as string);
						if (!ConnectionInfo.IsHttpAddress(address))
						{
							dataTable.Rows.RemoveAt(i);
							continue;
						}
					}
					i++;
				}
			}
		}

		private DataSet GetEmptyActionsDataSet(IDictionary restrictions)
		{
			if (this.cachedActionsDataSet == null)
			{
				RowsetFormatter rowsetFormatter = this.XmlaClientProviderEx.DiscoverSchema("MDSCHEMA_ACTIONS", restrictions, InlineErrorHandlingType.Throw);
				this.cachedActionsDataSet = rowsetFormatter.RowsetDataset;
			}
			return this.cachedActionsDataSet.Clone();
		}
	}
}
