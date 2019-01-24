using System;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[CompilerGenerated]
	internal class SR
	{
		[CompilerGenerated]
		public class Keys
		{
			public const string Server_IsAlreadyConnected = "Server_IsAlreadyConnected";

			public const string Server_IsNotConnected = "Server_IsNotConnected";

			public const string Server_NoServerName = "Server_NoServerName";

			public const string Connection_SessionID_SessionIsAlreadyOpen = "Connection_SessionID_SessionIsAlreadyOpen";

			public const string Connection_ShowHiddenObjects_ConnectionAlreadyOpen = "Connection_ShowHiddenObjects_ConnectionAlreadyOpen";

			public const string Connection_ConnectionString_NotInitialized = "Connection_ConnectionString_NotInitialized";

			public const string Connection_ConnectionToLocalServerNotSupported = "Connection_ConnectionToLocalServerNotSupported";

			public const string Command_ConnectionIsNotSet = "Command_ConnectionIsNotSet";

			public const string Command_ConnectionIsNotOpened = "Command_ConnectionIsNotOpened";

			public const string Command_InvalidTimeout = "Command_InvalidTimeout";

			public const string Cellset_propertyIsUnknown = "Cellset_propertyIsUnknown";

			public const string Resultset_IsNotDataset = "Resultset_IsNotDataset";

			public const string DatasetResponse_HierarchyWithSameNameOnSameAxis = "DatasetResponse_HierarchyWithSameNameOnSameAxis";

			public const string Schema_InvalidGuid = "Schema_InvalidGuid";

			public const string Schema_RestOutOfRange = "Schema_RestOutOfRange";

			public const string Schema_UnexpectedResponseForSchema = "Schema_UnexpectedResponseForSchema";

			public const string Schema_PropertyIsMissingOrOfAnUnexpectedType = "Schema_PropertyIsMissingOrOfAnUnexpectedType";

			public const string Connection_InvalidProperty = "Connection_InvalidProperty";

			public const string Parameter_Parent_Mismatch = "Parameter_Parent_Mismatch";

			public const string Parameter_Already_Exists = "Parameter_Already_Exists";

			public const string Parameter_Value_Wrong_Type = "Parameter_Value_Wrong_Type";

			public const string Property_Parent_Mismatch = "Property_Parent_Mismatch";

			public const string Property_Already_Exists = "Property_Already_Exists";

			public const string Property_Value_Wrong_Type = "Property_Value_Wrong_Type";

			public const string Property_DoesNotExist = "Property_DoesNotExist";

			public const string Property_Key_Wrong_Type = "Property_Key_Wrong_Type";

			public const string Member_MissingDisplayInfo = "Member_MissingDisplayInfo";

			public const string Member_MissingLevelName = "Member_MissingLevelName";

			public const string Member_MissingLevelDepth = "Member_MissingLevelDepth";

			public const string Indexer_ObjectNotFound = "Indexer_ObjectNotFound";

			public const string Metadata_CubeHasbeenUpdated = "Metadata_CubeHasbeenUpdated";

			public const string Metadata_CubesCollectionHasbeenUpdated = "Metadata_CubesCollectionHasbeenUpdated";

			public const string Metadata_MiningServicesCollectionHasbeenUpdated = "Metadata_MiningServicesCollectionHasbeenUpdated";

			public const string NotSupportedByProvider = "NotSupportedByProvider";

			public const string NotSupportedWhenConnectionMissing = "NotSupportedWhenConnectionMissing";

			public const string NotSupportedOnNonCellsetMember = "NotSupportedOnNonCellsetMember";

			public const string Property_UnknownProperty = "Property_UnknownProperty";

			public const string CellIndexer_InvalidNumberOfAxesIndexers = "CellIndexer_InvalidNumberOfAxesIndexers";

			public const string CellIndexer_IndexOutOfRange = "CellIndexer_IndexOutOfRange";

			public const string CellIndexer_InvalidIndexType = "CellIndexer_InvalidIndexType";

			public const string CellSet_InvalidStateOfReader = "CellSet_InvalidStateOfReader";

			public const string Command_CommandStreamDoesNotSupportReadingFrom = "Command_CommandStreamDoesNotSupportReadingFrom";

			public const string Command_CommandTextCommandStreamNotSet = "Command_CommandTextCommandStreamNotSet";

			public const string Command_CommandTextCommandStreamBothSet = "Command_CommandTextCommandStreamBothSet";

			public const string Connection_DatabaseNameEmpty = "Connection_DatabaseNameEmpty";

			public const string Connection_NoInformationAboutDataSourcesReturned = "Connection_NoInformationAboutDataSourcesReturned";

			public const string Connection_PropertyNameEmpty = "Connection_PropertyNameEmpty";

			public const string Connection_FailedToSetProperty = "Connection_FailedToSetProperty";

			public const string Property_PropertyNotFound = "Property_PropertyNotFound";

			public const string Restrictions_TypesMismatch = "Restrictions_TypesMismatch";

			public const string ICollection_CannotCopyToMultidimensionalArray = "ICollection_CannotCopyToMultidimensionalArray";

			public const string ICollection_NotEnoughSpaceToCopyTo = "ICollection_NotEnoughSpaceToCopyTo";

			public const string ICollection_ItemWithThisNameDoesNotExistInTheCollection = "ICollection_ItemWithThisNameDoesNotExistInTheCollection";

			public const string Connection_EffectiveUserNameEmpty = "Connection_EffectiveUserNameEmpty";

			public const string TransactionAlreadyComplete = "TransactionAlreadyComplete";

			public const string Command_OnlyAdomdTransactionObjectIsSupported = "Command_OnlyAdomdTransactionObjectIsSupported";

			public const string Command_OnlyActiveTransactionCanBeAssigned = "Command_OnlyActiveTransactionCanBeAssigned";

			public const string Command_OnlyTransactionAssociatedWithTheSameConnectionCanBeAssigned = "Command_OnlyTransactionAssociatedWithTheSameConnectionCanBeAssigned";

			public const string InvalidOperationPriorToFetchAllProperties = "InvalidOperationPriorToFetchAllProperties";

			public const string MetadataCache_Abandoned = "MetadataCache_Abandoned";

			public const string ArgumentErrorUniqueNameEmpty = "ArgumentErrorUniqueNameEmpty";

			public const string ArgumentErrorInvalidSchemaObjectType = "ArgumentErrorInvalidSchemaObjectType";

			public const string ArgumentErrorUnsupportedParameterType = "ArgumentErrorUnsupportedParameterType";

			public const string InvalidArgument = "InvalidArgument";

			private static ResourceManager resourceManager = new ResourceManager(typeof(SR).FullName, typeof(SR).Module.Assembly);

			private static CultureInfo _culture = null;

			public static CultureInfo Culture
			{
				get
				{
					return SR.Keys._culture;
				}
				set
				{
					SR.Keys._culture = value;
				}
			}

			private Keys()
			{
			}

			public static string GetString(string key)
			{
                return SR.Keys.resourceManager.GetString(key, SR.Keys._culture);
			}

			public static string GetString(string key, object arg0)
			{
			    return string.Format(CultureInfo.CurrentCulture, SR.Keys.resourceManager.GetString(key, SR.Keys._culture), new object[]
				{
					arg0
				});
			}

			public static string GetString(string key, object arg0, object arg1)
			{
			    return string.Format(CultureInfo.CurrentCulture, SR.Keys.resourceManager.GetString(key, SR.Keys._culture), new object[]
				{
					arg0,
					arg1
				});
			}

			public static string GetString(string key, object arg0, object arg1, object arg2)
			{
			    return string.Format(CultureInfo.CurrentCulture, SR.Keys.resourceManager.GetString(key, SR.Keys._culture), new object[]
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
				return SR.Keys.Culture;
			}
			set
			{
				SR.Keys.Culture = value;
			}
		}

		public static string Server_IsAlreadyConnected
		{
			get
			{
				return SR.Keys.GetString("Server_IsAlreadyConnected");
			}
		}

		public static string Server_IsNotConnected
		{
			get
			{
				return SR.Keys.GetString("Server_IsNotConnected");
			}
		}

		public static string Server_NoServerName
		{
			get
			{
				return SR.Keys.GetString("Server_NoServerName");
			}
		}

		public static string Connection_SessionID_SessionIsAlreadyOpen
		{
			get
			{
				return SR.Keys.GetString("Connection_SessionID_SessionIsAlreadyOpen");
			}
		}

		public static string Connection_ShowHiddenObjects_ConnectionAlreadyOpen
		{
			get
			{
				return SR.Keys.GetString("Connection_ShowHiddenObjects_ConnectionAlreadyOpen");
			}
		}

		public static string Connection_ConnectionString_NotInitialized
		{
			get
			{
				return SR.Keys.GetString("Connection_ConnectionString_NotInitialized");
			}
		}

		public static string Connection_ConnectionToLocalServerNotSupported
		{
			get
			{
				return SR.Keys.GetString("Connection_ConnectionToLocalServerNotSupported");
			}
		}

		public static string Command_ConnectionIsNotSet
		{
			get
			{
				return SR.Keys.GetString("Command_ConnectionIsNotSet");
			}
		}

		public static string Command_ConnectionIsNotOpened
		{
			get
			{
				return SR.Keys.GetString("Command_ConnectionIsNotOpened");
			}
		}

		public static string Resultset_IsNotDataset
		{
			get
			{
				return SR.Keys.GetString("Resultset_IsNotDataset");
			}
		}

		public static string Schema_InvalidGuid
		{
			get
			{
				return SR.Keys.GetString("Schema_InvalidGuid");
			}
		}

		public static string Schema_RestOutOfRange
		{
			get
			{
				return SR.Keys.GetString("Schema_RestOutOfRange");
			}
		}

		public static string Parameter_Parent_Mismatch
		{
			get
			{
				return SR.Keys.GetString("Parameter_Parent_Mismatch");
			}
		}

		public static string Parameter_Value_Wrong_Type
		{
			get
			{
				return SR.Keys.GetString("Parameter_Value_Wrong_Type");
			}
		}

		public static string Property_Parent_Mismatch
		{
			get
			{
				return SR.Keys.GetString("Property_Parent_Mismatch");
			}
		}

		public static string Property_Already_Exists
		{
			get
			{
				return SR.Keys.GetString("Property_Already_Exists");
			}
		}

		public static string Property_Value_Wrong_Type
		{
			get
			{
				return SR.Keys.GetString("Property_Value_Wrong_Type");
			}
		}

		public static string Property_DoesNotExist
		{
			get
			{
				return SR.Keys.GetString("Property_DoesNotExist");
			}
		}

		public static string Property_Key_Wrong_Type
		{
			get
			{
				return SR.Keys.GetString("Property_Key_Wrong_Type");
			}
		}

		public static string Member_MissingDisplayInfo
		{
			get
			{
				return SR.Keys.GetString("Member_MissingDisplayInfo");
			}
		}

		public static string Member_MissingLevelName
		{
			get
			{
				return SR.Keys.GetString("Member_MissingLevelName");
			}
		}

		public static string Member_MissingLevelDepth
		{
			get
			{
				return SR.Keys.GetString("Member_MissingLevelDepth");
			}
		}

		public static string Metadata_CubesCollectionHasbeenUpdated
		{
			get
			{
				return SR.Keys.GetString("Metadata_CubesCollectionHasbeenUpdated");
			}
		}

		public static string Metadata_MiningServicesCollectionHasbeenUpdated
		{
			get
			{
				return SR.Keys.GetString("Metadata_MiningServicesCollectionHasbeenUpdated");
			}
		}

		public static string NotSupportedByProvider
		{
			get
			{
				return SR.Keys.GetString("NotSupportedByProvider");
			}
		}

		public static string NotSupportedWhenConnectionMissing
		{
			get
			{
				return SR.Keys.GetString("NotSupportedWhenConnectionMissing");
			}
		}

		public static string NotSupportedOnNonCellsetMember
		{
			get
			{
				return SR.Keys.GetString("NotSupportedOnNonCellsetMember");
			}
		}

		public static string Command_CommandStreamDoesNotSupportReadingFrom
		{
			get
			{
				return SR.Keys.GetString("Command_CommandStreamDoesNotSupportReadingFrom");
			}
		}

		public static string Command_CommandTextCommandStreamNotSet
		{
			get
			{
				return SR.Keys.GetString("Command_CommandTextCommandStreamNotSet");
			}
		}

		public static string Command_CommandTextCommandStreamBothSet
		{
			get
			{
				return SR.Keys.GetString("Command_CommandTextCommandStreamBothSet");
			}
		}

		public static string Connection_DatabaseNameEmpty
		{
			get
			{
				return SR.Keys.GetString("Connection_DatabaseNameEmpty");
			}
		}

		public static string Connection_NoInformationAboutDataSourcesReturned
		{
			get
			{
				return SR.Keys.GetString("Connection_NoInformationAboutDataSourcesReturned");
			}
		}

		public static string Connection_PropertyNameEmpty
		{
			get
			{
				return SR.Keys.GetString("Connection_PropertyNameEmpty");
			}
		}

		public static string ICollection_CannotCopyToMultidimensionalArray
		{
			get
			{
				return SR.Keys.GetString("ICollection_CannotCopyToMultidimensionalArray");
			}
		}

		public static string ICollection_ItemWithThisNameDoesNotExistInTheCollection
		{
			get
			{
				return SR.Keys.GetString("ICollection_ItemWithThisNameDoesNotExistInTheCollection");
			}
		}

		public static string Connection_EffectiveUserNameEmpty
		{
			get
			{
				return SR.Keys.GetString("Connection_EffectiveUserNameEmpty");
			}
		}

		public static string TransactionAlreadyComplete
		{
			get
			{
				return SR.Keys.GetString("TransactionAlreadyComplete");
			}
		}

		public static string Command_OnlyAdomdTransactionObjectIsSupported
		{
			get
			{
				return SR.Keys.GetString("Command_OnlyAdomdTransactionObjectIsSupported");
			}
		}

		public static string Command_OnlyActiveTransactionCanBeAssigned
		{
			get
			{
				return SR.Keys.GetString("Command_OnlyActiveTransactionCanBeAssigned");
			}
		}

		public static string Command_OnlyTransactionAssociatedWithTheSameConnectionCanBeAssigned
		{
			get
			{
				return SR.Keys.GetString("Command_OnlyTransactionAssociatedWithTheSameConnectionCanBeAssigned");
			}
		}

		public static string InvalidOperationPriorToFetchAllProperties
		{
			get
			{
				return SR.Keys.GetString("InvalidOperationPriorToFetchAllProperties");
			}
		}

		public static string MetadataCache_Abandoned
		{
			get
			{
				return SR.Keys.GetString("MetadataCache_Abandoned");
			}
		}

		public static string ArgumentErrorUniqueNameEmpty
		{
			get
			{
				return SR.Keys.GetString("ArgumentErrorUniqueNameEmpty");
			}
		}

		public static string ArgumentErrorInvalidSchemaObjectType
		{
			get
			{
				return SR.Keys.GetString("ArgumentErrorInvalidSchemaObjectType");
			}
		}

		public static string InvalidArgument
		{
			get
			{
				return SR.Keys.GetString("InvalidArgument");
			}
		}

		protected SR()
		{
		}

		public static string Command_InvalidTimeout(string timeout)
		{
			return SR.Keys.GetString("Command_InvalidTimeout", timeout);
		}

		public static string Cellset_propertyIsUnknown(string propertyName)
		{
			return SR.Keys.GetString("Cellset_propertyIsUnknown", propertyName);
		}

		public static string DatasetResponse_HierarchyWithSameNameOnSameAxis(string hierarchyName, string axisName)
		{
			return SR.Keys.GetString("DatasetResponse_HierarchyWithSameNameOnSameAxis", hierarchyName, axisName);
		}

		public static string Schema_UnexpectedResponseForSchema(string schemaName)
		{
			return SR.Keys.GetString("Schema_UnexpectedResponseForSchema", schemaName);
		}

		public static string Schema_PropertyIsMissingOrOfAnUnexpectedType(string schemaName, string propertyName)
		{
			return SR.Keys.GetString("Schema_PropertyIsMissingOrOfAnUnexpectedType", schemaName, propertyName);
		}

		public static string Connection_InvalidProperty(string propertyName)
		{
			return SR.Keys.GetString("Connection_InvalidProperty", propertyName);
		}

		public static string Parameter_Already_Exists(string parameterName)
		{
			return SR.Keys.GetString("Parameter_Already_Exists", parameterName);
		}

		public static string Indexer_ObjectNotFound(string objectName)
		{
			return SR.Keys.GetString("Indexer_ObjectNotFound", objectName);
		}

		public static string Metadata_CubeHasbeenUpdated(string objectName)
		{
			return SR.Keys.GetString("Metadata_CubeHasbeenUpdated", objectName);
		}

		public static string Property_UnknownProperty(string propertyName)
		{
			return SR.Keys.GetString("Property_UnknownProperty", propertyName);
		}

		public static string CellIndexer_InvalidNumberOfAxesIndexers(int numberPresent, int numberProvided)
		{
			return SR.Keys.GetString("CellIndexer_InvalidNumberOfAxesIndexers", numberPresent, numberProvided);
		}

		public static string CellIndexer_IndexOutOfRange(int numberAxis, int maxIndexForAxis)
		{
			return SR.Keys.GetString("CellIndexer_IndexOutOfRange", numberAxis, maxIndexForAxis);
		}

		public static string CellIndexer_InvalidIndexType(int numberAxis)
		{
			return SR.Keys.GetString("CellIndexer_InvalidIndexType", numberAxis);
		}

		public static string CellSet_InvalidStateOfReader(string state)
		{
			return SR.Keys.GetString("CellSet_InvalidStateOfReader", state);
		}

		public static string Connection_FailedToSetProperty(string propName, string propValue)
		{
			return SR.Keys.GetString("Connection_FailedToSetProperty", propName, propValue);
		}

		public static string Property_PropertyNotFound(string name)
		{
			return SR.Keys.GetString("Property_PropertyNotFound", name);
		}

		public static string Restrictions_TypesMismatch(string restrictionName, string expectedType, string actualType)
		{
			return SR.Keys.GetString("Restrictions_TypesMismatch", restrictionName, expectedType, actualType);
		}

		public static string ICollection_NotEnoughSpaceToCopyTo(int available, int needed)
		{
			return SR.Keys.GetString("ICollection_NotEnoughSpaceToCopyTo", available, needed);
		}

		public static string ArgumentErrorUnsupportedParameterType(string parameterType)
		{
			return SR.Keys.GetString("ArgumentErrorUnsupportedParameterType", parameterType);
		}
	}
}
