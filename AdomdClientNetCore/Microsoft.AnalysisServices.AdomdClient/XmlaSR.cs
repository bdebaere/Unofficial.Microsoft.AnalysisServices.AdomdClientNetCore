using System;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[CompilerGenerated]
	internal class XmlaSR
	{
		[CompilerGenerated]
		public class Keys
		{
			public const string AlreadyConnected = "AlreadyConnected";

			public const string NotConnected = "NotConnected";

			public const string LocalCube_FileNotOpened = "LocalCube_FileNotOpened";

			public const string CannotConnect = "CannotConnect";

			public const string CannotConnectToRedirector = "CannotConnectToRedirector";

			public const string ConnectionBroken = "ConnectionBroken";

			public const string Instance_NotFound = "Instance_NotFound";

			public const string Reconnect_ConnectionInfoIsMissing = "Reconnect_ConnectionInfoIsMissing";

			public const string Reconnect_SessionIDIsMissing = "Reconnect_SessionIDIsMissing";

			public const string ServerDidNotProvideErrorInfo = "ServerDidNotProvideErrorInfo";

			public const string UnexpectedXsiType = "UnexpectedXsiType";

			public const string ConnectionCannotBeUsedWhileXmlReaderOpened = "ConnectionCannotBeUsedWhileXmlReaderOpened";

			public const string Connect_RedirectorDidntReturnDatabaseInfo = "Connect_RedirectorDidntReturnDatabaseInfo";

			public const string Connection_WorkbookIsOutdated = "Connection_WorkbookIsOutdated";

			public const string Connection_AnalysisServicesInstanceWasMoved = "Connection_AnalysisServicesInstanceWasMoved";

			public const string SoapFormatter_ResponseIsNotRowset = "SoapFormatter_ResponseIsNotRowset";

			public const string SoapFormatter_ResponseIsNotDataset = "SoapFormatter_ResponseIsNotDataset";

			public const string Cancel_SessionIDNotSpecified = "Cancel_SessionIDNotSpecified";

			public const string ConnectionString_Invalid = "ConnectionString_Invalid";

			public const string ConnectionString_InvalidPropertyNameFormat = "ConnectionString_InvalidPropertyNameFormat";

			public const string ConnectionString_DataSourceNotSpecified = "ConnectionString_DataSourceNotSpecified";

			public const string ConnectionString_UnsupportedPropertyValue = "ConnectionString_UnsupportedPropertyValue";

			public const string ConnectionString_OpenedQuoteIsNotClosed = "ConnectionString_OpenedQuoteIsNotClosed";

			public const string ConnectionString_ExpectedSemicolonNotFound = "ConnectionString_ExpectedSemicolonNotFound";

			public const string ConnectionString_ExpectedEqualSignNotFound = "ConnectionString_ExpectedEqualSignNotFound";

			public const string ConnectionString_InvalidCharInPropertyName = "ConnectionString_InvalidCharInPropertyName";

			public const string ConnectionString_InvalidCharInUnquotedPropertyValue = "ConnectionString_InvalidCharInUnquotedPropertyValue";

			public const string ConnectionString_PropertyNameNotDefined = "ConnectionString_PropertyNameNotDefined";

			public const string ConnectionString_InvalidIntegratedSecurityForNative = "ConnectionString_InvalidIntegratedSecurityForNative";

			public const string ConnectionString_InvalidProtectionLevelForHttp = "ConnectionString_InvalidProtectionLevelForHttp";

			public const string ConnectionString_InvalidProtectionLevelForHttps = "ConnectionString_InvalidProtectionLevelForHttps";

			public const string ConnectionString_InvalidImpersonationLevelForHttp = "ConnectionString_InvalidImpersonationLevelForHttp";

			public const string ConnectionString_InvalidImpersonationLevelForHttps = "ConnectionString_InvalidImpersonationLevelForHttps";

			public const string ConnectionString_InvalidIntegratedSecurityForHttpOrHttps = "ConnectionString_InvalidIntegratedSecurityForHttpOrHttps";

			public const string ConnectionString_MissingIdentityProviderForIntegratedSecurityFederated = "ConnectionString_MissingIdentityProviderForIntegratedSecurityFederated";

			public const string ConnectionString_InvalidIdentityProviderForIntegratedSecurityFederated = "ConnectionString_InvalidIdentityProviderForIntegratedSecurityFederated";

			public const string ConnectionString_PropertyNotApplicableWithTheDataSourceType = "ConnectionString_PropertyNotApplicableWithTheDataSourceType";

			public const string ConnectionString_DataSourceTypeDoesntSupportQuery = "ConnectionString_DataSourceTypeDoesntSupportQuery";

			public const string ConnectionString_LinkFileInvalidServer = "ConnectionString_LinkFileInvalidServer";

			public const string ConnectionString_LinkFileParseError = "ConnectionString_LinkFileParseError";

			public const string ConnectionString_LinkFileDupEffectiveUsername = "ConnectionString_LinkFileDupEffectiveUsername";

			public const string ConnectionString_LinkFileDownloadError = "ConnectionString_LinkFileDownloadError";

			public const string ConnectionString_LinkFileCannotRevert = "ConnectionString_LinkFileCannotRevert";

			public const string ConnectionString_LinkFileCannotDelegate = "ConnectionString_LinkFileCannotDelegate";

			public const string ConnectionString_MissingPassword = "ConnectionString_MissingPassword";

			public const string ConnectionString_ExternalConnectionIsIncomplete = "ConnectionString_ExternalConnectionIsIncomplete";

			public const string ConnectionString_AsAzure_DataSourcePathMoreThanOneSegment = "ConnectionString_AsAzure_DataSourcePathMoreThanOneSegment";

			public const string UnknownServerResponseFormat = "UnknownServerResponseFormat";

			public const string AfterExceptionAllTagsShouldCloseUntilMessagesSection = "AfterExceptionAllTagsShouldCloseUntilMessagesSection";

			public const string UnrecognizedElementInMessagesSection = "UnrecognizedElementInMessagesSection";

			public const string ErrorCodeIsMissingFromRowsetError = "ErrorCodeIsMissingFromRowsetError";

			public const string ErrorCodeIsMissingFromDatasetError = "ErrorCodeIsMissingFromDatasetError";

			public const string ExceptionRequiresXmlaErrorsInMessagesSection = "ExceptionRequiresXmlaErrorsInMessagesSection";

			public const string MessagesSectionIsEmpty = "MessagesSectionIsEmpty";

			public const string EmptyRootIsNotEmpty = "EmptyRootIsNotEmpty";

			public const string UnexpectedElement = "UnexpectedElement";

			public const string MissingElement = "MissingElement";

			public const string Resultset_IsNotRowset = "Resultset_IsNotRowset";

			public const string DataReaderClosedError = "DataReaderClosedError";

			public const string DataReaderInvalidRowError = "DataReaderInvalidRowError";

			public const string NonSequentialColumnAccessError = "NonSequentialColumnAccessError";

			public const string DataReader_IndexOutOfRange = "DataReader_IndexOutOfRange";

			public const string Authentication_Failed = "Authentication_Failed";

			public const string Authentication_Sspi_PackageNotFound = "Authentication_Sspi_PackageNotFound";

			public const string Authentication_Sspi_PackageDoesntSupportCapability = "Authentication_Sspi_PackageDoesntSupportCapability";

			public const string Authentication_Sspi_FlagNotEstablished = "Authentication_Sspi_FlagNotEstablished";

			public const string Authentication_Sspi_SchannelCantDelegate = "Authentication_Sspi_SchannelCantDelegate";

			public const string Authentication_Sspi_SchannelSupportsOnlyPrivacyLevel = "Authentication_Sspi_SchannelSupportsOnlyPrivacyLevel";

			public const string Authentication_Sspi_SchannelUnsupportedImpersonationLevel = "Authentication_Sspi_SchannelUnsupportedImpersonationLevel";

			public const string Authentication_Sspi_SchannelUnsupportedProtectionLevel = "Authentication_Sspi_SchannelUnsupportedProtectionLevel";

			public const string Authentication_Sspi_SchannelAnonymousAmbiguity = "Authentication_Sspi_SchannelAnonymousAmbiguity";

			public const string Authentication_MsoID_MissingSignInAssistant = "Authentication_MsoID_MissingSignInAssistant";

			public const string Authentication_MsoID_InternalError = "Authentication_MsoID_InternalError";

			public const string Authentication_MsoID_InvalidCredentials = "Authentication_MsoID_InvalidCredentials";

			public const string Authentication_MsoID_SsoFailed = "Authentication_MsoID_SsoFailed";

			public const string Authentication_MsoID_SsoFailedNonDomainUser = "Authentication_MsoID_SsoFailedNonDomainUser";

			public const string Authentication_ClaimsToken_AuthorityNotFound = "Authentication_ClaimsToken_AuthorityNotFound";

			public const string Authentication_ClaimsToken_UserIdAndPasswordRequired = "Authentication_ClaimsToken_UserIdAndPasswordRequired";

			public const string Authentication_ClaimsToken_IdentityProviderFormatInvalid = "Authentication_ClaimsToken_IdentityProviderFormatInvalid";

			public const string Authentication_ClaimsToken_AdalLoadingError = "Authentication_ClaimsToken_AdalLoadingError";

			public const string Authentication_ClaimsToken_AdalError = "Authentication_ClaimsToken_AdalError";

			public const string Authentication_AsAzure_OnlyClaimsTokenSupported = "Authentication_AsAzure_OnlyClaimsTokenSupported";

			public const string DimeReader_CannotReadFromStream = "DimeReader_CannotReadFromStream";

			public const string DimeReader_IsClosed = "DimeReader_IsClosed";

			public const string DimeReader_PreviousRecordStreamStillOpened = "DimeReader_PreviousRecordStreamStillOpened";

			public const string DimeRecord_StreamShouldBeReadable = "DimeRecord_StreamShouldBeReadable";

			public const string DimeRecord_StreamShouldBeWriteable = "DimeRecord_StreamShouldBeWriteable";

			public const string DimeRecord_InvalidContentLength = "DimeRecord_InvalidContentLength";

			public const string DimeRecord_PropertyOnlyAvailableForReadRecords = "DimeRecord_PropertyOnlyAvailableForReadRecords";

			public const string DimeRecord_InvalidChunkSize = "DimeRecord_InvalidChunkSize";

			public const string DimeRecord_UnableToReadFromStream = "DimeRecord_UnableToReadFromStream";

			public const string DimeRecord_StreamIsClosed = "DimeRecord_StreamIsClosed";

			public const string DimeRecord_ReadNotAllowed = "DimeRecord_ReadNotAllowed";

			public const string DimeRecord_WriteNotAllowed = "DimeRecord_WriteNotAllowed";

			public const string DimeRecord_TypeFormatEnumUnchangedNotAllowed = "DimeRecord_TypeFormatEnumUnchangedNotAllowed";

			public const string DimeRecord_MediaTypeNotDefined = "DimeRecord_MediaTypeNotDefined";

			public const string DimeRecord_InvalidUriFormat = "DimeRecord_InvalidUriFormat";

			public const string DimeRecord_NameMustNotBeDefinedForFormatNone = "DimeRecord_NameMustNotBeDefinedForFormatNone";

			public const string DimeRecord_EncodedTypeLengthExceeds8191 = "DimeRecord_EncodedTypeLengthExceeds8191";

			public const string DimeRecord_OffsetAndCountShouldBePositive = "DimeRecord_OffsetAndCountShouldBePositive";

			public const string DimeRecord_ContentLengthExceeded = "DimeRecord_ContentLengthExceeded";

			public const string DimeRecord_VersionNotSupported = "DimeRecord_VersionNotSupported";

			public const string DimeRecord_OnlySingleRecordMessagesAreSupported = "DimeRecord_OnlySingleRecordMessagesAreSupported";

			public const string DimeRecord_TypeFormatShouldBeMedia = "DimeRecord_TypeFormatShouldBeMedia";

			public const string DimeRecord_TypeFormatShouldBeUnchanged = "DimeRecord_TypeFormatShouldBeUnchanged";

			public const string DimeRecord_ReservedFlagShouldBeZero = "DimeRecord_ReservedFlagShouldBeZero";

			public const string DimeRecord_DataTypeShouldBeSpecifiedOnTheFirstChunk = "DimeRecord_DataTypeShouldBeSpecifiedOnTheFirstChunk";

			public const string DimeRecord_DataTypeIsOnlyForTheFirstChunk = "DimeRecord_DataTypeIsOnlyForTheFirstChunk";

			public const string DimeRecord_DataTypeNotSupported = "DimeRecord_DataTypeNotSupported";

			public const string DimeRecord_InvalidHeaderFlags = "DimeRecord_InvalidHeaderFlags";

			public const string DimeRecord_IDIsOnlyForFirstChunk = "DimeRecord_IDIsOnlyForFirstChunk";

			public const string DimeWriter_CannotWriteToStream = "DimeWriter_CannotWriteToStream";

			public const string DimeWriter_WriterIsClosed = "DimeWriter_WriterIsClosed";

			public const string DimeWriter_InvalidDefaultChunkSize = "DimeWriter_InvalidDefaultChunkSize";

			public const string Dime_DataTypeNotSupported = "Dime_DataTypeNotSupported";

			public const string TcpStream_MaxSignatureExceedsProtocolLimit = "TcpStream_MaxSignatureExceedsProtocolLimit";

			public const string IXMLAInterop_OnlyZeroOffsetIsSupported = "IXMLAInterop_OnlyZeroOffsetIsSupported";

			public const string IXMLAInterop_StreamDoesNotSupportReverting = "IXMLAInterop_StreamDoesNotSupportReverting";

			public const string IXMLAInterop_StreamDoesNotSupportLocking = "IXMLAInterop_StreamDoesNotSupportLocking";

			public const string IXMLAInterop_StreamDoesNotSupportUnlocking = "IXMLAInterop_StreamDoesNotSupportUnlocking";

			public const string IXMLAInterop_StreamCannotBeCloned = "IXMLAInterop_StreamCannotBeCloned";

			public const string XmlaClient_StartRequest_ThereIsAnotherPendingRequest = "XmlaClient_StartRequest_ThereIsAnotherPendingRequest";

			public const string XmlaClient_StartRequest_ThereIsAnotherPendingResponse = "XmlaClient_StartRequest_ThereIsAnotherPendingResponse";

			public const string XmlaClient_SendRequest_RequestStreamCannotBeRead = "XmlaClient_SendRequest_RequestStreamCannotBeRead";

			public const string XmlaClient_SendRequest_NoRequestWasCreated = "XmlaClient_SendRequest_NoRequestWasCreated";

			public const string XmlaClient_ConnectTimedOut = "XmlaClient_ConnectTimedOut";

			public const string XmlaClient_SendRequest_ThereIsAnotherPendingResponse = "XmlaClient_SendRequest_ThereIsAnotherPendingResponse";

			public const string XmlaClient_CannotConnectToLocalCubeWithRestictedClient = "XmlaClient_CannotConnectToLocalCubeWithRestictedClient";

			public const string Decompression_InitializationFailed = "Decompression_InitializationFailed";

			public const string Decompression_Failed = "Decompression_Failed";

			public const string Compression_InitializationFailed = "Compression_InitializationFailed";

			public const string InvalidArgument = "InvalidArgument";

			public const string UnsupportedDataFormat = "UnsupportedDataFormat";

			public const string UnsupportedMethod = "UnsupportedMethod";

			public const string ProvidePath = "ProvidePath";

			public const string DirectoryNotFound = "DirectoryNotFound";

			public const string InternalError = "InternalError";

			public const string InternalErrorAndInvalidBufferType = "InternalErrorAndInvalidBufferType";

			public const string HttpStream_ASAzure_TechnicalDetailsText = "HttpStream_ASAzure_TechnicalDetailsText";

			private static ResourceManager resourceManager = new ResourceManager("AdomdClientNetCore.Microsoft.AnalysisServices.AdomdClient.XmlaSR", typeof(XmlaSR).Module.Assembly);

			private static CultureInfo _culture = null;

			public static CultureInfo Culture
			{
				get
				{
					return XmlaSR.Keys._culture;
				}
				set
				{
					XmlaSR.Keys._culture = value;
				}
			}

			private Keys()
			{
			}

			public static string GetString(string key)
			{			    
				return XmlaSR.Keys.resourceManager.GetString(key, XmlaSR.Keys._culture);
			}

			public static string GetString(string key, object arg0)
			{			    
				return string.Format(CultureInfo.CurrentCulture, XmlaSR.Keys.resourceManager.GetString(key, XmlaSR.Keys._culture), new object[]
				{
					arg0
				});
			}

			public static string GetString(string key, object arg0, object arg1)
			{			    
				return string.Format(CultureInfo.CurrentCulture, XmlaSR.Keys.resourceManager.GetString(key, XmlaSR.Keys._culture), new object[]
				{
					arg0,
					arg1
				});
			}

			public static string GetString(string key, object arg0, object arg1, object arg2)
			{                
				return string.Format(CultureInfo.CurrentCulture, XmlaSR.Keys.resourceManager.GetString(key, XmlaSR.Keys._culture), new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
		}

		public static CultureInfo Culture
		{
			get
			{
				return XmlaSR.Keys.Culture;
			}
			set
			{
				XmlaSR.Keys.Culture = value;
			}
		}

		public static string AlreadyConnected
		{
			get
			{
				return XmlaSR.Keys.GetString("AlreadyConnected");
			}
		}

		public static string NotConnected
		{
			get
			{
				return XmlaSR.Keys.GetString("NotConnected");
			}
		}

		public static string CannotConnect
		{
			get
			{
				return XmlaSR.Keys.GetString("CannotConnect");
			}
		}

		public static string CannotConnectToRedirector
		{
			get
			{
				return XmlaSR.Keys.GetString("CannotConnectToRedirector");
			}
		}

		public static string ConnectionBroken
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionBroken");
			}
		}

		public static string Reconnect_ConnectionInfoIsMissing
		{
			get
			{
				return XmlaSR.Keys.GetString("Reconnect_ConnectionInfoIsMissing");
			}
		}

		public static string Reconnect_SessionIDIsMissing
		{
			get
			{
				return XmlaSR.Keys.GetString("Reconnect_SessionIDIsMissing");
			}
		}

		public static string ServerDidNotProvideErrorInfo
		{
			get
			{
				return XmlaSR.Keys.GetString("ServerDidNotProvideErrorInfo");
			}
		}

		public static string ConnectionCannotBeUsedWhileXmlReaderOpened
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionCannotBeUsedWhileXmlReaderOpened");
			}
		}

		public static string Connect_RedirectorDidntReturnDatabaseInfo
		{
			get
			{
				return XmlaSR.Keys.GetString("Connect_RedirectorDidntReturnDatabaseInfo");
			}
		}

		public static string Connection_WorkbookIsOutdated
		{
			get
			{
				return XmlaSR.Keys.GetString("Connection_WorkbookIsOutdated");
			}
		}

		public static string Connection_AnalysisServicesInstanceWasMoved
		{
			get
			{
				return XmlaSR.Keys.GetString("Connection_AnalysisServicesInstanceWasMoved");
			}
		}

		public static string SoapFormatter_ResponseIsNotRowset
		{
			get
			{
				return XmlaSR.Keys.GetString("SoapFormatter_ResponseIsNotRowset");
			}
		}

		public static string SoapFormatter_ResponseIsNotDataset
		{
			get
			{
				return XmlaSR.Keys.GetString("SoapFormatter_ResponseIsNotDataset");
			}
		}

		public static string Cancel_SessionIDNotSpecified
		{
			get
			{
				return XmlaSR.Keys.GetString("Cancel_SessionIDNotSpecified");
			}
		}

		public static string ConnectionString_Invalid
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_Invalid");
			}
		}

		public static string ConnectionString_DataSourceNotSpecified
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_DataSourceNotSpecified");
			}
		}

		public static string ConnectionString_MissingIdentityProviderForIntegratedSecurityFederated
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_MissingIdentityProviderForIntegratedSecurityFederated");
			}
		}

		public static string ConnectionString_InvalidIdentityProviderForIntegratedSecurityFederated
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_InvalidIdentityProviderForIntegratedSecurityFederated");
			}
		}

		public static string ConnectionString_DataSourceTypeDoesntSupportQuery
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_DataSourceTypeDoesntSupportQuery");
			}
		}

		public static string ConnectionString_LinkFileInvalidServer
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_LinkFileInvalidServer");
			}
		}

		public static string ConnectionString_LinkFileDupEffectiveUsername
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_LinkFileDupEffectiveUsername");
			}
		}

		public static string ConnectionString_LinkFileCannotRevert
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_LinkFileCannotRevert");
			}
		}

		public static string ConnectionString_LinkFileCannotDelegate
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_LinkFileCannotDelegate");
			}
		}

		public static string ConnectionString_MissingPassword
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_MissingPassword");
			}
		}

		public static string ConnectionString_AsAzure_DataSourcePathMoreThanOneSegment
		{
			get
			{
				return XmlaSR.Keys.GetString("ConnectionString_AsAzure_DataSourcePathMoreThanOneSegment");
			}
		}

		public static string UnknownServerResponseFormat
		{
			get
			{
				return XmlaSR.Keys.GetString("UnknownServerResponseFormat");
			}
		}

		public static string AfterExceptionAllTagsShouldCloseUntilMessagesSection
		{
			get
			{
				return XmlaSR.Keys.GetString("AfterExceptionAllTagsShouldCloseUntilMessagesSection");
			}
		}

		public static string ErrorCodeIsMissingFromRowsetError
		{
			get
			{
				return XmlaSR.Keys.GetString("ErrorCodeIsMissingFromRowsetError");
			}
		}

		public static string ErrorCodeIsMissingFromDatasetError
		{
			get
			{
				return XmlaSR.Keys.GetString("ErrorCodeIsMissingFromDatasetError");
			}
		}

		public static string ExceptionRequiresXmlaErrorsInMessagesSection
		{
			get
			{
				return XmlaSR.Keys.GetString("ExceptionRequiresXmlaErrorsInMessagesSection");
			}
		}

		public static string MessagesSectionIsEmpty
		{
			get
			{
				return XmlaSR.Keys.GetString("MessagesSectionIsEmpty");
			}
		}

		public static string EmptyRootIsNotEmpty
		{
			get
			{
				return XmlaSR.Keys.GetString("EmptyRootIsNotEmpty");
			}
		}

		public static string Resultset_IsNotRowset
		{
			get
			{
				return XmlaSR.Keys.GetString("Resultset_IsNotRowset");
			}
		}

		public static string DataReaderClosedError
		{
			get
			{
				return XmlaSR.Keys.GetString("DataReaderClosedError");
			}
		}

		public static string DataReaderInvalidRowError
		{
			get
			{
				return XmlaSR.Keys.GetString("DataReaderInvalidRowError");
			}
		}

		public static string NonSequentialColumnAccessError
		{
			get
			{
				return XmlaSR.Keys.GetString("NonSequentialColumnAccessError");
			}
		}

		public static string DataReader_IndexOutOfRange
		{
			get
			{
				return XmlaSR.Keys.GetString("DataReader_IndexOutOfRange");
			}
		}

		public static string Authentication_Failed
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_Failed");
			}
		}

		public static string Authentication_Sspi_SchannelCantDelegate
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_Sspi_SchannelCantDelegate");
			}
		}

		public static string Authentication_Sspi_SchannelSupportsOnlyPrivacyLevel
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_Sspi_SchannelSupportsOnlyPrivacyLevel");
			}
		}

		public static string Authentication_Sspi_SchannelUnsupportedImpersonationLevel
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_Sspi_SchannelUnsupportedImpersonationLevel");
			}
		}

		public static string Authentication_Sspi_SchannelUnsupportedProtectionLevel
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_Sspi_SchannelUnsupportedProtectionLevel");
			}
		}

		public static string Authentication_Sspi_SchannelAnonymousAmbiguity
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_Sspi_SchannelAnonymousAmbiguity");
			}
		}

		public static string Authentication_MsoID_MissingSignInAssistant
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_MsoID_MissingSignInAssistant");
			}
		}

		public static string Authentication_MsoID_InternalError
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_MsoID_InternalError");
			}
		}

		public static string Authentication_MsoID_InvalidCredentials
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_MsoID_InvalidCredentials");
			}
		}

		public static string Authentication_MsoID_SsoFailed
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_MsoID_SsoFailed");
			}
		}

		public static string Authentication_MsoID_SsoFailedNonDomainUser
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_MsoID_SsoFailedNonDomainUser");
			}
		}

		public static string Authentication_ClaimsToken_AuthorityNotFound
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_ClaimsToken_AuthorityNotFound");
			}
		}

		public static string Authentication_ClaimsToken_UserIdAndPasswordRequired
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_ClaimsToken_UserIdAndPasswordRequired");
			}
		}

		public static string Authentication_ClaimsToken_IdentityProviderFormatInvalid
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_ClaimsToken_IdentityProviderFormatInvalid");
			}
		}

		public static string Authentication_AsAzure_OnlyClaimsTokenSupported
		{
			get
			{
				return XmlaSR.Keys.GetString("Authentication_AsAzure_OnlyClaimsTokenSupported");
			}
		}

		public static string DimeReader_CannotReadFromStream
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeReader_CannotReadFromStream");
			}
		}

		public static string DimeReader_IsClosed
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeReader_IsClosed");
			}
		}

		public static string DimeReader_PreviousRecordStreamStillOpened
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeReader_PreviousRecordStreamStillOpened");
			}
		}

		public static string DimeRecord_StreamShouldBeReadable
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_StreamShouldBeReadable");
			}
		}

		public static string DimeRecord_StreamShouldBeWriteable
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_StreamShouldBeWriteable");
			}
		}

		public static string DimeRecord_InvalidContentLength
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_InvalidContentLength");
			}
		}

		public static string DimeRecord_PropertyOnlyAvailableForReadRecords
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_PropertyOnlyAvailableForReadRecords");
			}
		}

		public static string DimeRecord_InvalidChunkSize
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_InvalidChunkSize");
			}
		}

		public static string DimeRecord_UnableToReadFromStream
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_UnableToReadFromStream");
			}
		}

		public static string DimeRecord_StreamIsClosed
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_StreamIsClosed");
			}
		}

		public static string DimeRecord_ReadNotAllowed
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_ReadNotAllowed");
			}
		}

		public static string DimeRecord_WriteNotAllowed
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_WriteNotAllowed");
			}
		}

		public static string DimeRecord_TypeFormatEnumUnchangedNotAllowed
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_TypeFormatEnumUnchangedNotAllowed");
			}
		}

		public static string DimeRecord_MediaTypeNotDefined
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_MediaTypeNotDefined");
			}
		}

		public static string DimeRecord_NameMustNotBeDefinedForFormatNone
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_NameMustNotBeDefinedForFormatNone");
			}
		}

		public static string DimeRecord_EncodedTypeLengthExceeds8191
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_EncodedTypeLengthExceeds8191");
			}
		}

		public static string DimeRecord_OffsetAndCountShouldBePositive
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_OffsetAndCountShouldBePositive");
			}
		}

		public static string DimeRecord_ContentLengthExceeded
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_ContentLengthExceeded");
			}
		}

		public static string DimeRecord_OnlySingleRecordMessagesAreSupported
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_OnlySingleRecordMessagesAreSupported");
			}
		}

		public static string DimeRecord_DataTypeShouldBeSpecifiedOnTheFirstChunk
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_DataTypeShouldBeSpecifiedOnTheFirstChunk");
			}
		}

		public static string DimeRecord_DataTypeIsOnlyForTheFirstChunk
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_DataTypeIsOnlyForTheFirstChunk");
			}
		}

		public static string DimeRecord_IDIsOnlyForFirstChunk
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeRecord_IDIsOnlyForFirstChunk");
			}
		}

		public static string DimeWriter_CannotWriteToStream
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeWriter_CannotWriteToStream");
			}
		}

		public static string DimeWriter_WriterIsClosed
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeWriter_WriterIsClosed");
			}
		}

		public static string DimeWriter_InvalidDefaultChunkSize
		{
			get
			{
				return XmlaSR.Keys.GetString("DimeWriter_InvalidDefaultChunkSize");
			}
		}

		public static string TcpStream_MaxSignatureExceedsProtocolLimit
		{
			get
			{
				return XmlaSR.Keys.GetString("TcpStream_MaxSignatureExceedsProtocolLimit");
			}
		}

		public static string IXMLAInterop_OnlyZeroOffsetIsSupported
		{
			get
			{
				return XmlaSR.Keys.GetString("IXMLAInterop_OnlyZeroOffsetIsSupported");
			}
		}

		public static string IXMLAInterop_StreamDoesNotSupportReverting
		{
			get
			{
				return XmlaSR.Keys.GetString("IXMLAInterop_StreamDoesNotSupportReverting");
			}
		}

		public static string IXMLAInterop_StreamDoesNotSupportLocking
		{
			get
			{
				return XmlaSR.Keys.GetString("IXMLAInterop_StreamDoesNotSupportLocking");
			}
		}

		public static string IXMLAInterop_StreamDoesNotSupportUnlocking
		{
			get
			{
				return XmlaSR.Keys.GetString("IXMLAInterop_StreamDoesNotSupportUnlocking");
			}
		}

		public static string IXMLAInterop_StreamCannotBeCloned
		{
			get
			{
				return XmlaSR.Keys.GetString("IXMLAInterop_StreamCannotBeCloned");
			}
		}

		public static string XmlaClient_StartRequest_ThereIsAnotherPendingRequest
		{
			get
			{
				return XmlaSR.Keys.GetString("XmlaClient_StartRequest_ThereIsAnotherPendingRequest");
			}
		}

		public static string XmlaClient_StartRequest_ThereIsAnotherPendingResponse
		{
			get
			{
				return XmlaSR.Keys.GetString("XmlaClient_StartRequest_ThereIsAnotherPendingResponse");
			}
		}

		public static string XmlaClient_SendRequest_RequestStreamCannotBeRead
		{
			get
			{
				return XmlaSR.Keys.GetString("XmlaClient_SendRequest_RequestStreamCannotBeRead");
			}
		}

		public static string XmlaClient_SendRequest_NoRequestWasCreated
		{
			get
			{
				return XmlaSR.Keys.GetString("XmlaClient_SendRequest_NoRequestWasCreated");
			}
		}

		public static string XmlaClient_ConnectTimedOut
		{
			get
			{
				return XmlaSR.Keys.GetString("XmlaClient_ConnectTimedOut");
			}
		}

		public static string XmlaClient_SendRequest_ThereIsAnotherPendingResponse
		{
			get
			{
				return XmlaSR.Keys.GetString("XmlaClient_SendRequest_ThereIsAnotherPendingResponse");
			}
		}

		public static string XmlaClient_CannotConnectToLocalCubeWithRestictedClient
		{
			get
			{
				return XmlaSR.Keys.GetString("XmlaClient_CannotConnectToLocalCubeWithRestictedClient");
			}
		}

		public static string Decompression_InitializationFailed
		{
			get
			{
				return XmlaSR.Keys.GetString("Decompression_InitializationFailed");
			}
		}

		public static string Compression_InitializationFailed
		{
			get
			{
				return XmlaSR.Keys.GetString("Compression_InitializationFailed");
			}
		}

		public static string InvalidArgument
		{
			get
			{
				return XmlaSR.Keys.GetString("InvalidArgument");
			}
		}

		public static string ProvidePath
		{
			get
			{
				return XmlaSR.Keys.GetString("ProvidePath");
			}
		}

		public static string InternalError
		{
			get
			{
				return XmlaSR.Keys.GetString("InternalError");
			}
		}

		public static string InternalErrorAndInvalidBufferType
		{
			get
			{
				return XmlaSR.Keys.GetString("InternalErrorAndInvalidBufferType");
			}
		}

		protected XmlaSR()
		{
		}

		public static string LocalCube_FileNotOpened(string cubeFile)
		{
			return XmlaSR.Keys.GetString("LocalCube_FileNotOpened", cubeFile);
		}

		public static string Instance_NotFound(string instance, string server)
		{
			return XmlaSR.Keys.GetString("Instance_NotFound", instance, server);
		}

		public static string UnexpectedXsiType(string type)
		{
			return XmlaSR.Keys.GetString("UnexpectedXsiType", type);
		}

		public static string ConnectionString_InvalidPropertyNameFormat(string propertyName)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidPropertyNameFormat", propertyName);
		}

		public static string ConnectionString_UnsupportedPropertyValue(string propertyName, string value)
		{
			return XmlaSR.Keys.GetString("ConnectionString_UnsupportedPropertyValue", propertyName, value);
		}

		public static string ConnectionString_OpenedQuoteIsNotClosed(char openQuoteChar, int index)
		{
			return XmlaSR.Keys.GetString("ConnectionString_OpenedQuoteIsNotClosed", openQuoteChar, index);
		}

		public static string ConnectionString_ExpectedSemicolonNotFound(int index)
		{
			return XmlaSR.Keys.GetString("ConnectionString_ExpectedSemicolonNotFound", index);
		}

		public static string ConnectionString_ExpectedEqualSignNotFound(int fromIndex)
		{
			return XmlaSR.Keys.GetString("ConnectionString_ExpectedEqualSignNotFound", fromIndex);
		}

		public static string ConnectionString_InvalidCharInPropertyName(char invalidChar, int index)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidCharInPropertyName", invalidChar, index);
		}

		public static string ConnectionString_InvalidCharInUnquotedPropertyValue(char invalidChar, int index)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidCharInUnquotedPropertyValue", invalidChar, index);
		}

		public static string ConnectionString_PropertyNameNotDefined(int equalIndex)
		{
			return XmlaSR.Keys.GetString("ConnectionString_PropertyNameNotDefined", equalIndex);
		}

		public static string ConnectionString_InvalidIntegratedSecurityForNative(string integratedSecurity)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidIntegratedSecurityForNative", integratedSecurity);
		}

		public static string ConnectionString_InvalidProtectionLevelForHttp(string protectionLevel)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidProtectionLevelForHttp", protectionLevel);
		}

		public static string ConnectionString_InvalidProtectionLevelForHttps(string protectionLevel)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidProtectionLevelForHttps", protectionLevel);
		}

		public static string ConnectionString_InvalidImpersonationLevelForHttp(string impersonationLevel)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidImpersonationLevelForHttp", impersonationLevel);
		}

		public static string ConnectionString_InvalidImpersonationLevelForHttps(string impersonationLevel)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidImpersonationLevelForHttps", impersonationLevel);
		}

		public static string ConnectionString_InvalidIntegratedSecurityForHttpOrHttps(string integratedSecurity)
		{
			return XmlaSR.Keys.GetString("ConnectionString_InvalidIntegratedSecurityForHttpOrHttps", integratedSecurity);
		}

		public static string ConnectionString_PropertyNotApplicableWithTheDataSourceType(string propertyName)
		{
			return XmlaSR.Keys.GetString("ConnectionString_PropertyNotApplicableWithTheDataSourceType", propertyName);
		}

		public static string ConnectionString_LinkFileParseError(int size)
		{
			return XmlaSR.Keys.GetString("ConnectionString_LinkFileParseError", size);
		}

		public static string ConnectionString_LinkFileDownloadError(string linkFileName)
		{
			return XmlaSR.Keys.GetString("ConnectionString_LinkFileDownloadError", linkFileName);
		}

		public static string ConnectionString_ExternalConnectionIsIncomplete(string missingPropertyName)
		{
			return XmlaSR.Keys.GetString("ConnectionString_ExternalConnectionIsIncomplete", missingPropertyName);
		}

		public static string UnrecognizedElementInMessagesSection(string elementName)
		{
			return XmlaSR.Keys.GetString("UnrecognizedElementInMessagesSection", elementName);
		}

		public static string UnexpectedElement(string elementName, string namespaceName)
		{
			return XmlaSR.Keys.GetString("UnexpectedElement", elementName, namespaceName);
		}

		public static string MissingElement(string elementName, string namespaceName)
		{
			return XmlaSR.Keys.GetString("MissingElement", elementName, namespaceName);
		}

		public static string Authentication_Sspi_PackageNotFound(string packageName)
		{
			return XmlaSR.Keys.GetString("Authentication_Sspi_PackageNotFound", packageName);
		}

		public static string Authentication_Sspi_PackageDoesntSupportCapability(string package, string capability)
		{
			return XmlaSR.Keys.GetString("Authentication_Sspi_PackageDoesntSupportCapability", package, capability);
		}

		public static string Authentication_Sspi_FlagNotEstablished(string flagName)
		{
			return XmlaSR.Keys.GetString("Authentication_Sspi_FlagNotEstablished", flagName);
		}

		public static string Authentication_ClaimsToken_AdalLoadingError(string component)
		{
			return XmlaSR.Keys.GetString("Authentication_ClaimsToken_AdalLoadingError", component);
		}

		public static string Authentication_ClaimsToken_AdalError(string message)
		{
			return XmlaSR.Keys.GetString("Authentication_ClaimsToken_AdalError", message);
		}

		public static string DimeRecord_InvalidUriFormat(string uri)
		{
			return XmlaSR.Keys.GetString("DimeRecord_InvalidUriFormat", uri);
		}

		public static string DimeRecord_VersionNotSupported(int version)
		{
			return XmlaSR.Keys.GetString("DimeRecord_VersionNotSupported", version);
		}

		public static string DimeRecord_TypeFormatShouldBeMedia(string value)
		{
			return XmlaSR.Keys.GetString("DimeRecord_TypeFormatShouldBeMedia", value);
		}

		public static string DimeRecord_TypeFormatShouldBeUnchanged(string value)
		{
			return XmlaSR.Keys.GetString("DimeRecord_TypeFormatShouldBeUnchanged", value);
		}

		public static string DimeRecord_ReservedFlagShouldBeZero(byte value)
		{
			return XmlaSR.Keys.GetString("DimeRecord_ReservedFlagShouldBeZero", value);
		}

		public static string DimeRecord_DataTypeNotSupported(string value)
		{
			return XmlaSR.Keys.GetString("DimeRecord_DataTypeNotSupported", value);
		}

		public static string DimeRecord_InvalidHeaderFlags(int begin, int end, int chunked)
		{
			return XmlaSR.Keys.GetString("DimeRecord_InvalidHeaderFlags", begin, end, chunked);
		}

		public static string Dime_DataTypeNotSupported(string value)
		{
			return XmlaSR.Keys.GetString("Dime_DataTypeNotSupported", value);
		}

		public static string Decompression_Failed(int compressedSize, int expectedDecompressedSize, int actualDecompressedSize)
		{
			return XmlaSR.Keys.GetString("Decompression_Failed", compressedSize, expectedDecompressedSize, actualDecompressedSize);
		}

		public static string UnsupportedDataFormat(string format)
		{
			return XmlaSR.Keys.GetString("UnsupportedDataFormat", format);
		}

		public static string UnsupportedMethod(string name)
		{
			return XmlaSR.Keys.GetString("UnsupportedMethod", name);
		}

		public static string DirectoryNotFound(string path)
		{
			return XmlaSR.Keys.GetString("DirectoryNotFound", path);
		}

		public static string HttpStream_ASAzure_TechnicalDetailsText(string rootActivityId, string currentUtcDate)
		{
			return XmlaSR.Keys.GetString("HttpStream_ASAzure_TechnicalDetailsText", rootActivityId, currentUtcDate);
		}
	}
}
