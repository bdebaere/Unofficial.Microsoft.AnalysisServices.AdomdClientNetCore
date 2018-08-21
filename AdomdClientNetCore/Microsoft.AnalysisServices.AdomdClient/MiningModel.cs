using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningModel : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		private DateTime populationTime;

		private MiningModelColumnCollection miningModelColumns;

		private MiningContentNodeCollection content;

		private MiningParameterCollection parameters;

		private PropertyCollection propertyCollection;

		private MiningAttributeCollection attributes;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string miningModelNameColumn = "MODEL_NAME";

		internal static string miningModelNameRest = MiningModel.miningModelNameColumn;

		internal static string descriptionColumn = "DESCRIPTION";

		internal static string lastModifiedColumn = "DATE_MODIFIED";

		internal static string lastProcessedColumn = "LAST_PROCESSED";

		internal static string miningModelParameters = "MINING_PARAMETERS";

		internal static string miningModelService = "SERVICE_NAME";

		internal static string miningModelIsPopulated = "IS_POPULATED";

		internal static string createdColumn = "DATE_CREATED";

		internal static string drillThroughEnabledColumn = "MSOLAP_IS_DRILLTHROUGH_ENABLED";

		internal static string caseFilterColumn = "FILTER";

		internal static string trainingSetSizeColumn = "TRAINING_SET_SIZE";

		internal static string structureNameColumn = "MINING_STRUCTURE";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.miningModelNameColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.descriptionColumn).ToString();
			}
		}

		public string Algorithm
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.miningModelService).ToString();
			}
		}

		public bool IsProcessed
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.miningModelIsPopulated), CultureInfo.InvariantCulture);
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.lastModifiedColumn), CultureInfo.InvariantCulture);
			}
		}

		public DateTime LastProcessed
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.lastProcessedColumn), CultureInfo.InvariantCulture);
			}
		}

		public DateTime Created
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.createdColumn), CultureInfo.InvariantCulture);
			}
		}

		public AdomdConnection ParentConnection
		{
			get
			{
				return this.Connection;
			}
		}

		public MiningModelColumnCollection Columns
		{
			get
			{
				if (this.miningModelColumns == null)
				{
					this.miningModelColumns = new MiningModelColumnCollection(this.Connection, this);
				}
				else
				{
					this.miningModelColumns.CollectionInternal.CheckCache();
				}
				return this.miningModelColumns;
			}
		}

		public MiningContentNodeCollection Content
		{
			get
			{
				if (this.content == null)
				{
					this.content = new MiningContentNodeCollection(this.Connection, this);
				}
				else
				{
					this.content.CollectionInternal.CheckCache();
				}
				return this.content;
			}
		}

		public bool AllowDrillThrough
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.drillThroughEnabledColumn), CultureInfo.InvariantCulture);
			}
		}

		public string Filter
		{
			get
			{
				return AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.caseFilterColumn).ToString();
			}
		}

		public long TrainingSetSize
		{
			get
			{
				return Convert.ToInt64(AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.trainingSetSizeColumn), CultureInfo.InvariantCulture);
			}
		}

		public MiningStructure Parent
		{
			get
			{
				if (this.baseData.ParentObject == null)
				{
					string index = AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.structureNameColumn).ToString();
					MiningStructure parentObject = this.ParentConnection.MiningStructures[index];
					this.baseData.ParentObject = parentObject;
				}
				return (MiningStructure)this.baseData.ParentObject;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertyCollection == null)
				{
					this.propertyCollection = new PropertyCollection(this.MiningModelRow, this);
				}
				return this.propertyCollection;
			}
		}

		public MiningParameterCollection Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new MiningParameterCollection(AdomdUtils.GetProperty(this.MiningModelRow, MiningModel.miningModelParameters).ToString());
				}
				return this.parameters;
			}
		}

		public MiningAttributeCollection Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new MiningAttributeCollection(this);
				}
				return this.attributes;
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
				return typeof(MiningModel);
			}
		}

		internal DataRow MiningModelRow
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

		internal MiningModel(DataRow miningModelRow, AdomdConnection connection, DateTime populationTime, string catalog, string sessionId, MiningStructure parentObject)
		{
			this.baseData = new BaseObjectData(connection, true, null, miningModelRow, parentObject, null, catalog, sessionId);
			this.populationTime = populationTime;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public MiningContentNode GetNodeFromUniqueName(string nodeUniqueName)
		{
			MiningContentNodeCollection miningContentNodeCollection = new MiningContentNodeCollection(this.Connection, this, nodeUniqueName);
			MiningContentNode result = null;
			if (miningContentNodeCollection.Count > 0)
			{
				result = miningContentNodeCollection[0];
			}
			return result;
		}

		public MiningAttributeCollection GetAttributes(MiningFeatureSelection filter)
		{
			if (filter == MiningFeatureSelection.All)
			{
				return this.Attributes;
			}
			return new MiningAttributeCollection(this, filter);
		}

		public MiningAttribute FindAttribute(int attributeId)
		{
			return this.Attributes[attributeId];
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

		public static bool operator ==(MiningModel o1, MiningModel o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningModel o1, MiningModel o2)
		{
			return !(o1 == o2);
		}
	}
}
