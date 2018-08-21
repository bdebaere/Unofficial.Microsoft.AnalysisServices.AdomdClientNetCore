using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningModelColumn : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		private PropertyCollection propertiesCollection;

		private MiningModelColumnCollection columns;

		private MiningValueCollection values;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string miningModelColumnNameColumn = "COLUMN_NAME";

		internal static string miningModelColumnModelingFlag = "MODELING_FLAG";

		internal static string isInputColumn = "IS_INPUT";

		internal static string isPredictableColumn = "IS_PREDICTABLE";

		private static string contentColumn = "CONTENT_TYPE";

		private static string dataTypeColumn = "DATA_TYPE";

		private static string containingColumn = "CONTAINING_COLUMN";

		private static string descriptionColumn = "DESCRIPTION";

		private static string distributionColumn = "DISTRIBUTION";

		private static string isRelatedToKeyColumn = "IS_RELATED_TO_KEY";

		private static string relatedAttributeColumn = "RELATED_ATTRIBUTE";

		private static string isProcessedColumn = "IS_POPULATED";

		private static string predictionScoreColumn = "PREDICTION_SCORE";

		private static string sourceColumn = "SOURCE_COLUMN";

		private static string filterColumn = "FILTER";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.miningModelColumnNameColumn).ToString();
			}
		}

		public string FullyQualifiedName
		{
			get
			{
				string result;
				if (this.ContainingColumn.Length == 0)
				{
					result = "[" + this.Name + "]";
				}
				else
				{
					result = string.Concat(new string[]
					{
						"[",
						this.ContainingColumn,
						"].[",
						this.Name,
						"]"
					});
				}
				return result;
			}
		}

		public string Flags
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.miningModelColumnModelingFlag).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.descriptionColumn).ToString();
			}
		}

		public MiningColumnDistribution Distribution
		{
			get
			{
				string strDist = AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.distributionColumn).ToString();
				return MiningModelColumn.DistributionFromString(strDist);
			}
		}

		public bool IsInput
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.isInputColumn), CultureInfo.InvariantCulture);
			}
		}

		public bool IsPredictable
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.isPredictableColumn), CultureInfo.InvariantCulture);
			}
		}

		public bool IsTable
		{
			get
			{
				return this.Type == MiningColumnType.Table;
			}
		}

		public bool IsRelatedToKey
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.isRelatedToKeyColumn), CultureInfo.InvariantCulture);
			}
		}

		public string RelatedAttribute
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.relatedAttributeColumn).ToString();
			}
		}

		public string Content
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.contentColumn).ToString();
			}
		}

		public MiningColumnType Type
		{
			get
			{
				uint type = Convert.ToUInt32(AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.dataTypeColumn).ToString(), 10);
				return MiningModelColumn.DBTYPEToMiningColumnType(type);
			}
		}

		public string ContainingColumn
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.containingColumn).ToString();
			}
		}

		public string UniqueName
		{
			get
			{
				return ((IAdomdBaseObject)this).InternalUniqueName;
			}
		}

		public bool IsProcessed
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.isProcessedColumn), CultureInfo.InvariantCulture);
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return this.ParentMiningModel.LastUpdated;
			}
		}

		public DateTime LastProcessed
		{
			get
			{
				return this.ParentMiningModel.LastProcessed;
			}
		}

		public double PredictionScore
		{
			get
			{
				return Convert.ToDouble(AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.predictionScoreColumn), CultureInfo.InvariantCulture);
			}
		}

		public string StructureColumn
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.sourceColumn).ToString();
			}
		}

		public string Filter
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.filterColumn).ToString();
			}
		}

		public MiningModel ParentMiningModel
		{
			get
			{
				object parentObject = this.baseData.ParentObject;
				if (parentObject is MiningModel)
				{
					return (MiningModel)parentObject;
				}
				if (parentObject is MiningModelColumn)
				{
					MiningModelColumn miningModelColumn = (MiningModelColumn)parentObject;
					return miningModelColumn.ParentMiningModel;
				}
				return null;
			}
		}

		public object Parent
		{
			get
			{
				return this.baseData.ParentObject;
			}
		}

		public MiningModelColumnCollection Columns
		{
			get
			{
				return this.columns;
			}
		}

		public MiningValueCollection Values
		{
			get
			{
				if (this.values == null)
				{
					this.values = new MiningValueCollection(this);
				}
				return this.values;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.MiningModelColumnRow, this);
				}
				return this.propertiesCollection;
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
				return this.baseData.ParentObject;
			}
			set
			{
				this.baseData.ParentObject = value;
			}
		}

		string IAdomdBaseObject.CubeName
		{
			get
			{
				return this.baseData.ParentObject.CubeName;
			}
		}

		SchemaObjectType IAdomdBaseObject.SchemaObjectType
		{
			get
			{
				return SchemaObjectType.ObjectTypeMiningModelColumn;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelColumnRow, MiningModelColumn.miningModelColumnNameColumn).ToString();
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				return this.baseData.Connection;
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
				return ((IAdomdBaseObject)this).CubeName;
			}
		}

		string IMetadataObject.UniqueName
		{
			get
			{
				return this.UniqueName;
			}
		}

		Type IMetadataObject.Type
		{
			get
			{
				return typeof(MiningModelColumn);
			}
		}

		internal DataRow MiningModelColumnRow
		{
			get
			{
				return (DataRow)this.baseData.MetadataData;
			}
		}

		internal static MiningColumnType DBTYPEToMiningColumnType(uint type)
		{
			MiningColumnType result = MiningColumnType.Missing;
			if (type <= 11u)
			{
				if (type != 0u)
				{
					switch (type)
					{
					case 5u:
						result = MiningColumnType.Double;
						break;
					case 6u:
						break;
					case 7u:
						result = MiningColumnType.Date;
						break;
					default:
						if (type == 11u)
						{
							result = MiningColumnType.Boolean;
						}
						break;
					}
				}
				else
				{
					result = MiningColumnType.Missing;
				}
			}
			else if (type != 20u)
			{
				if (type != 130u)
				{
					if (type == 136u)
					{
						result = MiningColumnType.Table;
					}
				}
				else
				{
					result = MiningColumnType.Text;
				}
			}
			else
			{
				result = MiningColumnType.Long;
			}
			return result;
		}

		internal MiningModelColumn(AdomdConnection connection, DataRow miningModelColumnRow, IAdomdBaseObject parentObject, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, miningModelColumnRow, parentObject, null, catalog, sessionId);
			this.columns = new MiningModelColumnCollection(connection, this);
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

		public static bool operator ==(MiningModelColumn o1, MiningModelColumn o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningModelColumn o1, MiningModelColumn o2)
		{
			return !(o1 == o2);
		}

		internal static MiningColumnDistribution DistributionFromString(string strDist)
		{
			if (string.Compare(strDist, "Normal", StringComparison.Ordinal) == 0)
			{
				return MiningColumnDistribution.Normal;
			}
			if (string.Compare(strDist, "Missing", StringComparison.Ordinal) == 0 || strDist.Length == 0)
			{
				return MiningColumnDistribution.Missing;
			}
			if (string.Compare(strDist, "Uniform", StringComparison.Ordinal) == 0)
			{
				return MiningColumnDistribution.Uniform;
			}
			if (string.Compare(strDist, "Normal", StringComparison.Ordinal) == 0)
			{
				return MiningColumnDistribution.Normal;
			}
			if (string.Compare(strDist, "LogNormal", StringComparison.Ordinal) == 0)
			{
				return MiningColumnDistribution.LogNormal;
			}
			return MiningColumnDistribution.Custom;
		}
	}
}
