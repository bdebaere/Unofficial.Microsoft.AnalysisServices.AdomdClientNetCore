using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class CacheBasedCollection : ICollection, IEnumerable
	{
		protected IObjectCache objectCache;

		private string catalog;

		private string sessionId;

		private AdomdConnection connection;

		internal string Catalog
		{
			get
			{
				return this.catalog;
			}
		}

		internal string SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		protected AdomdConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		protected virtual bool isPopulated
		{
			get
			{
				return this.objectCache == null || this.objectCache.IsPopulated;
			}
		}

		public abstract object SyncRoot
		{
			get;
		}

		public abstract int Count
		{
			get;
		}

		public abstract bool IsSynchronized
		{
			get;
		}

		internal CacheBasedCollection(AdomdConnection connection, IObjectCache objectCache)
		{
			this.objectCache = objectCache;
			this.connection = connection;
			if (this.connection != null && this.connection.State == ConnectionState.Open)
			{
				this.catalog = this.connection.CatalogConnectionStringProperty;
				this.sessionId = this.connection.SessionID;
			}
		}

		internal CacheBasedCollection(AdomdConnection connection) : this(connection, null)
		{
		}

		internal CacheBasedCollection(AdomdConnection connection, InternalObjectType objectType, IMetadataCache metadataCache) : this(connection, metadataCache.GetObjectCache(objectType))
		{
		}

		protected void Initialize(IObjectCache objectCache)
		{
			this.objectCache = objectCache;
		}

		internal virtual void CheckCache()
		{
			this.objectCache.CheckCacheIsValid();
		}

		internal virtual void MarkCacheAsNeedCheckForValidness()
		{
			if (this.isPopulated && this.objectCache != null)
			{
				this.objectCache.MarkNeedCheckForValidness();
			}
		}

		public abstract void CopyTo(Array array, int index);

		public abstract IEnumerator GetEnumerator();

		protected abstract void PopulateCollection();

		internal DataRow FindObjectByName(string name, DataRow parentRow, string nameColumn)
		{
			this.PopulateCollection();
			string dataTableFilter = AdomdUtils.GetDataTableFilter(nameColumn, name);
			DataRow[] filteredRows = this.objectCache.GetFilteredRows(parentRow, dataTableFilter);
			if (filteredRows.Length <= 0)
			{
				return null;
			}
			return filteredRows[0];
		}

		internal virtual void AbandonCache()
		{
			if (this.objectCache != null)
			{
				this.objectCache.MarkAbandoned();
			}
		}
	}
}
