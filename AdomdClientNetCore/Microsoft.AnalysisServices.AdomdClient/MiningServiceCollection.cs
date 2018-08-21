using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningServiceCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningServicesEnumerator enumer;

			public MiningService Current
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

			internal Enumerator(MiningServiceCollection miningServices)
			{
				this.enumer = new MiningServicesEnumerator(miningServices.CollectionInternal);
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

		private MiningServiceCollectionInternal miningServiceCollectionInternal;

		public MiningService this[int index]
		{
			get
			{
				return this.miningServiceCollectionInternal[index];
			}
		}

		public MiningService this[string index]
		{
			get
			{
				return this.miningServiceCollectionInternal[index];
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
				return this.miningServiceCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningServiceCollectionInternal.Count;
			}
		}

		internal MiningServiceCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningServiceCollectionInternal;
			}
		}

		internal MiningServiceCollection(AdomdConnection connection)
		{
			this.miningServiceCollectionInternal = new MiningServiceCollectionInternal(connection);
		}

		public MiningService Find(string index)
		{
			return this.miningServiceCollectionInternal.Find(index);
		}

		public void CopyTo(MiningService[] array, int index)
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

		public MiningServiceCollection.Enumerator GetEnumerator()
		{
			return new MiningServiceCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
