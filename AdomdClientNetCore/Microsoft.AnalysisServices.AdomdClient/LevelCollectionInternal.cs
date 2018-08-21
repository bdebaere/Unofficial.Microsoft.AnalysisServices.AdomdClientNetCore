using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class LevelCollectionInternal : CacheBasedFilteredCollection
	{
		internal static string schemaName = "MDSCHEMA_LEVELS";

		internal static string levelUNameRest = "LEVEL_UNIQUE_NAME";

		private Hierarchy parentHierarchy;

		public Level this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return LevelCollectionInternal.GetLevelByRow(base.Connection, row, this.parentHierarchy, base.Catalog, base.SessionId);
			}
		}

		public Level this[string index]
		{
			get
			{
				Level level = this.Find(index);
				if (null == level)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return level;
			}
		}

		internal LevelCollectionInternal(AdomdConnection connection, Hierarchy parentHierarchy) : base(connection, InternalObjectType.InternalTypeLevel, parentHierarchy.ParentDimension.ParentCube.metadataCache)
		{
			this.parentHierarchy = parentHierarchy;
			base.Initialize((DataRow)((IAdomdBaseObject)parentHierarchy).MetadataData, null);
		}

		public Level Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, (DataRow)((IAdomdBaseObject)this.parentHierarchy).MetadataData, Level.levelNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return LevelCollectionInternal.GetLevelByRow(base.Connection, dataRow, this.parentHierarchy, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new LevelsEnumerator(this);
		}

		internal static Level GetLevelByRow(AdomdConnection connection, DataRow row, Hierarchy parentHierarchy, string catalog, string sessionId)
		{
			Level level;
			if (row[0] is DBNull)
			{
				level = new Level(connection, row, parentHierarchy, catalog, sessionId);
				row[0] = level;
			}
			else
			{
				level = (Level)row[0];
			}
			return level;
		}
	}
}
