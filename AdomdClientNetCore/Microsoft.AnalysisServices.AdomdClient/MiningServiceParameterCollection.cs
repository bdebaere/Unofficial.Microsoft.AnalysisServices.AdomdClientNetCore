using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningServiceParameterCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningServiceParametersEnumerator enumer;

			public MiningServiceParameter Current
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

			internal Enumerator(MiningServiceParameterCollection miningServiceParameters)
			{
				this.enumer = new MiningServiceParametersEnumerator(miningServiceParameters.CollectionInternal);
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

		private MiningServiceParameterCollectionInternal miningServiceParameterCollectionInternal;

		public MiningServiceParameter this[int index]
		{
			get
			{
				return this.miningServiceParameterCollectionInternal[index];
			}
		}

		public MiningServiceParameter this[string index]
		{
			get
			{
				return this.miningServiceParameterCollectionInternal[index];
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
				return this.miningServiceParameterCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningServiceParameterCollectionInternal.Count;
			}
		}

		internal MiningServiceParameterCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningServiceParameterCollectionInternal;
			}
		}

		internal MiningServiceParameterCollection(AdomdConnection connection, MiningService parentService)
		{
			this.miningServiceParameterCollectionInternal = new MiningServiceParameterCollectionInternal(connection, parentService);
		}

		public MiningServiceParameter Find(string index)
		{
			return this.miningServiceParameterCollectionInternal.Find(index);
		}

		public void CopyTo(MiningServiceParameter[] array, int index)
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

		public MiningServiceParameterCollection.Enumerator GetEnumerator()
		{
			return new MiningServiceParameterCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
