using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningStructureColumnCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "DMSCHEMA_MINING_STRUCTURE_COLUMNS";

		internal static string columnNameRest = "COLUMN_NAME";

		internal static string structureNameRest = "STRUCTURE_NAME";

		private IAdomdBaseObject parentObject;

		public MiningStructureColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return MiningStructureColumnCollectionInternal.GetMiningStructureColumnByRow(base.Connection, row, this.parentObject, base.Catalog, base.SessionId);
			}
		}

		public MiningStructureColumn this[string index]
		{
			get
			{
				MiningStructureColumn miningStructureColumn = this.Find(index);
				if (null == miningStructureColumn)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return miningStructureColumn;
			}
		}

		internal MiningStructureColumnCollectionInternal(AdomdConnection connection, MiningStructure parentStructure) : base(connection)
		{
			string name = parentStructure.Name;
			this.parentObject = parentStructure;
			this.InternalConstructor(connection, name);
		}

		internal MiningStructureColumnCollectionInternal(AdomdConnection connection, MiningStructureColumn parentColumn) : base(connection)
		{
			string name = parentColumn.ParentMiningStructure.Name;
			this.parentObject = parentColumn;
			this.InternalConstructor(connection, name);
		}

		private void InternalConstructor(AdomdConnection connection, string parentStructureName)
		{
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add(MiningStructureColumnCollectionInternal.structureNameRest, parentStructureName);
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningStructureColumn, MiningStructureColumnCollectionInternal.schemaName, listDictionary);
			base.Initialize(objectCache);
		}

		public MiningStructureColumn Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, MiningStructureColumn.miningStructureColumnNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return MiningStructureColumnCollectionInternal.GetMiningStructureColumnByRow(base.Connection, dataRow, this.parentObject, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new MiningStructureColumnsEnumerator(this);
		}

		internal static MiningStructureColumn GetMiningStructureColumnByRow(AdomdConnection connection, DataRow row, IAdomdBaseObject parentObject, string catalog, string sessionId)
		{
			MiningStructureColumn miningStructureColumn;
			if (row[0] is DBNull)
			{
				miningStructureColumn = new MiningStructureColumn(connection, row, parentObject, catalog, sessionId);
				row[0] = miningStructureColumn;
			}
			else
			{
				miningStructureColumn = (MiningStructureColumn)row[0];
			}
			return miningStructureColumn;
		}

		protected override void PopulateCollection()
		{
			if (!this.isPopulated)
			{
				base.PopulateCollection();
				string b = "";
				if (this.parentObject is MiningStructure)
				{
					b = "";
				}
				else if (this.parentObject is MiningStructureColumn)
				{
					b = ((MiningStructureColumn)this.parentObject).Name;
				}
				int i = 0;
				while (i < this.Count)
				{
					MiningStructureColumn miningStructureColumn = this[i];
					if (miningStructureColumn.ContainingColumn != b)
					{
						this.internalCollection.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		internal override void CheckCache()
		{
		}
	}
}
