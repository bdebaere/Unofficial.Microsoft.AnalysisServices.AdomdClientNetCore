using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdPropertyCollection : MarshalByRefObject, IList, ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private AdomdPropertyCollection properties;

			public AdomdProperty Current
			{
				get
				{
					AdomdProperty result;
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

			internal Enumerator(AdomdPropertyCollection properties)
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

		private AdomdPropertyCollectionInternal collectionInternal;

		public bool IsSynchronized
		{
			get
			{
				return ((ICollection)this.collectionInternal).IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return ((ICollection)this.collectionInternal).SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.collectionInternal.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return ((IList)this.collectionInternal).IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((IList)this.collectionInternal).IsReadOnly;
			}
		}

		public AdomdProperty this[int index]
		{
			get
			{
				return (AdomdProperty)this.collectionInternal[index];
			}
			set
			{
				this.collectionInternal[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return ((IList)this.collectionInternal)[index];
			}
			set
			{
				((IList)this.collectionInternal)[index] = value;
			}
		}

		internal AdomdPropertyCollectionInternal InternalCollection
		{
			get
			{
				return this.collectionInternal;
			}
		}

		public AdomdPropertyCollection()
		{
			this.collectionInternal = new AdomdPropertyCollectionInternal(this);
		}

		public AdomdProperty Find(string propertyName)
		{
			return this.Find(propertyName, null);
		}

		public AdomdProperty Find(string propertyName, string propertyNamespace)
		{
			XmlaPropertyKey key = new XmlaPropertyKey(propertyName, propertyNamespace);
			int num = this.collectionInternal.IndexOf(key);
			if (num != -1)
			{
				return this[num];
			}
			return null;
		}

		public void CopyTo(AdomdProperty[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.collectionInternal).CopyTo(array, index);
		}

		public AdomdPropertyCollection.Enumerator GetEnumerator()
		{
			return new AdomdPropertyCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public AdomdProperty Add(AdomdProperty value)
		{
			return (AdomdProperty)this.collectionInternal.Add(value);
		}

		public AdomdProperty Add(string propertyName, object value)
		{
			return (AdomdProperty)this.collectionInternal.Add(new AdomdProperty(propertyName, value));
		}

		public AdomdProperty Add(string propertyName, string propertyNamespace, object value)
		{
			return (AdomdProperty)this.collectionInternal.Add(new AdomdProperty(propertyName, propertyNamespace, value));
		}

		int IList.Add(object value)
		{
			return ((IList)this.collectionInternal).Add(value);
		}

		public void Clear()
		{
			this.collectionInternal.Clear();
		}

		public bool Contains(AdomdProperty value)
		{
			return this.collectionInternal.Contains(value);
		}

		bool IList.Contains(object value)
		{
			return ((IList)this.collectionInternal).Contains(value);
		}

		public int IndexOf(AdomdProperty value)
		{
			return this.collectionInternal.IndexOf(value);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)this.collectionInternal).IndexOf(value);
		}

		public void Insert(int index, AdomdProperty value)
		{
			this.collectionInternal.Insert(index, value);
		}

		void IList.Insert(int index, object value)
		{
			((IList)this.collectionInternal).Insert(index, value);
		}

		public void Remove(AdomdProperty value)
		{
			this.collectionInternal.Remove(value);
		}

		void IList.Remove(object value)
		{
			((IList)this.collectionInternal).Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.collectionInternal.RemoveAt(index);
		}
	}
}
