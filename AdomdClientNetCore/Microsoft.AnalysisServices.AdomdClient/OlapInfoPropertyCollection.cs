using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoPropertyCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private OlapInfoPropertyCollection properties;

			public OlapInfoProperty Current
			{
				get
				{
					OlapInfoProperty result;
					try
					{
						result = this.properties[this.currentIndex];
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

			internal Enumerator(OlapInfoPropertyCollection properties)
			{
				this.properties = properties;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.properties.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private DataTable propertiesDataTable;

		private OlapInfoProperty[] propertiesCach;

		private Hashtable namesHash;

		private Collection<int> indexMap;

		public OlapInfoProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (this.propertiesCach[index] == null)
				{
					this.propertiesCach[index] = new OlapInfoProperty(this.propertiesDataTable.Columns[this.indexMap[index]]);
				}
				return this.propertiesCach[index];
			}
		}

		public OlapInfoProperty this[string name]
		{
			get
			{
				OlapInfoProperty olapInfoProperty = this.Find(name);
				if (olapInfoProperty == null)
				{
					throw new ArgumentException(SR.ICollection_ItemWithThisNameDoesNotExistInTheCollection, "name");
				}
				return olapInfoProperty;
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
				return this.propertiesCach.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.propertiesCach.Length;
			}
		}

		internal OlapInfoPropertyCollection(DataTable propertiesDataTable)
		{
			this.propertiesDataTable = propertiesDataTable;
			this.indexMap = (propertiesDataTable.ExtendedProperties["MemberProperties"] as Collection<int>);
			this.propertiesCach = new OlapInfoProperty[this.indexMap.Count];
			for (int i = 0; i < this.propertiesCach.Length; i++)
			{
				this.propertiesCach[i] = null;
			}
			if (this.propertiesDataTable.ExtendedProperties["MemberPropertiesNamesHash"] is Hashtable)
			{
				this.namesHash = (this.propertiesDataTable.ExtendedProperties["MemberPropertiesNamesHash"] as Hashtable);
				return;
			}
			this.namesHash = OlapInfoPropertyCollection.GetNamesHash(this.propertiesDataTable);
			this.propertiesDataTable.ExtendedProperties["MemberPropertiesNamesHash"] = this.namesHash;
		}

		public OlapInfoProperty Find(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!this.namesHash.ContainsKey(name))
			{
				return null;
			}
			int index = (int)this.namesHash[name];
			return this[index];
		}

		private static Hashtable GetNamesHash(DataTable table)
		{
			Hashtable hashtable = new Hashtable();
			if (table == null)
			{
				return hashtable;
			}
			AdomdUtils.FillPropertiesNamesHashTable(table, hashtable, 0);
			return hashtable;
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

		public OlapInfoPropertyCollection.Enumerator GetEnumerator()
		{
			return new OlapInfoPropertyCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
