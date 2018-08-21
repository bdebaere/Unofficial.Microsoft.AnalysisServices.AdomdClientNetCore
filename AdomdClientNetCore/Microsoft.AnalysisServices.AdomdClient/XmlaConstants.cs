using System;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class XmlaConstants
	{
		internal const string EmptyNamespace = "";

		internal const string EnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

		internal const string XmlaNamespace = "urn:schemas-microsoft-com:xml-analysis";

		internal const string XmlaExtensionsNamespace = "http://schemas.microsoft.com/analysisservices/2003/xmla";

		internal const string PrivateExtensionsNamespace = "http://schemas.microsoft.com/analysisservices/2003/ext";

		internal const string MultipleResultsNamespace = "http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults";

		internal const string FaultNamespace = "urn:schemas-microsoft-com:xml-analysis:fault";

		internal const string DdlNamespace = "http://schemas.microsoft.com/analysisservices/2003/engine";

		internal const string DdlNamespace2 = "http://schemas.microsoft.com/analysisservices/2003/engine/2";

		internal const string DdlNamespace2_2 = "http://schemas.microsoft.com/analysisservices/2003/engine/2/2";

		internal const string DdlNamespace100 = "http://schemas.microsoft.com/analysisservices/2008/engine/100";

		internal const string DdlNamespace100_100 = "http://schemas.microsoft.com/analysisservices/2008/engine/100/100";

		internal const string DdlNamespace200 = "http://schemas.microsoft.com/analysisservices/2010/engine/200";

		internal const string DdlNamespace200_200 = "http://schemas.microsoft.com/analysisservices/2010/engine/200/200";

		internal const string DdlNamespace300 = "http://schemas.microsoft.com/analysisservices/2011/engine/300";

		internal const string DdlNamespace300_300 = "http://schemas.microsoft.com/analysisservices/2011/engine/300/300";

		internal const string DdlNamespace400 = "http://schemas.microsoft.com/analysisservices/2012/engine/400";

		internal const string DdlNamespace400_400 = "http://schemas.microsoft.com/analysisservices/2012/engine/400/400";

		internal const string DdlNamespace410 = "http://schemas.microsoft.com/analysisservices/2012/engine/410";

		internal const string DdlNamespace410_410 = "http://schemas.microsoft.com/analysisservices/2012/engine/410/410";

		internal const string DdlNamespace500 = "http://schemas.microsoft.com/analysisservices/2013/engine/500";

		internal const string DdlNamespace500_500 = "http://schemas.microsoft.com/analysisservices/2013/engine/500/500";

		internal const string RowsetNamespace = "urn:schemas-microsoft-com:xml-analysis:rowset";

		internal const string SqlNamespace = "urn:schemas-microsoft-com:xml-sql";

		internal const string DatasetNamespace = "urn:schemas-microsoft-com:xml-analysis:mddataset";

		internal const string XsdNamespace = "http://www.w3.org/2001/XMLSchema";

		internal const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		internal const string EmptyResultNamespace = "urn:schemas-microsoft-com:xml-analysis:empty";

		internal const string ExceptionNamespace = "urn:schemas-microsoft-com:xml-analysis:exception";

		internal const string XmlDocumentColumnDatasetNamespace = "urn:schemas-microsoft-com:xml-analysis:xmlDocumentDataset";

		internal const string XsdNamespaceAttribute = "xmlns:xsd";

		internal const string XsiNamespaceAttribute = "xmlns:xsi";

		internal const string TypeAttribute = "xsi:type";

		internal const string NullTypeAttribute = "xsi:nil";

		internal const string MustUnderstandAttribute = "mustUnderstand";

		internal const string SessionIdAttribute = "SessionId";

		internal const string SequenceAttribute = "Sequence";

		internal const string DescriptionElementAttribute = "Description";

		internal const string SourceAttribute = "Source";

		internal const string HelpFileAttribute = "HelpFile";

		internal const string WarningCodeAttribute = "WarningCode";

		internal const string TransactionAttribute = "Transaction";

		internal const string ProcessAffectedObjectsAttribute = "ProcessAffectedObjects";

		internal const string SkipVolatileObjectsAttribute = "SkipVolatileObjects";

		internal const string ErrorCodeElementAttribute = "ErrorCode";

		internal const string MustUnderstandFalse = "0";

		internal const string MustUnderstandTrue = "1";

		internal const string BinaryXmlCapabilityValue = "sx";

		internal const string CompressionCapabilityValue = "xpress";

		internal const string SchannelAuthProtocol = "Schannel";

		internal const string RestrictionTypeNSPrefix = "rt";

		internal const string XmlnsPrefix = "xmlns";

		internal const string SoapPrefix = "soap";

		internal const string BatchElement = "Batch";

		internal const string KeyErrorLimitElement = "KeyErrorLimit";

		internal const string KeyErrorLogFileElement = "KeyErrorLogFile";

		internal const string DiscoverElement = "Discover";

		internal const string RequestTypeElement = "RequestType";

		internal const string RestrictionsElement = "Restrictions";

		internal const string RestrictionListElement = "RestrictionList";

		internal const string RowElement = "row";

		internal const string SessionIDElement = "SessionID";

		internal const string ConnectionIDElement = "ConnectionID";

		internal const string SPIDElement = "SPID";

		internal const string DatabaseIDElement = "DatabaseID";

		internal const string DatabaseNameElement = "DatabaseName";

		internal const string AllowOverwrite = "AllowOverwrite";

		internal const string CancelAssociatedElement = "CancelAssociated";

		internal const string ExceptionElement = "Exception";

		internal const string MessagesElement = "Messages";

		internal const string CancelElement = "Cancel";

		internal const string HeaderElement = "Header";

		internal const string SessionElement = "Session";

		internal const string BeginSessionElement = "BeginSession";

		internal const string BeginGetSessionTokenElement = "BeginGetSessionToken";

		internal const string SetAuthContextElement = "SetAuthContext";

		internal const string ImageLoadElement = "ImageLoad";

		internal const string ImageSaveElement = "ImageSave";

		internal const string VersionElement = "Version";

		internal const string EndSessionElement = "EndSession";

		internal const string EnvelopeElement = "Envelope";

		internal const string AuthenticateElement = "Authenticate";

		internal const string AuthProtocolElement = "AuthProtocol";

		internal const string SspiHandshakeElement = "SspiHandshake";

		internal const string AuthTokenElement = "AuthToken";

		internal const string BodyElement = "Body";

		internal const string FaultElement = "Fault";

		internal const string ReturnElement = "return";

		internal const string ResultsElement = "results";

		internal const string RootElement = "root";

		internal const string FaultcodeElement = "faultcode";

		internal const string FaultstringElement = "faultstring";

		internal const string FaultactorElement = "faultactor";

		internal const string DetailElement = "detail";

		internal const string SchemaElement = "schema";

		internal const string ErrorElement = "Error";

		internal const string ErrorLocationElement = "Location";

		internal const string StartElement = "Start";

		internal const string EndElement = "End";

		internal const string LineElement = "Line";

		internal const string ColumnElement = "Column";

		internal const string LineOffsetElement = "LineOffset";

		internal const string TextLengthElement = "TextLength";

		internal const string WarningElement = "Warning";

		internal const string RowNumber = "RowNumber";

		internal const string SourceObject = "SourceObject";

		internal const string DependsOnObject = "DependsOnObject";

		internal const string ErrorCallStack = "CallStack";

		internal const string Dimension = "Dimension";

		internal const string Hierarchy = "Hierarchy";

		internal const string Attribute = "Attribute";

		internal const string Cube = "Cube";

		internal const string MeasureGroup = "MeasureGroup";

		internal const string MemberName = "MemberName";

		internal const string TableName = "TableName";

		internal const string ColumnName = "ColumnName";

		internal const string PartitionName = "PartitionName";

		internal const string MeasureName = "MeasureName";

		internal const string RoleName = "RoleName";

		internal const string ExecuteElement = "Execute";

		internal const string CommandElement = "Command";

		internal const string StatementElement = "Statement";

		internal const string PropertiesElement = "Properties";

		internal const string PropertyListElement = "PropertyList";

		internal const string ParametersElement = "Parameters";

		internal const string ParamElement = "Parameter";

		internal const string ParamNameElement = "Name";

		internal const string ParamValueElement = "Value";

		internal const string TokenElement = "Token";

		internal const string Role = "Role";

		internal const string SessionTokenElement = "SessionToken";

		internal const string AuthenticationSchemeElement = "AuthenticationScheme";

		internal const string AuthenticationSchemeDelegateToken = "DelegateToken";

		internal const string ScopeConnectionElement = "ScopeConnection";

		internal const string AccessTypeAttribute = "AccessType";

		internal const string AccessTypeReadOnly = "ReadOnly";

		internal const string AccessTypeDbAdmin = "DbAdmin";

		internal const string UserTokenElement = "UserToken";

		internal const string ExtAuthElement = "ExtAuth";

		internal const string ExtAuthInfoElement = "ExtAuthInfo";

		internal const string IdentityProviderElement = "IdentityProvider";

		internal const string BypassAuthorizationElement = "BypassAuthorization";

		internal const string RestrictCatalogElement = "RestrictCatalog";

		internal const string AccessModeElement = "AccessMode";

		internal const string UserNameElement = "UserName";

		internal const string AuthenticateResponseElement = "AuthenticateResponse";

		internal const string ExecuteResponseElement = "ExecuteResponse";

		internal const string DiscoverResponseElement = "DiscoverResponse";

		internal const string OlapInfoElement = "OlapInfo";

		internal const string AxesInfoElement = "AxesInfo";

		internal const string AxisInfoElement = "AxisInfo";

		internal const string HierarchyInfoElement = "HierarchyInfo";

		internal const string CellInfoElement = "CellInfo";

		internal const string SlicerAxisName = "SlicerAxis";

		internal const string AxesElement = "Axes";

		internal const string AxisElement = "Axis";

		internal const string TuplesElement = "Tuples";

		internal const string TupleElement = "Tuple";

		internal const string CellDataElement = "CellData";

		internal const string CellElement = "Cell";

		internal const string MemberElement = "Member";

		internal const string CubeInfoElement = "CubeInfo";

		internal const string CubeNameElement = "CubeName";

		internal const string LastDataUpdateElement = "LastDataUpdate";

		internal const string CubeElement = "Cube";

		internal const string LastSchemaUpdateElement = "LastSchemaUpdate";

		internal const string HierarchyAttribute = "Hierarchy";

		internal const string CellOrdinalAttribute = "CellOrdinal";

		internal const string ValueProperty = "Value";

		internal const string DescriptionProperty = "Description";

		internal const string FmtValueProperty = "FmtValue";

		internal const string UniqueNameProperty = "UName";

		internal const string LevelNameProperty = "LName";

		internal const string LevelNumberProperty = "LNum";

		internal const string CaptionProperty = "Caption";

		internal const string ParentProperty = "Parent";

		internal const string DisplayInfoProperty = "DisplayInfo";

		internal const string NameAtribute = "name";

		internal const string GuidColumnType = "uuid";

		internal const string XmlDocumentColumnType = "xmlDocument";

		internal const string TypeAttributeName = "type";

		internal const string OccurenceUnbounded = "unbounded";

		internal const string FieldAttributeName = "field";

		internal const string NilAttributeName = "nil";

		internal const string TrueConstant = "true";

		internal const string DiscoverSoapAction = "SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Discover\"";

		internal const string ExecuteSoapAction = "SOAPAction: \"urn:schemas-microsoft-com:xml-analysis:Execute\"";

		internal const string DiscoverInstances = "DISCOVER_INSTANCES";

		internal const string InstanceNameRestriction = "INSTANCE_NAME";

		internal const string PortRestriction = "INSTANCE_PORT_NUMBER";

		internal const string DiscoverProperties = "DISCOVER_PROPERTIES";

		internal const string PropertyName = "PropertyName";

		internal const string ImpactAnalysisProperty = "ImpactAnalysis";

		internal const string DatabaseReadWriteModeElement = "ReadWriteMode";

		internal const string DatabaseReadWriteMode = "ReadWrite";

		internal const string DatabaseReadOnlyExclusiveMode = "ReadOnlyExclusive";

		internal const string AffectedObjectsElement = "AffectedObjects";

		internal const string BaseVersionAttribute = "BaseVersion";

		internal const string CurrentVersionAttribute = "CurrentVersion";

		internal static NameTable GetNameTable()
		{
			NameTable nameTable = new NameTable();
			nameTable.Add("");
			nameTable.Add("http://schemas.xmlsoap.org/soap/envelope/");
			nameTable.Add("urn:schemas-microsoft-com:xml-analysis");
			nameTable.Add("http://schemas.microsoft.com/analysisservices/2003/xmla");
			nameTable.Add("http://schemas.microsoft.com/analysisservices/2003/ext");
			nameTable.Add("http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults");
			nameTable.Add("urn:schemas-microsoft-com:xml-analysis:fault");
			nameTable.Add("http://schemas.microsoft.com/analysisservices/2003/engine");
			nameTable.Add("urn:schemas-microsoft-com:xml-analysis:rowset");
			nameTable.Add("urn:schemas-microsoft-com:xml-sql");
			nameTable.Add("urn:schemas-microsoft-com:xml-analysis:mddataset");
			nameTable.Add("http://www.w3.org/2001/XMLSchema");
			nameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
			nameTable.Add("urn:schemas-microsoft-com:xml-analysis:empty");
			nameTable.Add("urn:schemas-microsoft-com:xml-analysis:exception");
			nameTable.Add("urn:schemas-microsoft-com:xml-analysis:xmlDocumentDataset");
			nameTable.Add("xmlns:xsd");
			nameTable.Add("xmlns:xsi");
			nameTable.Add("xsi:type");
			nameTable.Add("mustUnderstand");
			nameTable.Add("SessionId");
			nameTable.Add("Description");
			nameTable.Add("Source");
			nameTable.Add("HelpFile");
			nameTable.Add("WarningCode");
			nameTable.Add("Transaction");
			nameTable.Add("ProcessAffectedObjects");
			nameTable.Add("KeyErrorLimit");
			nameTable.Add("KeyErrorLogFile");
			nameTable.Add("ErrorCode");
			nameTable.Add("0");
			nameTable.Add("1");
			nameTable.Add("sx");
			nameTable.Add("xpress");
			nameTable.Add("rt");
			nameTable.Add("xmlns");
			nameTable.Add("Batch");
			nameTable.Add("Discover");
			nameTable.Add("RequestType");
			nameTable.Add("Restrictions");
			nameTable.Add("RestrictionList");
			nameTable.Add("row");
			nameTable.Add("SessionID");
			nameTable.Add("ConnectionID");
			nameTable.Add("SPID");
			nameTable.Add("CancelAssociated");
			nameTable.Add("Exception");
			nameTable.Add("Messages");
			nameTable.Add("Cancel");
			nameTable.Add("Header");
			nameTable.Add("Session");
			nameTable.Add("BeginSession");
			nameTable.Add("EndSession");
			nameTable.Add("Envelope");
			nameTable.Add("Authenticate");
			nameTable.Add("SspiHandshake");
			nameTable.Add("Body");
			nameTable.Add("Fault");
			nameTable.Add("return");
			nameTable.Add("results");
			nameTable.Add("root");
			nameTable.Add("faultcode");
			nameTable.Add("faultstring");
			nameTable.Add("faultactor");
			nameTable.Add("detail");
			nameTable.Add("schema");
			nameTable.Add("Error");
			nameTable.Add("Location");
			nameTable.Add("Start");
			nameTable.Add("End");
			nameTable.Add("Line");
			nameTable.Add("Column");
			nameTable.Add("LineOffset");
			nameTable.Add("TextLength");
			nameTable.Add("Warning");
			nameTable.Add("Execute");
			nameTable.Add("Command");
			nameTable.Add("Statement");
			nameTable.Add("Properties");
			nameTable.Add("PropertyList");
			nameTable.Add("Parameters");
			nameTable.Add("Parameter");
			nameTable.Add("Name");
			nameTable.Add("Value");
			nameTable.Add("AuthenticateResponse");
			nameTable.Add("ExecuteResponse");
			nameTable.Add("DiscoverResponse");
			nameTable.Add("uuid");
			nameTable.Add("xmlDocument");
			nameTable.Add("type");
			nameTable.Add("unbounded");
			nameTable.Add("field");
			nameTable.Add("nil");
			nameTable.Add("true");
			nameTable.Add("OlapInfo");
			nameTable.Add("AxesInfo");
			nameTable.Add("AxisInfo");
			nameTable.Add("HierarchyInfo");
			nameTable.Add("CellInfo");
			nameTable.Add("SlicerAxis");
			nameTable.Add("Axes");
			nameTable.Add("Axis");
			nameTable.Add("Tuples");
			nameTable.Add("Tuple");
			nameTable.Add("Member");
			nameTable.Add("CellData");
			nameTable.Add("Cell");
			nameTable.Add("CubeInfo");
			nameTable.Add("Cube");
			nameTable.Add("CubeName");
			nameTable.Add("LastDataUpdate");
			nameTable.Add("LastSchemaUpdate");
			nameTable.Add("name");
			nameTable.Add("Hierarchy");
			nameTable.Add("CellOrdinal");
			nameTable.Add("Value");
			nameTable.Add("FmtValue");
			nameTable.Add("UName");
			nameTable.Add("LName");
			nameTable.Add("LNum");
			nameTable.Add("Caption");
			nameTable.Add("Parent");
			nameTable.Add("Description");
			nameTable.Add("DisplayInfo");
			return nameTable;
		}
	}
}
