using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class DimensionCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private DimensionsEnumerator enumer;

			public Dimension Current
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

			internal Enumerator(DimensionCollection dimensions)
			{
				this.enumer = new DimensionsEnumerator(dimensions.CollectionInternal);
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

		private DimensionCollectionInternal dimensionCollectionInternal;

		public Dimension this[int index]
		{
			get
			{
				return this.dimensionCollectionInternal[index];
			}
		}

		public Dimension this[string index]
		{
			get
			{
				return this.dimensionCollectionInternal[index];
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
				return this.dimensionCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.dimensionCollectionInternal.Count;
			}
		}

		internal DimensionCollectionInternal CollectionInternal
		{
			get
			{
				return this.dimensionCollectionInternal;
			}
		}

		internal DimensionCollection(AdomdConnection connection, CubeDef parentCube)
		{
			this.dimensionCollectionInternal = new DimensionCollectionInternal(connection, parentCube);
		}

		public Dimension Find(string index)
		{
			return this.dimensionCollectionInternal.Find(index);
		}

		public void CopyTo(Dimension[] array, int index)
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

		public DimensionCollection.Enumerator GetEnumerator()
		{
			return new DimensionCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
