using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MeasureCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MeasuresEnumerator enumer;

			public Measure Current
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

			internal Enumerator(MeasureCollection measures)
			{
				this.enumer = new MeasuresEnumerator(measures.CollectionInternal);
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

		private MeasureCollectionInternal measureCollectionInternal;

		public Measure this[int index]
		{
			get
			{
				return this.measureCollectionInternal[index];
			}
		}

		public Measure this[string index]
		{
			get
			{
				return this.measureCollectionInternal[index];
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
				return this.measureCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.measureCollectionInternal.Count;
			}
		}

		internal MeasureCollectionInternal CollectionInternal
		{
			get
			{
				return this.measureCollectionInternal;
			}
		}

		internal MeasureCollection(AdomdConnection connection, CubeDef parentCube)
		{
			this.measureCollectionInternal = new MeasureCollectionInternal(connection, parentCube);
		}

		public Measure Find(string index)
		{
			return this.measureCollectionInternal.Find(index);
		}

		public void CopyTo(Measure[] array, int index)
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

		public MeasureCollection.Enumerator GetEnumerator()
		{
			return new MeasureCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
