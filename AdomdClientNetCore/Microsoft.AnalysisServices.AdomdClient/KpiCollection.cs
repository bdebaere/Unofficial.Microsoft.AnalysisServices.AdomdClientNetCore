using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class KpiCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private KpisEnumerator enumer;

			public Kpi Current
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

			internal Enumerator(KpiCollection kpis)
			{
				this.enumer = new KpisEnumerator(kpis.CollectionInternal);
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

		private KpiCollectionInternal kpiCollectionInternal;

		public Kpi this[int index]
		{
			get
			{
				return this.kpiCollectionInternal[index];
			}
		}

		public Kpi this[string index]
		{
			get
			{
				return this.kpiCollectionInternal[index];
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
				return this.kpiCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.kpiCollectionInternal.Count;
			}
		}

		internal KpiCollectionInternal CollectionInternal
		{
			get
			{
				return this.kpiCollectionInternal;
			}
		}

		internal KpiCollection(AdomdConnection connection, CubeDef parentCube)
		{
			this.kpiCollectionInternal = new KpiCollectionInternal(connection, parentCube);
		}

		public Kpi Find(string index)
		{
			return this.kpiCollectionInternal.Find(index);
		}

		public void CopyTo(Kpi[] array, int index)
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

		public KpiCollection.Enumerator GetEnumerator()
		{
			return new KpiCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
