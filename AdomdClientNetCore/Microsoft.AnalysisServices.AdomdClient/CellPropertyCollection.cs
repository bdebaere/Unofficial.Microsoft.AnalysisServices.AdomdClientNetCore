using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CellPropertyCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private CellPropertyCollection cellProps;

			public CellProperty Current
			{
				get
				{
					CellProperty result;
					try
					{
						result = this.cellProps[this.currentIndex];
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

			internal Enumerator(CellPropertyCollection cellProps)
			{
				this.cellProps = cellProps;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.cellProps.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private const string namesHashtablePropertyName = "CellPropertiesNamesHash";

		private DataColumnCollection internalCollection;

		private Hashtable namesHash;

		private DataTable cellTable;

		private DataRow cellRow;

		private Cell parentCell;

		private Collection<int> indexMap;

		public CellProperty this[int index]
		{
			get
			{
				if (index < 0 && index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return new CellProperty(this.cellTable, this.cellRow, this.indexMap[index], this.parentCell);
			}
		}

		public CellProperty this[string propertyName]
		{
			get
			{
				CellProperty cellProperty = this.Find(propertyName);
				if (null == cellProperty)
				{
					throw new ArgumentException(SR.Cellset_propertyIsUnknown(propertyName), "propertyName");
				}
				return cellProperty;
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
				return this.internalCollection.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				if (this.internalCollection != null)
				{
					return this.indexMap.Count;
				}
				return 0;
			}
		}

		internal CellPropertyCollection(DataTable cellTable, DataRow cellRow, Cell parentCell)
		{
			this.parentCell = parentCell;
			this.cellTable = cellTable;
			this.cellRow = cellRow;
			this.internalCollection = cellTable.Columns;
			this.indexMap = (cellTable.ExtendedProperties["MemberProperties"] as Collection<int>);
			if (this.cellTable.ExtendedProperties["CellPropertiesNamesHash"] is Hashtable)
			{
				this.namesHash = (this.cellTable.ExtendedProperties["CellPropertiesNamesHash"] as Hashtable);
				return;
			}
			this.namesHash = CellPropertyCollection.GetNamesHash(this.cellTable);
			this.cellTable.ExtendedProperties["CellPropertiesNamesHash"] = this.namesHash;
		}

		public CellProperty Find(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!this.namesHash.ContainsKey(name))
			{
				return null;
			}
			int propOrdinal = (int)this.namesHash[name];
			return new CellProperty(this.cellTable, this.cellRow, propOrdinal, this.parentCell);
		}

		public void CopyTo(CellProperty[] array, int index)
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public CellPropertyCollection.Enumerator GetEnumerator()
		{
			return new CellPropertyCollection.Enumerator(this);
		}

		private static Hashtable GetNamesHash(DataTable table)
		{
			Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase);
			if (table == null)
			{
				return hashtable;
			}
			AdomdUtils.FillNamesHashTable(table, hashtable);
			return hashtable;
		}
	}
}
