using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningModelColumnCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningModelColumnsEnumerator enumer;

			public MiningModelColumn Current
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

			internal Enumerator(MiningModelColumnCollection miningModelColumns)
			{
				this.enumer = new MiningModelColumnsEnumerator(miningModelColumns.CollectionInternal);
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

		private MiningModelColumnCollectionInternal miningModelColumnCollectionInternal;

		public MiningModelColumn this[int index]
		{
			get
			{
				return this.miningModelColumnCollectionInternal[index];
			}
		}

		public MiningModelColumn this[string index]
		{
			get
			{
				return this.miningModelColumnCollectionInternal[index];
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
				return this.miningModelColumnCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningModelColumnCollectionInternal.Count;
			}
		}

		internal MiningModelColumnCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningModelColumnCollectionInternal;
			}
		}

		internal MiningModelColumnCollection(AdomdConnection connection, MiningModel parentModel)
		{
			this.miningModelColumnCollectionInternal = new MiningModelColumnCollectionInternal(connection, parentModel);
		}

		internal MiningModelColumnCollection(AdomdConnection connection, MiningModelColumn parentColumn)
		{
			this.miningModelColumnCollectionInternal = new MiningModelColumnCollectionInternal(connection, parentColumn);
		}

		public MiningModelColumn Find(string index)
		{
			return this.miningModelColumnCollectionInternal.Find(index);
		}

		public void CopyTo(MiningModelColumn[] array, int index)
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

		public MiningModelColumnCollection.Enumerator GetEnumerator()
		{
			return new MiningModelColumnCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
