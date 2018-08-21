using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class CubeCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "MDSCHEMA_CUBES";

		internal static string cubeNameRest = "CUBE_NAME";

		public CubeDef this[int index]
		{
			get
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRowCollection internalCollection = this.internalCollection;
				DataRow row = internalCollection[index];
				return this.GetCubeByRow(row);
			}
		}

		public CubeDef this[string index]
		{
			get
			{
				CubeDef cubeDef = this.Find(index);
				if (null == cubeDef)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return cubeDef;
			}
		}

		internal CubeCollectionInternal(AdomdConnection connection) : base(connection)
		{
			ListDictionary restrictions = new ListDictionary();
			AdomdUtils.AddCubeSourceRestrictionIfApplicable(connection, restrictions);
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeCube, CubeCollectionInternal.schemaName, restrictions);
			base.Initialize(objectCache);
		}

		public CubeDef Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, CubeDef.cubeNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return this.GetCubeByRow(dataRow);
		}

		private CubeDef GetCubeByRow(DataRow row)
		{
			CubeDef cubeDef;
			if (row[0] is DBNull)
			{
				cubeDef = new CubeDef(row, base.Connection, this.populatedTime, base.Catalog, base.SessionId);
				row[0] = cubeDef;
			}
			else
			{
				cubeDef = (CubeDef)row[0];
			}
			return cubeDef;
		}

		internal override void MarkCacheAsNeedCheckForValidness()
		{
			base.MarkCacheAsNeedCheckForValidness();
			if (this.isPopulated)
			{
				foreach (DataRow dataRow in this.internalCollection)
				{
					if (dataRow[0] is CubeDef)
					{
						CubeDef cubeDef = (CubeDef)dataRow[0];
						cubeDef.metadataCache.MarkNeedCheckForValidness();
					}
				}
			}
		}

		internal override void AbandonCache()
		{
			base.AbandonCache();
			if (this.isPopulated)
			{
				foreach (DataRow dataRow in this.internalCollection)
				{
					if (dataRow[0] is CubeDef)
					{
						CubeDef cubeDef = (CubeDef)dataRow[0];
						cubeDef.metadataCache.MarkAbandoned();
					}
				}
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return new CubesEnumerator(this);
		}
	}
}
