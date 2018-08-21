using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningValueCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningValuesEnumerator enumer;

			public MiningValue Current
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

			internal Enumerator(MiningValueCollection MiningValues)
			{
				this.enumer = new MiningValuesEnumerator(MiningValues.CollectionInternal);
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

		private MiningValueCollectionInternal miningValueCollectionInternal;

		public MiningValue this[int index]
		{
			get
			{
				return this.miningValueCollectionInternal[index];
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
				return this.miningValueCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningValueCollectionInternal.Count;
			}
		}

		internal MiningValueCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningValueCollectionInternal;
			}
		}

		internal MiningValueCollection()
		{
			this.miningValueCollectionInternal = new MiningValueCollectionInternal();
		}

		internal MiningValueCollection(MiningModelColumn column)
		{
			this.miningValueCollectionInternal = new MiningValueCollectionInternal(column);
		}

		public void CopyTo(MiningValue[] array, int index)
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

		public MiningValueCollection.Enumerator GetEnumerator()
		{
			return new MiningValueCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
