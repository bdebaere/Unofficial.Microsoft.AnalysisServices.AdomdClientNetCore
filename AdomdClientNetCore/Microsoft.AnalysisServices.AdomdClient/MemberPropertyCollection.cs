using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MemberPropertyCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private MemberPropertyCollection memberProperties;

			public MemberProperty Current
			{
				get
				{
					MemberProperty result;
					try
					{
						result = this.memberProperties[this.currentIndex];
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

			internal Enumerator(MemberPropertyCollection memberProperties)
			{
				this.memberProperties = memberProperties;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.memberProperties.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private const int STANDARD_PROP_OFFSET = 5;

		private const string namesHashtablePropertyName = "MemberPropertiesNamesHash";

		private int firstMemberPropertyOffset;

		private DataRow memberAxisRow;

		private Member parentMember;

		private Collection<int> indexMap;

		private Hashtable namesHash;

		public MemberProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				int index2 = index + this.firstMemberPropertyOffset;
				return new MemberProperty(this.memberAxisRow, this.indexMap[index2], this.parentMember);
			}
		}

		public MemberProperty this[string index]
		{
			get
			{
				MemberProperty memberProperty = this.Find(index);
				if (null == memberProperty)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return memberProperty;
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
				if (this.memberAxisRow == null)
				{
					return this;
				}
				return this.memberAxisRow.Table.Columns.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				if (this.memberAxisRow == null)
				{
					return 0;
				}
				return Math.Max(this.indexMap.Count - this.firstMemberPropertyOffset, 0);
			}
		}

		internal MemberPropertyCollection(DataRow memberAxisRow, Member parentMember, int internallyAddedDimensionPropertyCount)
		{
			if (parentMember == null)
			{
				throw new ArgumentNullException("parentMember");
			}
			if (memberAxisRow == null)
			{
				this.indexMap = new Collection<int>();
			}
			else
			{
				this.indexMap = (memberAxisRow.Table.ExtendedProperties["MemberProperties"] as Collection<int>);
			}
			this.memberAxisRow = memberAxisRow;
			this.parentMember = parentMember;
			this.firstMemberPropertyOffset = 5 + internallyAddedDimensionPropertyCount;
			if (this.memberAxisRow == null)
			{
				this.namesHash = new Hashtable();
				return;
			}
			this.namesHash = MemberPropertyCollection.GetOrCreateNamesHashtable(this.memberAxisRow.Table, internallyAddedDimensionPropertyCount);
		}

		public MemberProperty Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			int propertyColumnIndex = this.GetPropertyColumnIndex(index);
			if (-1 == propertyColumnIndex)
			{
				return null;
			}
			return new MemberProperty(this.memberAxisRow, propertyColumnIndex, this.parentMember);
		}

		private int GetPropertyColumnIndex(string propName)
		{
			if (this.namesHash[propName] is int)
			{
				return (int)this.namesHash[propName];
			}
			return -1;
		}

		private static Hashtable GetNamesHash(DataTable table, int firstPropertyOffSet)
		{
			Hashtable hashtable = new Hashtable();
			if (table == null)
			{
				return hashtable;
			}
			AdomdUtils.FillPropertiesNamesHashTable(table, hashtable, firstPropertyOffSet);
			return hashtable;
		}

		internal static Hashtable GetOrCreateNamesHashtable(DataTable table, int internallyAddedPropCount)
		{
			if (table.ExtendedProperties["MemberPropertiesNamesHash"] is Hashtable)
			{
				return table.ExtendedProperties["MemberPropertiesNamesHash"] as Hashtable;
			}
			Hashtable hashtable = MemberPropertyCollection.GetNamesHash(table, 5 + internallyAddedPropCount);
			table.ExtendedProperties["MemberPropertiesNamesHash"] = hashtable;
			return hashtable;
		}

		public void CopyTo(MemberProperty[] array, int index)
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

		public MemberPropertyCollection.Enumerator GetEnumerator()
		{
			return new MemberPropertyCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
