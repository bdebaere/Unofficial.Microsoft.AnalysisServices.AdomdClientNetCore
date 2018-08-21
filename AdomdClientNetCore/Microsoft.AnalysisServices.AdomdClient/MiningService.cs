using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningService : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		private DateTime populationTime;

		private MiningServiceParameterCollection miningServiceParameters;

		private PropertyCollection propertyCollection;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string miningServiceNameColumn = "SERVICE_NAME";

		internal static string miningServiceNameRest = MiningService.miningServiceNameColumn;

		internal static string descriptionColumn = "DESCRIPTION";

		internal static string viewerTypeColumn = "VIEWER_TYPE";

		internal static string displayNameColumn = "SERVICE_DISPLAY_NAME";

		internal static string guidColumn = "GUID";

		internal static string predictionLimitColumn = "PREDICTION_LIMIT";

		internal static string supportedDistributionFlagsColumn = "SUPPORTED_DISTRIBUTION_FLAGS";

		internal static string supportedInputContentTypesColumn = "SUPPORTED_INPUT_CONTENT_TYPES";

		internal static string supportedPredictionContentTypesColumn = "SUPPORTED_PREDICTION_CONTENT_TYPES";

		internal static string supportedModelingFlagsColumn = "SUPPORTED_MODELING_FLAGS";

		internal static string trainingComplexityColumn = "TRAINING_COMPLEXITY";

		internal static string predictionComplexityColumn = "PREDICTION_COMPLEXITY";

		internal static string expectedQualityColumn = "EXPECTED_QUALITY";

		internal static string scalingColumn = "SCALING";

		internal static string controlColumn = "CONTROL";

		internal static string allowIncrementalInsertColumn = "ALLOW_INCREMENTAL_INSERT";

		internal static string allowPMMLInitializationColumn = "ALLOW_PMML_INITIALIZATION";

		internal static string allowDuplicateKeyColumn = "ALLOW_DUPLICATE_KEY";

		internal static string supportsDMDimensionsColumn = "MSOLAP_SUPPORTS_DATA_MINING_DIMENSIONS";

		internal static string supportsDrillthroughColumn = "MSOLAP_SUPPORTS_DRILLTHROUGH";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.miningServiceNameColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.descriptionColumn).ToString();
			}
		}

		public string ViewerType
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.viewerTypeColumn).ToString();
			}
		}

		public string DisplayName
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.displayNameColumn).ToString();
			}
		}

		public string Guid
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.guidColumn).ToString();
			}
		}

		public int PredictionLimit
		{
			get
			{
				return Convert.ToInt32(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.predictionLimitColumn).ToString(), 10);
			}
		}

		public MiningServiceTrainingComplexity TrainingComplexity
		{
			get
			{
				return (MiningServiceTrainingComplexity)Convert.ToInt32(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.trainingComplexityColumn).ToString(), 10);
			}
		}

		public MiningServicePredictionComplexity PredictionComplexity
		{
			get
			{
				return (MiningServicePredictionComplexity)Convert.ToInt32(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.predictionComplexityColumn).ToString(), 10);
			}
		}

		public MiningServiceExpectedQuality ExpectedQuality
		{
			get
			{
				return (MiningServiceExpectedQuality)Convert.ToInt32(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.expectedQualityColumn).ToString(), 10);
			}
		}

		public MiningServiceScaling Scaling
		{
			get
			{
				return (MiningServiceScaling)Convert.ToInt32(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.scalingColumn).ToString(), 10);
			}
		}

		public MiningServiceControl Control
		{
			get
			{
				return (MiningServiceControl)Convert.ToInt32(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.controlColumn).ToString(), 10);
			}
		}

		public bool AllowsIncrementalInsert
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.allowIncrementalInsertColumn), CultureInfo.InvariantCulture);
			}
		}

		public bool AllowsPMMLInitialization
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.allowPMMLInitializationColumn), CultureInfo.InvariantCulture);
			}
		}

		public bool AllowsDuplicateKey
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.allowDuplicateKeyColumn), CultureInfo.InvariantCulture);
			}
		}

		public bool SupportsDMDimensions
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.supportsDMDimensionsColumn), CultureInfo.InvariantCulture);
			}
		}

		public bool SupportsDrillthrough
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.supportsDrillthroughColumn), CultureInfo.InvariantCulture);
			}
		}

		public MiningColumnDistribution[] SupportedDistributionFlags
		{
			get
			{
				string text = AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.supportedDistributionFlagsColumn).ToString();
				string[] array = text.Split(new char[]
				{
					','
				});
				MiningColumnDistribution[] array2 = new MiningColumnDistribution[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					array2[i] = MiningModelColumn.DistributionFromString(text2.Trim());
				}
				return array2;
			}
		}

		public string[] SupportedInputContentTypes
		{
			get
			{
				string text = AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.supportedInputContentTypesColumn).ToString();
				string[] array = text.Split(new char[]
				{
					','
				});
				string[] array2 = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					array2[i] = text2.Trim();
				}
				return array2;
			}
		}

		public string[] SupportedPredictionContentTypes
		{
			get
			{
				string text = AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.supportedPredictionContentTypesColumn).ToString();
				string[] array = text.Split(new char[]
				{
					','
				});
				string[] array2 = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					array2[i] = text2.Trim();
				}
				return array2;
			}
		}

		public string[] SupportedModelingFlags
		{
			get
			{
				string text = AdomdUtils.GetProperty(this.MiningServiceRow, MiningService.supportedModelingFlagsColumn).ToString();
				string[] array = text.Split(new char[]
				{
					','
				});
				string[] array2 = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					array2[i] = text2.Trim();
				}
				return array2;
			}
		}

		public AdomdConnection ParentConnection
		{
			get
			{
				return this.Connection;
			}
		}

		public MiningServiceParameterCollection AvailableParameters
		{
			get
			{
				if (this.miningServiceParameters == null)
				{
					this.miningServiceParameters = new MiningServiceParameterCollection(this.Connection, this);
				}
				else
				{
					this.miningServiceParameters.CollectionInternal.CheckCache();
				}
				return this.miningServiceParameters;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertyCollection == null)
				{
					this.propertyCollection = new PropertyCollection(this.MiningServiceRow, this);
				}
				return this.propertyCollection;
			}
		}

		AdomdConnection IAdomdBaseObject.Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		bool IAdomdBaseObject.IsMetadata
		{
			get
			{
				return this.baseData.IsMetadata;
			}
			set
			{
				this.baseData.IsMetadata = value;
			}
		}

		object IAdomdBaseObject.MetadataData
		{
			get
			{
				return this.baseData.MetadataData;
			}
			set
			{
				this.baseData.MetadataData = value;
			}
		}

		IAdomdBaseObject IAdomdBaseObject.ParentObject
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		string IAdomdBaseObject.CubeName
		{
			get
			{
				return this.Name;
			}
		}

		SchemaObjectType IAdomdBaseObject.SchemaObjectType
		{
			get
			{
				return SchemaObjectType.ObjectTypeDimension;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				return this.Name;
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				return this.Connection;
			}
		}

		string IMetadataObject.Catalog
		{
			get
			{
				return this.baseData.Catalog;
			}
		}

		string IMetadataObject.SessionId
		{
			get
			{
				return this.baseData.SessionID;
			}
		}

		string IMetadataObject.CubeName
		{
			get
			{
				return this.Name;
			}
		}

		string IMetadataObject.UniqueName
		{
			get
			{
				return this.Name;
			}
		}

		Type IMetadataObject.Type
		{
			get
			{
				return typeof(MiningService);
			}
		}

		internal DataRow MiningServiceRow
		{
			get
			{
				return (DataRow)this.baseData.MetadataData;
			}
		}

		internal DateTime PopulatedTime
		{
			get
			{
				return this.populationTime;
			}
		}

		private AdomdConnection Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		internal MiningService(DataRow miningServiceRow, AdomdConnection connection, DateTime populationTime, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, miningServiceRow, null, null, catalog, sessionId);
			this.populationTime = populationTime;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			if (!this.hashCodeCalculated)
			{
				this.hashCode = AdomdUtils.GetHashCode(this);
				this.hashCodeCalculated = true;
			}
			return this.hashCode;
		}

		public override bool Equals(object obj)
		{
			return AdomdUtils.Equals(this, obj as IMetadataObject);
		}

		public static bool operator ==(MiningService o1, MiningService o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningService o1, MiningService o2)
		{
			return !(o1 == o2);
		}
	}
}
