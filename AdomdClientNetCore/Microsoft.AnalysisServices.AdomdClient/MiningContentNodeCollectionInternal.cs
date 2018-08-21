using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningContentNodeCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "DMSCHEMA_MINING_MODEL_CONTENT";

		internal static string modelNameRest = "MODEL_NAME";

		internal static string treeOperationRest = "TREE_OPERATION";

		internal static string nodeTypeRest = "NODE_TYPE";

		internal static string nodeUniqueNameRest = "NODE_UNIQUE_NAME";

		private MiningModel parentMiningModel;

		private MiningContentNode parentNode;

		private MiningNodeTreeOpType operation = MiningNodeTreeOpType.TreeopSelf;

		public MiningContentNode this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				MiningContentNode miningContentNodeByRow = MiningContentNodeCollectionInternal.GetMiningContentNodeByRow(this.nestedDataset, base.Connection, row, this.parentMiningModel, base.Catalog, base.SessionId);
				if (miningContentNodeByRow != null && this.operation == MiningNodeTreeOpType.TreeopChildren)
				{
					miningContentNodeByRow.SetParentNode(this.parentNode);
				}
				return miningContentNodeByRow;
			}
		}

		public MiningContentNode this[string index]
		{
			get
			{
				MiningContentNode miningContentNode = this.Find(index);
				if (null == miningContentNode)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return miningContentNode;
			}
		}

		internal MiningContentNodeCollectionInternal(AdomdConnection connection, MiningModel parentMiningModel) : base(connection)
		{
			this.parentMiningModel = parentMiningModel;
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add(MiningContentNodeCollectionInternal.modelNameRest, parentMiningModel.Name);
			listDictionary.Add(MiningContentNodeCollectionInternal.nodeTypeRest, 1);
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningContentNode, MiningContentNodeCollectionInternal.schemaName, listDictionary, true);
			base.Initialize(objectCache);
		}

		internal MiningContentNodeCollectionInternal(AdomdConnection connection, MiningModel parentMiningModel, string nodeUniqueName) : base(connection)
		{
			this.parentMiningModel = parentMiningModel;
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add(MiningContentNodeCollectionInternal.modelNameRest, parentMiningModel.Name);
			listDictionary.Add(MiningContentNodeCollectionInternal.nodeUniqueNameRest, nodeUniqueName);
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningContentNode, MiningContentNodeCollectionInternal.schemaName, listDictionary, true);
			base.Initialize(objectCache);
		}

		internal MiningContentNodeCollectionInternal(AdomdConnection connection, MiningContentNode parentNode, MiningNodeTreeOpType operation) : base(connection)
		{
			this.parentNode = parentNode;
			this.parentMiningModel = parentNode.ParentMiningModel;
			this.operation = operation;
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add(MiningContentNodeCollectionInternal.modelNameRest, this.parentMiningModel.Name);
			listDictionary.Add(MiningContentNodeCollectionInternal.nodeUniqueNameRest, parentNode.UniqueName);
			listDictionary.Add(MiningContentNodeCollectionInternal.treeOperationRest, (int)operation);
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningContentNode, MiningContentNodeCollectionInternal.schemaName, listDictionary, true);
			base.Initialize(objectCache);
		}

		public MiningContentNode Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, MiningContentNode.miningContentNodeNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			MiningContentNode miningContentNodeByRow = MiningContentNodeCollectionInternal.GetMiningContentNodeByRow(this.nestedDataset, base.Connection, dataRow, this.parentMiningModel, base.Catalog, base.SessionId);
			if (miningContentNodeByRow != null && this.operation == MiningNodeTreeOpType.TreeopChildren)
			{
				miningContentNodeByRow.SetParentNode(this.parentNode);
			}
			return miningContentNodeByRow;
		}

		public override IEnumerator GetEnumerator()
		{
			return new MiningContentNodesEnumerator(this);
		}

		internal static MiningContentNode GetMiningContentNodeByRow(DataSet dataSet, AdomdConnection connection, DataRow row, MiningModel parentMiningModel, string catalog, string sessionId)
		{
			MiningContentNode miningContentNode;
			if (row[0] is DBNull)
			{
				DataTable dataTable = dataSet.Tables[0];
				DataRowCollection rows = dataTable.Rows;
				int index = (int)row[1];
				DataRow miningContentNodeRow = rows[index];
				miningContentNode = new MiningContentNode(connection, miningContentNodeRow, parentMiningModel, catalog, sessionId);
				row[0] = miningContentNode;
			}
			else
			{
				miningContentNode = (MiningContentNode)row[0];
			}
			return miningContentNode;
		}

		internal override void CheckCache()
		{
		}
	}
}
