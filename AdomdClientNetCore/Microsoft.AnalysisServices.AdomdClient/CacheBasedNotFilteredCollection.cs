using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class CacheBasedNotFilteredCollection : CacheBasedCollection
	{
		protected DataRowCollection internalCollection;

		protected DataSet nestedDataset;

		protected DateTime populatedTime;

		public override int Count
		{
			get
			{
				this.PopulateCollection();
				return this.internalCollection.Count;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public override object SyncRoot
		{
			get
			{
				this.PopulateCollection();
				return this.internalCollection.SyncRoot;
			}
		}

		protected override bool isPopulated
		{
			get
			{
				return base.isPopulated && this.internalCollection != null;
			}
		}

		internal CacheBasedNotFilteredCollection(AdomdConnection connection, InternalObjectType objectType, IMetadataCache metadataCache) : base(connection, objectType, metadataCache)
		{
		}

		internal CacheBasedNotFilteredCollection(AdomdConnection connection) : base(connection)
		{
		}

		public override void CopyTo(Array array, int index)
		{
			this.PopulateCollection();
			this.internalCollection.CopyTo(array, index);
		}

		protected override void PopulateCollection()
		{
			if (!this.isPopulated)
			{
				if (!base.isPopulated)
				{
					this.objectCache.Populate();
				}
				this.internalCollection = this.objectCache.GetNonFilteredRows();
				this.nestedDataset = this.objectCache.CacheDataSet;
				this.populatedTime = DateTime.Now;
			}
		}

		internal void Refresh()
		{
			this.objectCache.Refresh();
			this.PopulateCollection();
		}
	}
}
