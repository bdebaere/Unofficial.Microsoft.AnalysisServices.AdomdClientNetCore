using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class HierarchyCollectionInternal : CacheBasedFilteredCollection
	{
		private const string attrbHierRest = "HIERARCHY_ORIGIN";

		private const string attributeHierarchyFilterExpression = "( ((HIERARCHY_ORIGIN % 2) = 0) AND ( ( Convert(HIERARCHY_ORIGIN/ 2, 'System.Int32') % 2 ) = 1 ))";

		internal static string schemaName = "MDSCHEMA_HIERARCHIES";

		internal static string hierUNameRest = "HIERARCHY_UNIQUE_NAME";

		internal AdomdConnection connection;

		internal IDSFDataSet internalTableCollection;

		private Set axis;

		private Dimension parentDimension;

		private string cubeName;

		public Hierarchy this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (this.IsMetadata)
				{
					DataRow row = this.internalCollection[index];
					return HierarchyCollectionInternal.GetHiearchyByRow(this.connection, row, this.parentDimension, base.Catalog, base.SessionId);
				}
				DataTable hierarchyTable = this.axis.AxisDataset[index];
				return new Hierarchy(this.connection, hierarchyTable, this.cubeName, this.axis.ParentAxis, index);
			}
		}

		public Hierarchy this[string index]
		{
			get
			{
				Hierarchy hierarchy = this.Find(index);
				if (null == hierarchy)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return hierarchy;
			}
		}

		public override int Count
		{
			get
			{
				if (this.IsMetadata)
				{
					return base.Count;
				}
				return this.internalTableCollection.Count;
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
				if (this.IsMetadata)
				{
					return base.SyncRoot;
				}
				return this.internalTableCollection.SyncRoot;
			}
		}

		private bool IsMetadata
		{
			get
			{
				return this.axis == null;
			}
		}

		internal HierarchyCollectionInternal(AdomdConnection connection, Set axis, string cubeName) : base(connection)
		{
			this.connection = connection;
			this.axis = axis;
			this.cubeName = cubeName;
			this.internalTableCollection = axis.AxisDataset;
		}

		internal HierarchyCollectionInternal(AdomdConnection connection, Dimension parentDimension, bool isAttribute) : base(connection, InternalObjectType.InternalTypeHierarchy, parentDimension.ParentCube.metadataCache)
		{
			this.connection = connection;
			this.parentDimension = parentDimension;
			string filter = null;
			if (isAttribute)
			{
				filter = "( ((HIERARCHY_ORIGIN % 2) = 0) AND ( ( Convert(HIERARCHY_ORIGIN/ 2, 'System.Int32') % 2 ) = 1 ))";
			}
			base.Initialize((DataRow)((IAdomdBaseObject)parentDimension).MetadataData, filter);
		}

		public Hierarchy Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			if (this.IsMetadata)
			{
				DataRow dataRow = base.FindObjectByName(index, (DataRow)((IAdomdBaseObject)this.parentDimension).MetadataData, Hierarchy.hierarchyNameColumn);
				if (dataRow == null && index == this.parentDimension.Name)
				{
					dataRow = base.FindObjectByName(null, (DataRow)((IAdomdBaseObject)this.parentDimension).MetadataData, Hierarchy.hierarchyNameColumn);
				}
				if (dataRow == null)
				{
					return null;
				}
				return HierarchyCollectionInternal.GetHiearchyByRow(this.connection, dataRow, this.parentDimension, base.Catalog, base.SessionId);
			}
			else
			{
				if (this.Count == 0)
				{
					return null;
				}
				if (this.parentDimension == null)
				{
					this.parentDimension = this[0].ParentDimension;
				}
				Hierarchy hierarchy = this.parentDimension.hierarchies.Find(index);
				if (hierarchy != null && this.axis.AxisDataset.Contains(hierarchy.UniqueName))
				{
					return hierarchy;
				}
				return null;
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return new HierarchiesEnumerator(this);
		}

		public override void CopyTo(Array array, int index)
		{
			if (this.IsMetadata)
			{
				base.CopyTo(array, index);
				return;
			}
			this.internalTableCollection.CopyTo(array, index);
		}

		internal static Hierarchy GetHiearchyByRow(AdomdConnection connection, DataRow row, Dimension parentDimension, string catalog, string sessionId)
		{
			Hierarchy hierarchy;
			if (row[0] is DBNull)
			{
				hierarchy = new Hierarchy(connection, row, parentDimension, catalog, sessionId);
				row[0] = hierarchy;
			}
			else
			{
				hierarchy = (Hierarchy)row[0];
			}
			return hierarchy;
		}
	}
}
