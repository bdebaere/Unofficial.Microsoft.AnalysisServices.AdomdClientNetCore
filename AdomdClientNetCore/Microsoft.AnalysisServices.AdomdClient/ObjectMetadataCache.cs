using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class ObjectMetadataCache : IObjectCache
	{
		private const string objectColumn = "ObjectColumn_ADOMDInternal$$";

		private DataTable cacheTable;

		private DateTime lastCacheValidationTime;

		private DataTable cacheValidationTable;

		private string[] cacheValidationColumnNames;

		private static readonly string[] columnNamesForCubeValidation = new string[]
		{
			CubeDef.cubeNameColumn,
			CubeDef.lastSchemaUpdateColumn
		};

		private bool isInitialized;

		private MetadataCacheState cacheState;

		private string requestType;

		private IMetadataCache metadataCache;

		private AdomdConnection connection;

		private ListDictionary restrictions;

		private string relationColumn;

		private DataRelation relation;

		private InternalObjectType objectType;

		private bool isNestedSchema;

		private DataSet cacheDataSet;

		private AdomdUtils.GetInvalidatedMessageDelegate msgDelegate;

		internal bool IsInitialized
		{
			get
			{
				return this.isInitialized;
			}
		}

		internal bool IsPopulated
		{
			get
			{
				return this.cacheState != MetadataCacheState.Empty;
			}
		}

		internal string RelationColumn
		{
			get
			{
				return this.relationColumn;
			}
		}

		internal DataRelation Relation
		{
			get
			{
				return this.relation;
			}
			set
			{
				this.relation = value;
			}
		}

		internal DataTable CacheTable
		{
			get
			{
				return this.cacheTable;
			}
		}

		bool IObjectCache.IsPopulated
		{
			get
			{
				return this.IsPopulated;
			}
		}

		DataSet IObjectCache.CacheDataSet
		{
			get
			{
				return this.cacheDataSet;
			}
		}

		internal ObjectMetadataCache(AdomdConnection connection, InternalObjectType objectType, string requestType, ListDictionary restrictions) : this(connection, objectType, requestType, restrictions, false)
		{
		}

		internal ObjectMetadataCache(AdomdConnection connection, InternalObjectType objectType, string requestType, ListDictionary restrictions, bool isNestedSchema) : this(null, connection, objectType, requestType, restrictions, null)
		{
			this.isNestedSchema = isNestedSchema;
		}

		internal ObjectMetadataCache(IMetadataCache metadataCache, AdomdConnection connection, SchemaRowsetCacheData rowsetData, ListDictionary restrictions) : this(metadataCache, connection, (rowsetData == null) ? InternalObjectType.InternalTypeMemberProperty : rowsetData.ObjectType, (rowsetData == null) ? null : rowsetData.RequestType, restrictions, (rowsetData == null) ? null : rowsetData.RelationColumnName)
		{
		}

		internal ObjectMetadataCache(IMetadataCache metadataCache, AdomdConnection connection, InternalObjectType objectType, string requestType, ListDictionary restrictions, string relationColumn)
		{
			this.msgDelegate = new AdomdUtils.GetInvalidatedMessageDelegate(this.GetMessageForCacheExpiration);
			this.isInitialized = false;
			this.cacheState = MetadataCacheState.Empty;
			this.metadataCache = metadataCache;
			this.connection = connection;
			this.objectType = objectType;
			this.requestType = requestType;
			this.restrictions = restrictions;
			this.relationColumn = relationColumn;
		}

		internal void PopulateSelf()
		{
			if (this.connection == null)
			{
				throw new NotSupportedException(SR.NotSupportedWhenConnectionMissing);
			}
			AdomdUtils.CheckConnectionOpened(this.connection);
			if (!this.IsInitialized)
			{
				this.cacheTable = new DataTable();
				this.cacheTable.Locale = CultureInfo.InvariantCulture;
				this.cacheTable.Columns.Add("ObjectColumn_ADOMDInternal$$", typeof(object));
			}
			if (this.isNestedSchema)
			{
				ObjectMetadataCache.DiscoverNested(this.connection, this.requestType, this.restrictions, out this.cacheDataSet);
				this.cacheTable.Columns.Add("__RowIndex__", typeof(int));
				object[] array = new object[2];
				for (int i = 0; i < this.cacheDataSet.Tables[0].Rows.Count; i++)
				{
					array[0] = null;
					array[1] = i;
					this.cacheTable.Rows.Add(array);
				}
			}
			else
			{
				ObjectMetadataCache.Discover(this.connection, this.requestType, this.restrictions, this.cacheTable, !this.isInitialized);
			}
			this.cacheState = MetadataCacheState.UpToDate;
			this.isInitialized = true;
		}

		internal void RefreshSelf()
		{
			if (this.cacheState != MetadataCacheState.Empty)
			{
				if (this.cacheTable != null)
				{
					DataTable dataTable = this.cacheTable.Clone();
					this.cacheTable = dataTable;
				}
				this.cacheState = MetadataCacheState.Empty;
			}
		}

		internal void MarkAbandonedSelf()
		{
			this.cacheState = MetadataCacheState.Abandoned;
		}

		internal static void Discover(AdomdConnection connection, string requestType, ListDictionary restrictions, DataTable destinationTable, bool doCreate)
		{
			if (!doCreate)
			{
				connection.IDiscoverProvider.DiscoverData(requestType, restrictions, destinationTable);
				return;
			}
			connection.IDiscoverProvider.Discover(requestType, restrictions, destinationTable);
		}

		internal static void DiscoverNested(AdomdConnection connection, string requestType, ListDictionary restrictions, out DataSet destinationDS)
		{
			RowsetFormatter rowsetFormatter = connection.IDiscoverProvider.Discover(requestType, restrictions);
			destinationDS = rowsetFormatter.RowsetDataset;
		}

		void IObjectCache.Populate()
		{
			if (!this.IsPopulated)
			{
				if (this.metadataCache != null)
				{
					this.metadataCache.Populate(this.objectType);
					return;
				}
				this.EnsureNotAbandoned();
				this.EnsureValid();
				this.PopulateSelf();
			}
		}

		void IObjectCache.Refresh()
		{
			if (this.IsPopulated)
			{
				if (this.metadataCache != null)
				{
					this.metadataCache.Refresh(this.objectType);
					return;
				}
				this.EnsureNotAbandoned();
				this.EnsureValid();
				this.RefreshSelf();
			}
		}

		DataRowCollection IObjectCache.GetNonFilteredRows()
		{
			return this.cacheTable.Rows;
		}

		DataRow[] IObjectCache.GetFilteredRows(DataRow parentRow, string filter)
		{
			if (filter == null && this.Relation != null && parentRow != null)
			{
				return parentRow.GetChildRows(this.Relation);
			}
			return this.GetRows(parentRow, filter);
		}

		void IObjectCache.CheckCacheIsValid()
		{
			if (this.IsPopulated)
			{
				if (this.metadataCache != null)
				{
					this.metadataCache.CheckCacheIsValid();
					return;
				}
				this.EnsureNotAbandoned();
				this.EnsureValid();
				if (this.NeedCheckForRefresh())
				{
					bool flag = true;
					if (this.cacheValidationTable == null)
					{
						this.cacheValidationTable = this.cacheTable.Clone();
						this.cacheValidationColumnNames = ObjectMetadataCache.GetColumnNamesToCompareForUpdate(this.objectType);
					}
					else
					{
						this.cacheValidationTable.Clear();
					}
					this.Discover(this.cacheValidationTable, false);
					if (this.cacheTable.Rows.Count != this.cacheValidationTable.Rows.Count)
					{
						flag = false;
					}
					else if (this.cacheValidationColumnNames != null)
					{
						for (int i = 0; i < this.cacheValidationTable.Rows.Count; i++)
						{
							DataRow dataRow = this.cacheTable.Rows[i];
							string[] array = this.cacheValidationColumnNames;
							for (int j = 0; j < array.Length; j++)
							{
								string columnName = array[j];
								if (!object.Equals(dataRow[columnName], this.cacheValidationTable.Rows[i][columnName]))
								{
									flag = false;
									break;
								}
							}
							if (!flag)
							{
								break;
							}
						}
					}
					this.lastCacheValidationTime = DateTime.Now;
					if (!flag)
					{
						this.cacheState = MetadataCacheState.Invalid;
					}
					else
					{
						this.cacheState = MetadataCacheState.UpToDate;
					}
					this.EnsureValid();
				}
			}
		}

		void IObjectCache.MarkNeedCheckForValidness()
		{
			if (this.metadataCache != null)
			{
				this.metadataCache.MarkNeedCheckForValidness();
				return;
			}
			if (this.cacheState == MetadataCacheState.UpToDate)
			{
				this.cacheState = MetadataCacheState.NeedsValidnessCheck;
			}
		}

		void IObjectCache.MarkAbandoned()
		{
			if (this.metadataCache != null)
			{
				this.metadataCache.MarkAbandoned();
				return;
			}
			this.cacheState = MetadataCacheState.Abandoned;
		}

		private void EnsureValid()
		{
			AdomdUtils.EnsureCacheNotInvalid(this.cacheState, this.msgDelegate);
		}

		private void EnsureNotAbandoned()
		{
			AdomdUtils.EnsureCacheNotAbandoned(this.cacheState);
		}

		private void Discover(DataTable destinationTable, bool doCreate)
		{
			ObjectMetadataCache.Discover(this.connection, this.requestType, this.restrictions, destinationTable, doCreate);
		}

		private DataRow[] GetRows(DataRow parentRow, string filter)
		{
			string text;
			if (parentRow != null)
			{
				text = AdomdUtils.GetDataTableFilter(this.relationColumn, parentRow[this.relationColumn].ToString());
				if (filter != null)
				{
					text = string.Concat(new string[]
					{
						"(",
						text,
						" and ",
						filter,
						")"
					});
				}
			}
			else
			{
				text = filter;
			}
			return this.cacheTable.Select(text);
		}

		private bool NeedCheckForRefresh()
		{
			return this.cacheState == MetadataCacheState.NeedsValidnessCheck || this.connection.HasAutoSyncTimeElapsed(this.lastCacheValidationTime, DateTime.Now);
		}

		private static string[] GetColumnNamesToCompareForUpdate(InternalObjectType objectType)
		{
			if (objectType == InternalObjectType.InternalTypeCube)
			{
				return ObjectMetadataCache.columnNamesForCubeValidation;
			}
			return null;
		}

		private string GetMessageForCacheExpiration()
		{
			InternalObjectType internalObjectType = this.objectType;
			if (internalObjectType == InternalObjectType.InternalTypeMiningService)
			{
				return SR.Metadata_MiningServicesCollectionHasbeenUpdated;
			}
			if (internalObjectType == InternalObjectType.InternalTypeCube)
			{
				return SR.Metadata_CubesCollectionHasbeenUpdated;
			}
			return null;
		}
	}
}
