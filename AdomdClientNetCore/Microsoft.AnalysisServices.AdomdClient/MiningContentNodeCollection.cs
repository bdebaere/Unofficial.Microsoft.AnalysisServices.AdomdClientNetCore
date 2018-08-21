using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningContentNodeCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private MiningContentNodesEnumerator enumer;

			public MiningContentNode Current
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

			internal Enumerator(MiningContentNodeCollection miningContentNodes)
			{
				this.enumer = new MiningContentNodesEnumerator(miningContentNodes.CollectionInternal);
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

		private MiningContentNodeCollectionInternal miningContentNodeCollectionInternal;

		public MiningContentNode this[int index]
		{
			get
			{
				return this.miningContentNodeCollectionInternal[index];
			}
		}

		public MiningContentNode this[string index]
		{
			get
			{
				return this.miningContentNodeCollectionInternal[index];
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
				return this.miningContentNodeCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.miningContentNodeCollectionInternal.Count;
			}
		}

		internal MiningContentNodeCollectionInternal CollectionInternal
		{
			get
			{
				return this.miningContentNodeCollectionInternal;
			}
		}

		internal MiningContentNodeCollection(AdomdConnection connection, MiningModel parentMiningModel)
		{
			this.miningContentNodeCollectionInternal = new MiningContentNodeCollectionInternal(connection, parentMiningModel);
		}

		internal MiningContentNodeCollection(AdomdConnection connection, MiningModel parentMiningModel, string nodeUniqueName)
		{
			this.miningContentNodeCollectionInternal = new MiningContentNodeCollectionInternal(connection, parentMiningModel, nodeUniqueName);
		}

		internal MiningContentNodeCollection(AdomdConnection connection, MiningContentNode parentNode, MiningNodeTreeOpType operation)
		{
			this.miningContentNodeCollectionInternal = new MiningContentNodeCollectionInternal(connection, parentNode, operation);
		}

		public MiningContentNode Find(string index)
		{
			return this.miningContentNodeCollectionInternal.Find(index);
		}

		public void CopyTo(MiningContentNode[] array, int index)
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

		public MiningContentNodeCollection.Enumerator GetEnumerator()
		{
			return new MiningContentNodeCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
