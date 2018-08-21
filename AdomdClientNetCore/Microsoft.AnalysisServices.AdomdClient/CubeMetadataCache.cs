using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class CubeMetadataCache : IMetadataCache
	{
		private static readonly Dictionary<InternalObjectType, int> internalTypeMap;

		private static readonly SchemaRowsetCacheData[] SchemaRowsetsData;

		private DataSet cacheDataset;

		private AdomdConnection connection;

		private ObjectMetadataCache[] objectMetadataCaches;

		private ListDictionary restrictions;

		private CubeDef parentCube;

		private DateTime lastCacheValidationTime;

		private DataTable cubeInfoTable;

		private DataColumn originalTimeColumn;

		private DataColumn currentTimeColumn;

		private MetadataCacheState cacheState = MetadataCacheState.UpToDate;

		private AdomdUtils.GetInvalidatedMessageDelegate msgDelegate;

		static CubeMetadataCache()
		{
			CubeMetadataCache.SchemaRowsetsData = new SchemaRowsetCacheData[]
			{
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeDimension, DimensionCollectionInternal.schemaName, null, new string[]
				{
					Dimension.dimensionNameColumn,
					Dimension.uniqueNameColumn
				}, Dimension.uniqueNameColumn),
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeHierarchy, HierarchyCollectionInternal.schemaName, DimensionCollectionInternal.dimUNameRest, new string[]
				{
					Hierarchy.uniqueNameColumn,
					Hierarchy.isAttribHierColumn
				}, Hierarchy.uniqueNameColumn),
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeLevel, LevelCollectionInternal.schemaName, HierarchyCollectionInternal.hierUNameRest, new string[]
				{
					Level.levelNameColumn,
					Level.uniqueNameColumn
				}, Level.uniqueNameColumn),
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeMember, "MDSCHEMA_MEMBERS", null, new string[0], "MEMBER_UNIQUE_NAME"),
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeMeasure, MeasureCollectionInternal.schemaName, null, new string[]
				{
					Measure.measureNameColumn,
					Measure.uniqueNameColumn
				}, Measure.uniqueNameColumn),
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeNamedSet, NamedSetCollectionInternal.schemaName, null, new string[]
				{
					"SET_NAME"
				}, "SET_NAME"),
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeLevelProperty, "MDSCHEMA_PROPERTIES", LevelCollectionInternal.levelUNameRest, new string[]
				{
					Level.uniqueNameColumn,
					LevelProperty.levelPropNameColumn
				}, LevelProperty.levelPropNameColumn, new KeyValuePair<string, string>[]
				{
					new KeyValuePair<string, string>("PROPERTY_TYPE", 1.ToString(CultureInfo.InvariantCulture))
				}),
				new SchemaRowsetCacheData(InternalObjectType.InternalTypeKpi, KpiCollectionInternal.schemaName, null, new string[]
				{
					Kpi.kpiNameColumn
				}, Kpi.kpiNameColumn)
			};
			CubeMetadataCache.internalTypeMap = new Dictionary<InternalObjectType, int>(8);
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeDimension] = 0;
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeHierarchy] = 1;
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeLevel] = 2;
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeMember] = 3;
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeMeasure] = 4;
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeNamedSet] = 5;
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeLevelProperty] = 6;
			CubeMetadataCache.internalTypeMap[InternalObjectType.InternalTypeKpi] = 7;
		}

		private static int GetIndexForInternalType(InternalObjectType objectType)
		{
			return CubeMetadataCache.internalTypeMap[objectType];
		}

		internal CubeMetadataCache(AdomdConnection connection, CubeDef parentCube)
		{
			this.msgDelegate = new AdomdUtils.GetInvalidatedMessageDelegate(this.GetCubesUpdatedMessage);
			this.connection = connection;
			this.parentCube = parentCube;
			this.cacheState = MetadataCacheState.UpToDate;
			this.cacheDataset = new DataSet();
			this.cacheDataset.Locale = CultureInfo.InvariantCulture;
			this.restrictions = new ListDictionary();
			this.restrictions.Add(CubeCollectionInternal.cubeNameRest, parentCube.Name);
			AdomdUtils.AddCubeSourceRestrictionIfApplicable(this.connection, this.restrictions);
			int count = CubeMetadataCache.internalTypeMap.Count;
			this.objectMetadataCaches = new ObjectMetadataCache[count];
			bool flag = AdomdUtils.ShouldAddObjectVisibilityRestriction(this.connection);
			for (int i = 0; i < count; i++)
			{
				ListDictionary destinationRestrictions;
				if (flag || CubeMetadataCache.SchemaRowsetsData[i].HasAdditionalRestrictions)
				{
					destinationRestrictions = new ListDictionary();
					AdomdUtils.CopyRestrictions(this.restrictions, destinationRestrictions);
					if (CubeMetadataCache.SchemaRowsetsData[i].HasAdditionalRestrictions)
					{
						CubeMetadataCache.AddSpecificRestrictions(CubeMetadataCache.SchemaRowsetsData[i], destinationRestrictions);
					}
					if (flag)
					{
						AdomdUtils.AddObjectVisibilityRestrictionIfApplicable(this.connection, CubeMetadataCache.SchemaRowsetsData[i].RequestType, destinationRestrictions);
					}
				}
				else
				{
					destinationRestrictions = this.restrictions;
				}
				this.objectMetadataCaches[i] = new ObjectMetadataCache(this, connection, CubeMetadataCache.SchemaRowsetsData[i], destinationRestrictions);
			}
			this.lastCacheValidationTime = parentCube.PopulatedTime;
			this.cubeInfoTable = new DataTable();
			this.cubeInfoTable.Locale = CultureInfo.InvariantCulture;
			this.cubeInfoTable = ((DataRow)((IAdomdBaseObject)parentCube).MetadataData).Table.Clone();
			this.originalTimeColumn = ((DataRow)((IAdomdBaseObject)parentCube).MetadataData).Table.Columns[CubeDef.lastSchemaUpdateColumn];
			this.currentTimeColumn = this.cubeInfoTable.Columns[CubeDef.lastSchemaUpdateColumn];
		}

		IObjectCache IMetadataCache.GetObjectCache(InternalObjectType objectType)
		{
			return this.GetObjectCache(objectType);
		}

		void IMetadataCache.Populate(InternalObjectType objectType)
		{
			this.EnsureNotAbandoned();
			this.EnsureValid();
			this.Populate(objectType);
		}

		void IMetadataCache.Refresh(InternalObjectType objectType)
		{
			this.EnsureNotAbandoned();
			this.EnsureValid();
			this.Refresh(objectType);
		}

		void IMetadataCache.CheckCacheIsValid()
		{
			this.EnsureNotAbandoned();
			this.EnsureValid();
			this.CheckCacheIsValid();
		}

		void IMetadataCache.MarkNeedCheckForValidness()
		{
			if (this.cacheState == MetadataCacheState.UpToDate)
			{
				this.cacheState = MetadataCacheState.NeedsValidnessCheck;
			}
		}

		void IMetadataCache.MarkAbandoned()
		{
			this.cacheState = MetadataCacheState.Abandoned;
			ObjectMetadataCache[] array = this.objectMetadataCaches;
			for (int i = 0; i < array.Length; i++)
			{
				ObjectMetadataCache objectMetadataCache = array[i];
				objectMetadataCache.MarkAbandonedSelf();
			}
		}

		DataRow IMetadataCache.FindObjectByUniqueName(SchemaObjectType objectType, string nameUnique)
		{
			this.Populate((InternalObjectType)objectType);
			ObjectMetadataCache objectCache = this.GetObjectCache((InternalObjectType)objectType);
			string dataTableFilter = AdomdUtils.GetDataTableFilter(this.GetUniqueNameColumn((InternalObjectType)objectType), nameUnique);
			DataRow[] filteredRows = ((IObjectCache)objectCache).GetFilteredRows(null, dataTableFilter);
			if (filteredRows.Length <= 0)
			{
				return null;
			}
			return filteredRows[0];
		}

		private void EnsureValid()
		{
			AdomdUtils.EnsureCacheNotInvalid(this.cacheState, this.msgDelegate);
		}

		private string GetCubesUpdatedMessage()
		{
			return SR.Metadata_CubeHasbeenUpdated(this.parentCube.Name);
		}

		private void EnsureNotAbandoned()
		{
			AdomdUtils.EnsureCacheNotAbandoned(this.cacheState);
		}

		private void Populate(InternalObjectType objectType)
		{
			ObjectMetadataCache objectCache = this.GetObjectCache(objectType);
			bool isInitialized = objectCache.IsInitialized;
			bool isPopulated = objectCache.IsPopulated;
			if (!isInitialized || !isPopulated)
			{
				objectCache.PopulateSelf();
				if (!isInitialized)
				{
					this.CreatePrimaryKeys(objectCache.CacheTable, objectType);
					this.cacheDataset.Tables.Add(objectCache.CacheTable);
				}
				DataTable objectsParentTable = this.GetObjectsParentTable(objectType);
				if (objectsParentTable != null)
				{
					objectCache.Relation = this.CreateRelation(objectsParentTable, objectCache.CacheTable, objectCache.RelationColumn);
				}
				ObjectMetadataCache objectsChildObjectCache = this.GetObjectsChildObjectCache(objectType);
				if (this.GetObjectTable(objectsChildObjectCache) != null)
				{
					objectsChildObjectCache.Relation = this.CreateRelation(objectCache.CacheTable, objectsChildObjectCache.CacheTable, objectsChildObjectCache.RelationColumn);
				}
			}
		}

		private void Refresh(InternalObjectType objectType)
		{
			ObjectMetadataCache objectCache = this.GetObjectCache(objectType);
			objectCache.RefreshSelf();
		}

		private void CheckCacheIsValid()
		{
			if (this.NeedCheckForRefresh())
			{
				this.CheckCacheValid();
			}
		}

		private bool NeedCheckForRefresh()
		{
			return this.cacheState == MetadataCacheState.NeedsValidnessCheck || this.connection.HasAutoSyncTimeElapsed(this.lastCacheValidationTime, DateTime.Now);
		}

		private void CheckCacheValid()
		{
			this.cubeInfoTable.Clear();
			ObjectMetadataCache.Discover(this.connection, CubeCollectionInternal.schemaName, this.restrictions, this.cubeInfoTable, false);
			bool flag;
			if (this.cubeInfoTable.Rows.Count <= 0)
			{
				flag = false;
			}
			else
			{
				DataRow dataRow = this.cubeInfoTable.Rows[0];
				flag = dataRow[this.currentTimeColumn].Equals(((DataRow)((IAdomdBaseObject)this.parentCube).MetadataData)[this.originalTimeColumn]);
			}
			this.lastCacheValidationTime = DateTime.Now;
			if (flag)
			{
				this.cacheState = MetadataCacheState.UpToDate;
			}
			else
			{
				this.cacheState = MetadataCacheState.Invalid;
			}
			this.EnsureValid();
		}

		private DataRelation CreateRelation(DataTable parentTable, DataTable childTable, string relationColumn)
		{
			DataColumn parentColumn = parentTable.Columns[relationColumn];
			DataColumn childColumn = childTable.Columns[relationColumn];
			DataRelation dataRelation = new DataRelation(relationColumn, parentColumn, childColumn);
			this.cacheDataset.Relations.Add(dataRelation);
			return dataRelation;
		}

		private void CreatePrimaryKeys(DataTable table, InternalObjectType objectType)
		{
			SchemaRowsetCacheData schemaRowsetCacheData = CubeMetadataCache.SchemaRowsetsData[CubeMetadataCache.GetIndexForInternalType(objectType)];
			DataColumn[] array = new DataColumn[schemaRowsetCacheData.PrimaryKeyColumns.Length];
			for (int i = 0; i < schemaRowsetCacheData.PrimaryKeyColumns.Length; i++)
			{
				array[i] = table.Columns[schemaRowsetCacheData.PrimaryKeyColumns[i]];
			}
			table.PrimaryKey = array;
		}

		private DataTable GetObjectTable(ObjectMetadataCache cache)
		{
			if (cache != null && cache.IsInitialized)
			{
				return cache.CacheTable;
			}
			return null;
		}

		private ObjectMetadataCache GetObjectsParentObjectCache(InternalObjectType objectType)
		{
			switch (objectType)
			{
			case InternalObjectType.InternalTypeDimension:
			case InternalObjectType.InternalTypeMeasure:
			case InternalObjectType.InternalTypeKpi:
				break;
			case InternalObjectType.InternalTypeHierarchy:
				return this.GetObjectCache(InternalObjectType.InternalTypeDimension);
			case InternalObjectType.InternalTypeLevel:
				return this.GetObjectCache(InternalObjectType.InternalTypeHierarchy);
			case InternalObjectType.InternalTypeMember:
				return null;
			case (InternalObjectType)5:
				goto IL_55;
			default:
				switch (objectType)
				{
				case InternalObjectType.InternalTypeNamedSet:
					break;
				case InternalObjectType.InternalTypeLevelProperty:
					return this.GetObjectCache(InternalObjectType.InternalTypeLevel);
				default:
					goto IL_55;
				}
				break;
			}
			return null;
			IL_55:
			return null;
		}

		private ObjectMetadataCache GetObjectsChildObjectCache(InternalObjectType objectType)
		{
			switch (objectType)
			{
			case InternalObjectType.InternalTypeDimension:
				return this.GetObjectCache(InternalObjectType.InternalTypeHierarchy);
			case InternalObjectType.InternalTypeHierarchy:
				return this.GetObjectCache(InternalObjectType.InternalTypeLevel);
			case InternalObjectType.InternalTypeLevel:
				return this.GetObjectCache(InternalObjectType.InternalTypeLevelProperty);
			case InternalObjectType.InternalTypeMember:
				return null;
			case (InternalObjectType)5:
				goto IL_56;
			case InternalObjectType.InternalTypeMeasure:
			case InternalObjectType.InternalTypeKpi:
				break;
			default:
				switch (objectType)
				{
				case InternalObjectType.InternalTypeNamedSet:
				case InternalObjectType.InternalTypeLevelProperty:
					break;
				default:
					goto IL_56;
				}
				break;
			}
			return null;
			IL_56:
			return null;
		}

		private DataTable GetObjectsParentTable(InternalObjectType objectType)
		{
			return this.GetObjectTable(this.GetObjectsParentObjectCache(objectType));
		}

		private string GetUniqueNameColumn(InternalObjectType objectType)
		{
			SchemaRowsetCacheData schemaRowsetCacheData = CubeMetadataCache.SchemaRowsetsData[CubeMetadataCache.GetIndexForInternalType(objectType)];
			return schemaRowsetCacheData.UniqueNameColumnName;
		}

		private ObjectMetadataCache GetObjectCache(InternalObjectType objectType)
		{
			return this.objectMetadataCaches[CubeMetadataCache.GetIndexForInternalType(objectType)];
		}

		private static bool IsSupportedInternalType(InternalObjectType objectType)
		{
			return CubeMetadataCache.internalTypeMap.ContainsKey(objectType);
		}

		private static void AddSpecificRestrictions(SchemaRowsetCacheData data, ListDictionary restrictions)
		{
			if (data.AdditionalStaticRestrictions != null)
			{
				KeyValuePair<string, string>[] additionalStaticRestrictions = data.AdditionalStaticRestrictions;
				for (int i = 0; i < additionalStaticRestrictions.Length; i++)
				{
					KeyValuePair<string, string> keyValuePair = additionalStaticRestrictions[i];
					restrictions[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}
	}
}
