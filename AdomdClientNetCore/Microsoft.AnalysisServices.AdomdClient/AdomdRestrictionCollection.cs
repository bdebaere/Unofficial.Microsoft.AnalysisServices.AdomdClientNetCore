using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdRestrictionCollection : MarshalByRefObject, IList, ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private AdomdRestrictionCollection restrictions;

			public AdomdRestriction Current
			{
				get
				{
					AdomdRestriction result;
					try
					{
						result = this.restrictions[this.currentIndex];
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

			internal Enumerator(AdomdRestrictionCollection restrictions)
			{
				this.restrictions = restrictions;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.restrictions.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private AdomdRestrictionCollectionInternal collectionInternal;

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

		public AdomdRestriction this[int index]
		{
			get
			{
				return (AdomdRestriction)this.collectionInternal[index];
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

		internal AdomdRestrictionCollectionInternal InternalCollection
		{
			get
			{
				return this.collectionInternal;
			}
		}

		public AdomdRestrictionCollection()
		{
			this.collectionInternal = new AdomdRestrictionCollectionInternal(this);
		}

		public AdomdRestriction Find(string propertyName)
		{
			return this.Find(propertyName, null);
		}

		public AdomdRestriction Find(string propertyName, string propertyNamespace)
		{
			XmlaPropertyKey key = new XmlaPropertyKey(propertyName, propertyNamespace);
			int num = this.collectionInternal.IndexOf(key);
			if (num != -1)
			{
				return this[num];
			}
			return null;
		}

		public void CopyTo(AdomdRestriction[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.collectionInternal).CopyTo(array, index);
		}

		public AdomdRestrictionCollection.Enumerator GetEnumerator()
		{
			return new AdomdRestrictionCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public AdomdRestriction Add(AdomdRestriction value)
		{
			return (AdomdRestriction)this.collectionInternal.Add(value);
		}

		public AdomdRestriction Add(string propertyName, object value)
		{
			return (AdomdRestriction)this.collectionInternal.Add(new AdomdRestriction(propertyName, value));
		}

		public AdomdRestriction Add(string propertyName, string propertyNamespace, object value)
		{
			return (AdomdRestriction)this.collectionInternal.Add(new AdomdRestriction(propertyName, propertyNamespace, value));
		}

		int IList.Add(object value)
		{
			return ((IList)this.collectionInternal).Add(value);
		}

		public void Clear()
		{
			this.collectionInternal.Clear();
		}

		public bool Contains(AdomdRestriction value)
		{
			return this.collectionInternal.Contains(value);
		}

		bool IList.Contains(object value)
		{
			return ((IList)this.collectionInternal).Contains(value);
		}

		public int IndexOf(AdomdRestriction value)
		{
			return this.collectionInternal.IndexOf(value);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)this.collectionInternal).IndexOf(value);
		}

		public void Insert(int index, AdomdRestriction value)
		{
			this.collectionInternal.Insert(index, value);
		}

		void IList.Insert(int index, object value)
		{
			((IList)this.collectionInternal).Insert(index, value);
		}

		public void Remove(AdomdRestriction value)
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
