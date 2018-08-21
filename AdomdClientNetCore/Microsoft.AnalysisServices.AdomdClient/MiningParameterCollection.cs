using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningParameterCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningParametersEnumerator enumer;

			public MiningParameter Current
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

			internal Enumerator(MiningParameterCollection miningParameters)
			{
				this.enumer = new MiningParametersEnumerator(miningParameters.CollectionInternal);
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

		private MiningParameterCollectionInternal miningParameterCollectionInternal;

		public MiningParameter this[int index]
		{
			get
			{
				return this.miningParameterCollectionInternal[index];
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
				return this.miningParameterCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningParameterCollectionInternal.Count;
			}
		}

		internal MiningParameterCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningParameterCollectionInternal;
			}
		}

		internal MiningParameterCollection(string parameters)
		{
			this.miningParameterCollectionInternal = new MiningParameterCollectionInternal(parameters);
		}

		public MiningParameter Find(string name)
		{
			MiningParameterCollection.Enumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MiningParameter current = enumerator.Current;
				if (string.Compare(current.Name, name, StringComparison.Ordinal) == 0)
				{
					return current;
				}
			}
			return null;
		}

		public void CopyTo(MiningParameter[] array, int index)
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

		public MiningParameterCollection.Enumerator GetEnumerator()
		{
			return new MiningParameterCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
