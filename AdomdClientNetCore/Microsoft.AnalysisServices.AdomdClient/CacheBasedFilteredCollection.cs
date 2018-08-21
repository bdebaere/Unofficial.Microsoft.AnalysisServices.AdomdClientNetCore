using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class CacheBasedFilteredCollection : CacheBasedCollection
	{
		protected DataRow[] internalCollection;

		private DataRow parentRow;

		private string filter;

		public override int Count
		{
			get
			{
				this.PopulateCollection();
				return this.internalCollection.Length;
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

		internal CacheBasedFilteredCollection(AdomdConnection connection, IObjectCache objectCache) : base(connection, objectCache)
		{
		}

		internal CacheBasedFilteredCollection(AdomdConnection connection, InternalObjectType objectType, IMetadataCache metadataCache) : this(connection, metadataCache.GetObjectCache(objectType))
		{
		}

		internal CacheBasedFilteredCollection(AdomdConnection connection) : base(connection, null)
		{
		}

		internal void Initialize(DataRow parentRow, string filter)
		{
			this.parentRow = parentRow;
			this.filter = filter;
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
				this.internalCollection = this.objectCache.GetFilteredRows(this.parentRow, this.filter);
			}
		}
	}
}
