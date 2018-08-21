using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningModelColumnCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "DMSCHEMA_MINING_COLUMNS";

		internal static string columnNameRest = "COLUMN_NAME";

		internal static string modelNameRest = "MODEL_NAME";

		private IAdomdBaseObject parentObject;

		public MiningModelColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return MiningModelColumnCollectionInternal.GetMiningModelColumnByRow(base.Connection, row, this.parentObject, base.Catalog, base.SessionId);
			}
		}

		public MiningModelColumn this[string index]
		{
			get
			{
				MiningModelColumn miningModelColumn = this.Find(index);
				if (null == miningModelColumn)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return miningModelColumn;
			}
		}

		internal MiningModelColumnCollectionInternal(AdomdConnection connection, MiningModel parentModel) : base(connection)
		{
			string name = parentModel.Name;
			this.parentObject = parentModel;
			this.InternalConstructor(connection, name);
		}

		internal MiningModelColumnCollectionInternal(AdomdConnection connection, MiningModelColumn parentColumn) : base(connection)
		{
			string name = parentColumn.ParentMiningModel.Name;
			this.parentObject = parentColumn;
			this.InternalConstructor(connection, name);
		}

		private void InternalConstructor(AdomdConnection connection, string parentModelName)
		{
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add(MiningModelColumnCollectionInternal.modelNameRest, parentModelName);
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningModelColumn, MiningModelColumnCollectionInternal.schemaName, listDictionary);
			base.Initialize(objectCache);
		}

		public MiningModelColumn Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, MiningModelColumn.miningModelColumnNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return MiningModelColumnCollectionInternal.GetMiningModelColumnByRow(base.Connection, dataRow, this.parentObject, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new MiningModelColumnsEnumerator(this);
		}

		internal static MiningModelColumn GetMiningModelColumnByRow(AdomdConnection connection, DataRow row, IAdomdBaseObject parentObject, string catalog, string sessionId)
		{
			MiningModelColumn miningModelColumn;
			if (row[0] is DBNull)
			{
				miningModelColumn = new MiningModelColumn(connection, row, parentObject, catalog, sessionId);
				row[0] = miningModelColumn;
			}
			else
			{
				miningModelColumn = (MiningModelColumn)row[0];
			}
			return miningModelColumn;
		}

		protected override void PopulateCollection()
		{
			if (!this.isPopulated)
			{
				base.PopulateCollection();
				string b = "";
				if (this.parentObject is MiningModel)
				{
					b = "";
				}
				else if (this.parentObject is MiningModelColumn)
				{
					b = ((MiningModelColumn)this.parentObject).Name;
				}
				int i = 0;
				while (i < this.Count)
				{
					MiningModelColumn miningModelColumn = this[i];
					if (miningModelColumn.ContainingColumn != b)
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
