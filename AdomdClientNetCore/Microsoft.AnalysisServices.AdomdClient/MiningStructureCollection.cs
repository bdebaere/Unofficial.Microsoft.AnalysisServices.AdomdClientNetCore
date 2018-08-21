using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningStructureCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningStructuresEnumerator enumer;

			public MiningStructure Current
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

			internal Enumerator(MiningStructureCollection miningStructures)
			{
				this.enumer = new MiningStructuresEnumerator(miningStructures.CollectionInternal);
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

		private MiningStructureCollectionInternal miningStructureCollectionInternal;

		public MiningStructure this[int index]
		{
			get
			{
				return this.miningStructureCollectionInternal[index];
			}
		}

		public MiningStructure this[string index]
		{
			get
			{
				return this.miningStructureCollectionInternal[index];
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
				return this.miningStructureCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningStructureCollectionInternal.Count;
			}
		}

		internal MiningStructureCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningStructureCollectionInternal;
			}
		}

		internal MiningStructureCollection(AdomdConnection connection)
		{
			this.miningStructureCollectionInternal = new MiningStructureCollectionInternal(connection);
		}

		public MiningStructure Find(string index)
		{
			return this.miningStructureCollectionInternal.Find(index);
		}

		public void CopyTo(MiningStructure[] array, int index)
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

		public MiningStructureCollection.Enumerator GetEnumerator()
		{
			return new MiningStructureCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
