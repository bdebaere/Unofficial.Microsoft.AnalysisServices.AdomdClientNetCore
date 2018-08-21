using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoHierarchyCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private OlapInfoHierarchyCollection hierarchies;

			public OlapInfoHierarchy Current
			{
				get
				{
					OlapInfoHierarchy result;
					try
					{
						result = this.hierarchies[this.currentIndex];
					}
					catch (ArgumentException)
					{
						throw new InvalidOperationException();
					}
					return result;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(OlapInfoHierarchyCollection hierarchies)
			{
				this.hierarchies = hierarchies;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.hierarchies.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private const string cachedObjectPropertyName = "$CachedOlapInfoHierarchy";

		private IDSFDataSet hierarchiesDataSet;

		private Hashtable namesHash;

		public OlapInfoHierarchy this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataTable dataTable = this.hierarchiesDataSet[index];
				if (dataTable.ExtendedProperties["$CachedOlapInfoHierarchy"] == null)
				{
					dataTable.ExtendedProperties["$CachedOlapInfoHierarchy"] = new OlapInfoHierarchy(dataTable);
				}
				return (OlapInfoHierarchy)dataTable.ExtendedProperties["$CachedOlapInfoHierarchy"];
			}
		}

		public OlapInfoHierarchy this[string name]
		{
			get
			{
				OlapInfoHierarchy olapInfoHierarchy = this.Find(name);
				if (olapInfoHierarchy == null)
				{
					throw new ArgumentException(SR.ICollection_ItemWithThisNameDoesNotExistInTheCollection, "name");
				}
				return olapInfoHierarchy;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.hierarchiesDataSet.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.hierarchiesDataSet.Count;
			}
		}

		internal OlapInfoHierarchyCollection(IDSFDataSet hierarchiesDataSet)
		{
			this.hierarchiesDataSet = hierarchiesDataSet;
		}

		public OlapInfoHierarchy Find(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.namesHash == null)
			{
				this.namesHash = new Hashtable(this.Count);
				for (int i = 0; i < this.hierarchiesDataSet.Count; i++)
				{
					string tableName = this.hierarchiesDataSet[i].TableName;
					if (this.namesHash[tableName] == null)
					{
						this.namesHash[tableName] = i;
					}
				}
			}
			if (!this.namesHash.ContainsKey(name))
			{
				return null;
			}
			int index = (int)this.namesHash[name];
			return this[index];
		}

		public void CopyTo(OlapInfoHierarchy[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			AdomdUtils.CheckCopyToParameters(array, index, this.Count);
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index + i);
			}
		}

		public OlapInfoHierarchyCollection.Enumerator GetEnumerator()
		{
			return new OlapInfoHierarchyCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
