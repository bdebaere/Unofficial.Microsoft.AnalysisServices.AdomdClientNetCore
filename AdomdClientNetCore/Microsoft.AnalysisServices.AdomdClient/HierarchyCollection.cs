using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class HierarchyCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private HierarchiesEnumerator enumer;

			public Hierarchy Current
			{
				get
				{
					return this.enumer.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(HierarchyCollection hierarchies)
			{
				this.enumer = new HierarchiesEnumerator(hierarchies.CollectionInternal);
			}

			public bool MoveNext()
			{
				return this.enumer.MoveNext();
			}

			public void Reset()
			{
				this.enumer.Reset();
			}
		}

		private HierarchyCollectionInternal hierarchyCollectionInternal;

		public Hierarchy this[int index]
		{
			get
			{
				return this.hierarchyCollectionInternal[index];
			}
		}

		public Hierarchy this[string index]
		{
			get
			{
				return this.hierarchyCollectionInternal[index];
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
				return this.hierarchyCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.hierarchyCollectionInternal.Count;
			}
		}

		internal HierarchyCollectionInternal CollectionInternal
		{
			get
			{
				return this.hierarchyCollectionInternal;
			}
		}

		internal HierarchyCollection(AdomdConnection connection, Set axis, string cubeName)
		{
			this.hierarchyCollectionInternal = new HierarchyCollectionInternal(connection, axis, cubeName);
		}

		internal HierarchyCollection(AdomdConnection connection, Dimension parentDimension, bool isAttribute)
		{
			this.hierarchyCollectionInternal = new HierarchyCollectionInternal(connection, parentDimension, isAttribute);
		}

		public Hierarchy Find(string index)
		{
			return this.hierarchyCollectionInternal.Find(index);
		}

		public void CopyTo(Hierarchy[] array, int index)
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

		public HierarchyCollection.Enumerator GetEnumerator()
		{
			return new HierarchyCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
