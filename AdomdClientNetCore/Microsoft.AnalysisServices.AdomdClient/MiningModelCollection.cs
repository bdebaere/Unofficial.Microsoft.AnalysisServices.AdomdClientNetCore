using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningModelCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningModelsEnumerator enumer;

			public MiningModel Current
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

			internal Enumerator(MiningModelCollection miningModels)
			{
				this.enumer = new MiningModelsEnumerator(miningModels.CollectionInternal);
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

		private MiningModelCollectionInternal miningModelCollectionInternal;

		public MiningModel this[int index]
		{
			get
			{
				return this.miningModelCollectionInternal[index];
			}
		}

		public MiningModel this[string index]
		{
			get
			{
				return this.miningModelCollectionInternal[index];
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
				return this.miningModelCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningModelCollectionInternal.Count;
			}
		}

		internal MiningModelCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningModelCollectionInternal;
			}
		}

		internal MiningModelCollection(AdomdConnection connection)
		{
			this.miningModelCollectionInternal = new MiningModelCollectionInternal(connection);
		}

		internal MiningModelCollection(MiningStructure structure)
		{
			this.miningModelCollectionInternal = new MiningModelCollectionInternal(structure);
		}

		public MiningModel Find(string index)
		{
			return this.miningModelCollectionInternal.Find(index);
		}

		public void CopyTo(MiningModel[] array, int index)
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

		public MiningModelCollection.Enumerator GetEnumerator()
		{
			return new MiningModelCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
