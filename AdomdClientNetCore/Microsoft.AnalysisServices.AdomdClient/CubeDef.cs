using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CubeDef : IAdomdBaseObject, IMetadataObject
	{
		private const string cubeSourceColumn = "CUBE_SOURCE";

		private const int cubeSourceCube = 1;

		private const int cubeSourceDimension = 2;

		private BaseObjectData baseData;

		private DimensionCollection dimensions;

		private KpiCollection kpis;

		private MeasureCollection measures;

		private NamedSetCollection namedSets;

		private PropertyCollection propertyCollection;

		private DateTime populationTime;

		internal IMetadataCache metadataCache;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string cubeNameColumn = "CUBE_NAME";

		internal static string cubeNameRest = CubeDef.cubeNameColumn;

		internal static string cubeCaption = "CUBE_CAPTION";

		internal static string descriptionColumn = "DESCRIPTION";

		internal static string lastDataUpdateColumn = "LAST_DATA_UPDATE";

		internal static string lastSchemaUpdateColumn = "LAST_SCHEMA_UPDATE";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.CubeRow, CubeDef.cubeNameColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.CubeRow, CubeDef.descriptionColumn).ToString();
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.CubeRow, CubeDef.lastSchemaUpdateColumn), CultureInfo.InvariantCulture);
			}
		}

		public DateTime LastProcessed
		{
			get
			{
				return Convert.ToDateTime(AdomdUtils.GetProperty(this.CubeRow, CubeDef.lastDataUpdateColumn), CultureInfo.InvariantCulture);
			}
		}

		public string Caption
		{
			get
			{
				if (this.CubeRow.Table.Columns.Contains(CubeDef.cubeCaption))
				{
					return AdomdUtils.GetProperty(this.CubeRow, CubeDef.cubeCaption).ToString();
				}
				return this.Name;
			}
		}

		public CubeType Type
		{
			get
			{
				if (!this.CubeRow.Table.Columns.Contains("CUBE_SOURCE"))
				{
					return CubeType.Cube;
				}
				int num = Convert.ToInt32(AdomdUtils.GetProperty(this.CubeRow, "CUBE_SOURCE"), CultureInfo.InvariantCulture);
				if (num == 1)
				{
					return CubeType.Cube;
				}
				if (num == 2)
				{
					return CubeType.Dimension;
				}
				return CubeType.Unknown;
			}
		}

		public AdomdConnection ParentConnection
		{
			get
			{
				return this.Connection;
			}
		}

		public DimensionCollection Dimensions
		{
			get
			{
				if (this.dimensions == null)
				{
					this.dimensions = new DimensionCollection(this.Connection, this);
				}
				else
				{
					this.dimensions.CollectionInternal.CheckCache();
				}
				return this.dimensions;
			}
		}

		public KpiCollection Kpis
		{
			get
			{
				if (!this.Connection.IsPostYukonProvider())
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				if (this.kpis == null)
				{
					this.kpis = new KpiCollection(this.Connection, this);
				}
				else
				{
					this.kpis.CollectionInternal.CheckCache();
				}
				return this.kpis;
			}
		}

		public NamedSetCollection NamedSets
		{
			get
			{
				if (this.namedSets == null)
				{
					this.namedSets = new NamedSetCollection(this.Connection, this);
				}
				else
				{
					this.namedSets.CollectionInternal.Refresh();
				}
				return this.namedSets;
			}
		}

		public MeasureCollection Measures
		{
			get
			{
				if (this.measures == null)
				{
					this.measures = new MeasureCollection(this.Connection, this);
				}
				else
				{
					this.measures.CollectionInternal.Refresh();
				}
				return this.measures;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertyCollection == null)
				{
					this.propertyCollection = new PropertyCollection(this.CubeRow, this);
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
				return typeof(CubeDef);
			}
		}

		internal DataRow CubeRow
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

		internal CubeDef(DataRow cubeRow, AdomdConnection connection, DateTime populationTime, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, cubeRow, null, null, catalog, sessionId);
			this.populationTime = populationTime;
			this.metadataCache = new CubeMetadataCache(connection, this);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public object GetSchemaObject(SchemaObjectType schemaObjectType, string uniqueName)
		{
			return this.GetSchemaObject(schemaObjectType, uniqueName, true);
		}

		public object GetSchemaObject(SchemaObjectType schemaObjectType, string uniqueName, bool retryUniqueNameOnServer)
		{
			if (uniqueName == null)
			{
				throw new ArgumentNullException("uniqueName");
			}
			if (uniqueName.Length == 0)
			{
				throw new ArgumentException(SR.ArgumentErrorUniqueNameEmpty, "uniqueName");
			}
			if (!SchemaObjectTypeChecker.IsValidSchemaObjectType(schemaObjectType))
			{
				throw new ArgumentException(SR.ArgumentErrorInvalidSchemaObjectType, "schemaObjectType");
			}
			return this.InternalGetSchemaObject(schemaObjectType, uniqueName, retryUniqueNameOnServer);
		}

		internal object InternalGetSchemaObject(SchemaObjectType schemaObjectType, string uniqueName)
		{
			return this.InternalGetSchemaObject(schemaObjectType, uniqueName, false);
		}

		internal object InternalGetSchemaObject(SchemaObjectType schemaObjectType, string uniqueName, bool retryUniqueName)
		{
			DataRow dataRow;
			if (SchemaObjectType.ObjectTypeMember == schemaObjectType)
			{
				ListDictionary listDictionary = new ListDictionary();
				listDictionary.Add(CubeCollectionInternal.cubeNameRest, this.Name);
				AdomdUtils.AddCubeSourceRestrictionIfApplicable(this.Connection, listDictionary);
				string requestType = "MDSCHEMA_MEMBERS";
				string key = "MEMBER_UNIQUE_NAME";
				listDictionary.Add(key, uniqueName);
				AdomdUtils.AddMemberBinaryRestrictionIfApplicable(this.Connection, listDictionary);
				DataRowCollection rows = AdomdUtils.GetRows(this.Connection, requestType, listDictionary);
				if (rows.Count != 1)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(uniqueName), "uniqueName");
				}
				dataRow = rows[0];
			}
			else
			{
				dataRow = this.metadataCache.FindObjectByUniqueName(schemaObjectType, uniqueName);
				if (dataRow == null && retryUniqueName)
				{
					ListDictionary listDictionary2 = new ListDictionary();
					listDictionary2.Add(CubeCollectionInternal.cubeNameRest, this.Name);
					AdomdUtils.AddCubeSourceRestrictionIfApplicable(this.Connection, listDictionary2);
					string schemaName;
					string text;
					switch (schemaObjectType)
					{
					case SchemaObjectType.ObjectTypeDimension:
						schemaName = DimensionCollectionInternal.schemaName;
						text = DimensionCollectionInternal.dimUNameRest;
						goto IL_16D;
					case SchemaObjectType.ObjectTypeHierarchy:
						schemaName = HierarchyCollectionInternal.schemaName;
						text = HierarchyCollectionInternal.hierUNameRest;
						goto IL_16D;
					case SchemaObjectType.ObjectTypeLevel:
						schemaName = LevelCollectionInternal.schemaName;
						text = LevelCollectionInternal.levelUNameRest;
						goto IL_16D;
					case SchemaObjectType.ObjectTypeMember:
					case (SchemaObjectType)5:
						break;
					case SchemaObjectType.ObjectTypeMeasure:
						schemaName = MeasureCollectionInternal.schemaName;
						text = Measure.uniqueNameColumn;
						goto IL_16D;
					case SchemaObjectType.ObjectTypeKpi:
						schemaName = KpiCollectionInternal.schemaName;
						text = Kpi.kpiNameColumn;
						goto IL_16D;
					default:
						if (schemaObjectType == SchemaObjectType.ObjectTypeNamedSet)
						{
							schemaName = NamedSetCollectionInternal.schemaName;
							text = "SET_NAME";
							goto IL_16D;
						}
						break;
					}
					throw new ArgumentOutOfRangeException("schemaObjectType");
					IL_16D:
					listDictionary2.Add(text, uniqueName);
					AdomdUtils.AddObjectVisibilityRestrictionIfApplicable(this.Connection, schemaName, listDictionary2);
					DataRowCollection rows2 = AdomdUtils.GetRows(this.Connection, schemaName, listDictionary2);
					if (rows2.Count > 0)
					{
						uniqueName = (rows2[0][text] as string);
						if (uniqueName != null)
						{
							dataRow = this.metadataCache.FindObjectByUniqueName(schemaObjectType, uniqueName);
						}
					}
				}
			}
			if (dataRow == null)
			{
				throw new ArgumentException(SR.Indexer_ObjectNotFound(uniqueName), "uniqueName");
			}
			switch (schemaObjectType)
			{
			case SchemaObjectType.ObjectTypeDimension:
			{
				object result = DimensionCollectionInternal.GetDimensionByRow(this.Connection, dataRow, this, this.baseData.Catalog, this.baseData.SessionID);
				return result;
			}
			case SchemaObjectType.ObjectTypeHierarchy:
			{
				Dimension parentDimension = (Dimension)this.InternalGetSchemaObject(SchemaObjectType.ObjectTypeDimension, dataRow[Dimension.uniqueNameColumn].ToString());
				object result = HierarchyCollectionInternal.GetHiearchyByRow(this.Connection, dataRow, parentDimension, this.baseData.Catalog, this.baseData.SessionID);
				return result;
			}
			case SchemaObjectType.ObjectTypeLevel:
			{
				string uniqueName2 = dataRow[Hierarchy.uniqueNameColumn].ToString();
				Hierarchy parentHierarchy = (Hierarchy)this.InternalGetSchemaObject(SchemaObjectType.ObjectTypeHierarchy, uniqueName2);
				object result = LevelCollectionInternal.GetLevelByRow(this.Connection, dataRow, parentHierarchy, this.baseData.Catalog, this.baseData.SessionID);
				return result;
			}
			case SchemaObjectType.ObjectTypeMember:
			{
				object result = new Member(this.Connection, dataRow, null, null, MemberOrigin.Metadata, this.Name, null, -1, this.baseData.Catalog, this.baseData.SessionID);
				return result;
			}
			case (SchemaObjectType)5:
				break;
			case SchemaObjectType.ObjectTypeMeasure:
			{
				object result = MeasureCollectionInternal.GetMeasureByRow(this.Connection, dataRow, this, this.baseData.Catalog, this.baseData.SessionID);
				return result;
			}
			case SchemaObjectType.ObjectTypeKpi:
			{
				object result = KpiCollectionInternal.GetKpiByRow(this.Connection, dataRow, this, this.baseData.Catalog, this.baseData.SessionID);
				return result;
			}
			default:
				if (schemaObjectType == SchemaObjectType.ObjectTypeNamedSet)
				{
					object result = NamedSetCollectionInternal.GetNamedSetByRow(this.Connection, dataRow, this, this.baseData.Catalog, this.baseData.SessionID);
					return result;
				}
				break;
			}
			throw new ArgumentOutOfRangeException("schemaObjectType");
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

		public static bool operator ==(CubeDef o1, CubeDef o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(CubeDef o1, CubeDef o2)
		{
			return !(o1 == o2);
		}

		internal MemberCollection GetMembers(string memberSet, string[] properties, Level parentLevel, Member parentMember)
		{
			string dimensionPropertiesClause = MemberQueryGenerator.GetDimensionPropertiesClause(properties);
			string memberQuery = MemberQueryGenerator.GetMemberQuery(memberSet, dimensionPropertiesClause, this);
			return this.ExecuteMembersQuery(memberQuery, parentLevel, parentMember);
		}

		private MemberCollection ExecuteMembersQuery(string memberMdxQuery, Level parentLevel, Member parentMember)
		{
			return this.ExecuteMembersQuery(memberMdxQuery, parentLevel, parentMember, 0, 0);
		}

		private MemberCollection ExecuteMembersQuery(string memberMdxQuery, Level parentLevel, Member parentMember, int memberAxisPosition, int memberHierarhcyPosition)
		{
			if (memberAxisPosition < 0)
			{
				throw new ArgumentOutOfRangeException("memberAxisPosition");
			}
			if (memberHierarhcyPosition < 0)
			{
				throw new ArgumentOutOfRangeException("memberHierarhcyPosition");
			}
			AdomdConnection connection = this.Connection;
			if (connection == null)
			{
				throw new NotSupportedException(SR.NotSupportedWhenConnectionMissing);
			}
			AdomdUtils.CheckConnectionOpened(connection);
			AdomdCommand adomdCommand = new AdomdCommand(memberMdxQuery, connection);
			CellSet cellSet = adomdCommand.ExecuteCellSet();
			if (memberAxisPosition >= cellSet.Axes.Count)
			{
				throw new ArgumentOutOfRangeException("memberAxisPosition");
			}
			Axis axis = cellSet.Axes[memberAxisPosition];
			IDSFDataSet axisDataset = axis.Set.AxisDataset;
			DataTable memberHierarchyDataTable = null;
			if (memberHierarhcyPosition != 0 || axisDataset.Count != 0)
			{
				if (memberHierarhcyPosition >= axisDataset.Count)
				{
					throw new ArgumentOutOfRangeException("memberHierarhcyPosition");
				}
				memberHierarchyDataTable = axisDataset[memberHierarhcyPosition];
			}
			return new MemberCollection(connection, memberHierarchyDataTable, this.Name, parentLevel, parentMember);
		}
	}
}
