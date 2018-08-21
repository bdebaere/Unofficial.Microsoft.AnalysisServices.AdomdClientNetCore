using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningModelCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "DMSCHEMA_MINING_MODELS";

		internal static string miningModelNameRest = "MODEL_NAME";

		internal static string structNameRest = "MINING_STRUCTURE";

		private MiningStructure parentObject;

		public MiningModel this[int index]
		{
			get
			{
				if (index < 0 || index >= base.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRowCollection internalCollection = this.internalCollection;
				DataRow row = internalCollection[index];
				return this.GetMiningModelByRow(row);
			}
		}

		public MiningModel this[string index]
		{
			get
			{
				MiningModel miningModel = this.Find(index);
				if (null == miningModel)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return miningModel;
			}
		}

		internal MiningModelCollectionInternal(AdomdConnection connection) : base(connection)
		{
			ListDictionary restrictions = new ListDictionary();
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningModel, MiningModelCollectionInternal.schemaName, restrictions);
			base.Initialize(objectCache);
		}

		internal MiningModelCollectionInternal(MiningStructure structure) : base(structure.ParentConnection)
		{
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add(MiningModelCollectionInternal.structNameRest, structure.Name);
			this.parentObject = structure;
			ObjectMetadataCache objectCache = new ObjectMetadataCache(structure.ParentConnection, InternalObjectType.InternalTypeMiningModel, MiningModelCollectionInternal.schemaName, listDictionary);
			base.Initialize(objectCache);
		}

		public MiningModel Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, MiningModel.miningModelNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return this.GetMiningModelByRow(dataRow);
		}

		private MiningModel GetMiningModelByRow(DataRow row)
		{
			MiningModel miningModel;
			if (row[0] is DBNull)
			{
				miningModel = new MiningModel(row, base.Connection, this.populatedTime, base.Catalog, base.SessionId, this.parentObject);
				row[0] = miningModel;
			}
			else
			{
				miningModel = (MiningModel)row[0];
			}
			return miningModel;
		}

		public override IEnumerator GetEnumerator()
		{
			return new MiningModelsEnumerator(this);
		}

		internal override void CheckCache()
		{
		}
	}
}
