using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningStructureCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "DMSCHEMA_MINING_STRUCTURES";

		internal static string miningStructureNameRest = "STRUCTURE_NAME";

		public MiningStructure this[int index]
		{
			get
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRowCollection internalCollection = this.internalCollection;
				DataRow row = internalCollection[index];
				return this.GetMiningStructureByRow(row);
			}
		}

		public MiningStructure this[string index]
		{
			get
			{
				MiningStructure miningStructure = this.Find(index);
				if (null == miningStructure)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return miningStructure;
			}
		}

		internal MiningStructureCollectionInternal(AdomdConnection connection) : base(connection)
		{
			ListDictionary restrictions = new ListDictionary();
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningStructure, MiningStructureCollectionInternal.schemaName, restrictions);
			base.Initialize(objectCache);
		}

		public MiningStructure Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, MiningStructure.miningStructureNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return this.GetMiningStructureByRow(dataRow);
		}

		private MiningStructure GetMiningStructureByRow(DataRow row)
		{
			MiningStructure miningStructure;
			if (row[0] is DBNull)
			{
				miningStructure = new MiningStructure(row, base.Connection, this.populatedTime, base.Catalog, base.SessionId);
				row[0] = miningStructure;
			}
			else
			{
				miningStructure = (MiningStructure)row[0];
			}
			return miningStructure;
		}

		public override IEnumerator GetEnumerator()
		{
			return new MiningStructuresEnumerator(this);
		}

		internal override void CheckCache()
		{
		}
	}
}
