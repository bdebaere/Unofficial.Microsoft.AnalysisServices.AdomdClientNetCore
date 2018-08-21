using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningStructureColumn : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		private PropertyCollection propertiesCollection;

		private MiningStructureColumnCollection columns;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string miningStructureColumnNameColumn = "COLUMN_NAME";

		internal static string miningStructureColumnModelingFlag = "MODELING_FLAG";

		private static string contentColumn = "CONTENT_TYPE";

		private static string dataTypeColumn = "DATA_TYPE";

		private static string containingColumn = "CONTAINING_COLUMN";

		private static string descriptionColumn = "DESCRIPTION";

		private static string distributionColumn = "DISTRIBUTION";

		private static string isRelatedToKeyColumn = "IS_RELATED_TO_KEY";

		private static string relatedAttributeColumn = "RELATED_ATTRIBUTE";

		private static string isProcessedColumn = "IS_POPULATED";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.miningStructureColumnNameColumn).ToString();
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
				return AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.miningStructureColumnModelingFlag).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.descriptionColumn).ToString();
			}
		}

		public string Content
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.contentColumn).ToString();
			}
		}

		public MiningColumnType Type
		{
			get
			{
				uint type = Convert.ToUInt32(AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.dataTypeColumn).ToString(), 10);
				return MiningModelColumn.DBTYPEToMiningColumnType(type);
			}
		}

		public MiningColumnDistribution Distribution
		{
			get
			{
				string strDist = AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.distributionColumn).ToString();
				return MiningModelColumn.DistributionFromString(strDist);
			}
		}

		public bool IsRelatedToKey
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.isRelatedToKeyColumn), CultureInfo.InvariantCulture);
			}
		}

		public string RelatedAttribute
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.relatedAttributeColumn).ToString();
			}
		}

		public bool IsProcessed
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.isProcessedColumn), CultureInfo.InvariantCulture);
			}
		}

		public string ContainingColumn
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.containingColumn).ToString();
			}
		}

		public string UniqueName
		{
			get
			{
				return ((IAdomdBaseObject)this).InternalUniqueName;
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return this.ParentMiningStructure.LastUpdated;
			}
		}

		public DateTime LastProcessed
		{
			get
			{
				return this.ParentMiningStructure.LastProcessed;
			}
		}

		public MiningStructure ParentMiningStructure
		{
			get
			{
				object parentObject = this.baseData.ParentObject;
				if (parentObject is MiningStructure)
				{
					return (MiningStructure)parentObject;
				}
				if (parentObject is MiningStructureColumn)
				{
					MiningStructureColumn miningStructureColumn = (MiningStructureColumn)parentObject;
					return miningStructureColumn.ParentMiningStructure;
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

		public MiningStructureColumnCollection Columns
		{
			get
			{
				return this.columns;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.MiningStructureColumnRow, this);
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
				return SchemaObjectType.ObjectTypeMiningStructureColumn;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureColumnRow, MiningStructureColumn.miningStructureColumnNameColumn).ToString();
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
				return typeof(MiningStructureColumn);
			}
		}

		internal DataRow MiningStructureColumnRow
		{
			get
			{
				return (DataRow)this.baseData.MetadataData;
			}
		}

		internal MiningStructureColumn(AdomdConnection connection, DataRow miningStructureColumnRow, IAdomdBaseObject parentObject, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, miningStructureColumnRow, parentObject, null, catalog, sessionId);
			this.columns = new MiningStructureColumnCollection(connection, this);
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

		public static bool operator ==(MiningStructureColumn o1, MiningStructureColumn o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningStructureColumn o1, MiningStructureColumn o2)
		{
			return !(o1 == o2);
		}
	}
}
