using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningServiceCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "DMSCHEMA_MINING_SERVICES";

		internal static string miningServiceNameRest = "SERVICE_NAME";

		public MiningService this[int index]
		{
			get
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRowCollection internalCollection = this.internalCollection;
				DataRow row = internalCollection[index];
				return this.GetMiningServiceByRow(row);
			}
		}

		public MiningService this[string index]
		{
			get
			{
				MiningService miningService = this.Find(index);
				if (null == miningService)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return miningService;
			}
		}

		internal MiningServiceCollectionInternal(AdomdConnection connection) : base(connection)
		{
			ListDictionary restrictions = new ListDictionary();
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningService, MiningServiceCollectionInternal.schemaName, restrictions);
			base.Initialize(objectCache);
		}

		public MiningService Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, MiningService.miningServiceNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return this.GetMiningServiceByRow(dataRow);
		}

		private MiningService GetMiningServiceByRow(DataRow row)
		{
			MiningService miningService;
			if (row[0] is DBNull)
			{
				miningService = new MiningService(row, base.Connection, this.populatedTime, base.Catalog, base.SessionId);
				row[0] = miningService;
			}
			else
			{
				miningService = (MiningService)row[0];
			}
			return miningService;
		}

		public override IEnumerator GetEnumerator()
		{
			return new MiningServicesEnumerator(this);
		}
	}
}
