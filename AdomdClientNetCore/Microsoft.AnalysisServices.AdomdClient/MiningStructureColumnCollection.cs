using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningStructureColumnCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningStructureColumnsEnumerator enumer;

			public MiningStructureColumn Current
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

			internal Enumerator(MiningStructureColumnCollection miningStructureColumns)
			{
				this.enumer = new MiningStructureColumnsEnumerator(miningStructureColumns.CollectionInternal);
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

		private MiningStructureColumnCollectionInternal miningStructureColumnCollectionInternal;

		public MiningStructureColumn this[int index]
		{
			get
			{
				return this.miningStructureColumnCollectionInternal[index];
			}
		}

		public MiningStructureColumn this[string index]
		{
			get
			{
				return this.miningStructureColumnCollectionInternal[index];
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
				return this.miningStructureColumnCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningStructureColumnCollectionInternal.Count;
			}
		}

		internal MiningStructureColumnCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningStructureColumnCollectionInternal;
			}
		}

		internal MiningStructureColumnCollection(AdomdConnection connection, MiningStructure parentStructure)
		{
			this.miningStructureColumnCollectionInternal = new MiningStructureColumnCollectionInternal(connection, parentStructure);
		}

		internal MiningStructureColumnCollection(AdomdConnection connection, MiningStructureColumn parentColumn)
		{
			this.miningStructureColumnCollectionInternal = new MiningStructureColumnCollectionInternal(connection, parentColumn);
		}

		public MiningStructureColumn Find(string index)
		{
			return this.miningStructureColumnCollectionInternal.Find(index);
		}

		public void CopyTo(MiningStructureColumn[] array, int index)
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

		public MiningStructureColumnCollection.Enumerator GetEnumerator()
		{
			return new MiningStructureColumnCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
