using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class NamedSetCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private NamedSetsEnumerator enumer;

			public NamedSet Current
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

			internal Enumerator(NamedSetCollection namedSets)
			{
				this.enumer = new NamedSetsEnumerator(namedSets.CollectionInternal);
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

		private NamedSetCollectionInternal namedSetCollectionInternal;

		public NamedSet this[int index]
		{
			get
			{
				return this.namedSetCollectionInternal[index];
			}
		}

		public NamedSet this[string index]
		{
			get
			{
				return this.namedSetCollectionInternal[index];
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
				return this.namedSetCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.namedSetCollectionInternal.Count;
			}
		}

		internal NamedSetCollectionInternal CollectionInternal
		{
			get
			{
				return this.namedSetCollectionInternal;
			}
		}

		internal NamedSetCollection(AdomdConnection connection, CubeDef parentCube)
		{
			this.namedSetCollectionInternal = new NamedSetCollectionInternal(connection, parentCube);
		}

		public NamedSet Find(string index)
		{
			return this.namedSetCollectionInternal.Find(index);
		}

		public void CopyTo(NamedSet[] array, int index)
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

		public NamedSetCollection.Enumerator GetEnumerator()
		{
			return new NamedSetCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
