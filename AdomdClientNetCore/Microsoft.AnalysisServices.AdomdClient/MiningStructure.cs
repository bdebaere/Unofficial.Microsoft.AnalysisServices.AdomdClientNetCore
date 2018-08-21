using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningStructure : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		private DateTime populationTime;

		private MiningStructureColumnCollection miningStructureColumns;

		private PropertyCollection propertyCollection;

		private MiningModelCollection miningModels;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string miningStructureNameColumn = "STRUCTURE_NAME";

		internal static string miningStructureNameRest = MiningStructure.miningStructureNameColumn;

		internal static string miningStructureCaption = "STRUCTURE_CAPTION";

		internal static string descriptionColumn = "DESCRIPTION";

		internal static string lastProcessedColumn = "LAST_PROCESSED";

		internal static string lastModifiedColumn = "DATE_MODIFIED";

		internal static string createdColumn = "DATE_CREATED";

		internal static string isPopulatedColumn = "IS_POPULATED";

		internal static string holdoutMaxPercentColumn = "HOLDOUT_MAXPERCENT";

		internal static string holdoutMaxCasesColumn = "HOLDOUT_MAXCASES";

		internal static string holdoutSeedColumn = "HOLDOUT_SEED";

		internal static string holdoutActualSizeColumn = "HOLDOUT_ACTUAL_SIZE";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.miningStructureNameColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.descriptionColumn).ToString();
			}
		}

		public bool IsProcessed
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.isPopulatedColumn), CultureInfo.InvariantCulture);
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.lastModifiedColumn), CultureInfo.InvariantCulture);
			}
		}

		public DateTime LastProcessed
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.lastProcessedColumn), CultureInfo.InvariantCulture);
			}
		}

		public DateTime Created
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.createdColumn), CultureInfo.InvariantCulture);
			}
		}

		public string Caption
		{
			get
			{
				if (this.MiningStructureRow.Table.Columns.Contains(MiningStructure.miningStructureCaption))
				{
					return AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.miningStructureCaption).ToString();
				}
				return this.Name;
			}
		}

		public AdomdConnection ParentConnection
		{
			get
			{
				return this.Connection;
			}
		}

		public MiningStructureColumnCollection Columns
		{
			get
			{
				if (this.miningStructureColumns == null)
				{
					this.miningStructureColumns = new MiningStructureColumnCollection(this.Connection, this);
				}
				else
				{
					this.miningStructureColumns.CollectionInternal.CheckCache();
				}
				return this.miningStructureColumns;
			}
		}

		public MiningModelCollection MiningModels
		{
			get
			{
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

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertyCollection == null)
				{
					this.propertyCollection = new PropertyCollection(this.MiningStructureRow, this);
				}
				return this.propertyCollection;
			}
		}

		public int HoldoutMaxPercent
		{
			get
			{
				return Convert.ToInt32(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.holdoutMaxPercentColumn), CultureInfo.InvariantCulture);
			}
		}

		public long HoldoutMaxCases
		{
			get
			{
				return Convert.ToInt64(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.holdoutMaxCasesColumn), CultureInfo.InvariantCulture);
			}
		}

		public long HoldoutSeed
		{
			get
			{
				return Convert.ToInt64(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.holdoutSeedColumn), CultureInfo.InvariantCulture);
			}
		}

		public long HoldoutActualSize
		{
			get
			{
				return Convert.ToInt64(AdomdUtils.GetProperty(this.MiningStructureRow, MiningStructure.holdoutActualSizeColumn), CultureInfo.InvariantCulture);
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
				return typeof(MiningStructure);
			}
		}

		internal DataRow MiningStructureRow
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

		internal MiningStructure(DataRow miningStructureRow, AdomdConnection connection, DateTime populationTime, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, miningStructureRow, null, null, catalog, sessionId);
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

		public static bool operator ==(MiningStructure o1, MiningStructure o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningStructure o1, MiningStructure o2)
		{
			return !(o1 == o2);
		}
	}
}
