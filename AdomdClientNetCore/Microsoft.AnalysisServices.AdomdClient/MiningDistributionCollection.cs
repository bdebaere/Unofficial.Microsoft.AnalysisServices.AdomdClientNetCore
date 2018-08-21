using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningDistributionCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningDistributionsEnumerator enumer;

			public MiningDistribution Current
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

			internal Enumerator(MiningDistributionCollection miningDistributions)
			{
				this.enumer = new MiningDistributionsEnumerator(miningDistributions.CollectionInternal);
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

		private MiningDistributionCollectionInternal miningDistributionCollectionInternal;

		public MiningDistribution this[int index]
		{
			get
			{
				return this.miningDistributionCollectionInternal[index];
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
				return this.miningDistributionCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningDistributionCollectionInternal.Count;
			}
		}

		internal MiningDistributionCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningDistributionCollectionInternal;
			}
		}

		internal MiningDistributionCollection(AdomdConnection connection, MiningContentNode parentNode, DataRow[] rows)
		{
			this.miningDistributionCollectionInternal = new MiningDistributionCollectionInternal(connection, parentNode, rows);
		}

		public void CopyTo(MiningDistribution[] array, int index)
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

		public MiningDistributionCollection.Enumerator GetEnumerator()
		{
			return new MiningDistributionCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
