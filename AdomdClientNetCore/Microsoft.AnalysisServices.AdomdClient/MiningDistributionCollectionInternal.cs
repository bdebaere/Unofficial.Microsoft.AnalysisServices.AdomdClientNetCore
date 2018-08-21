using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningDistributionCollectionInternal : ICollection, IEnumerable
	{
		private object[] internalObjectCollection;

		private DataRow[] internalCollection;

		private MiningContentNode parentNode;

		private AdomdConnection connection;

		public MiningDistribution this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				MiningDistribution miningDistribution;
				if (this.internalObjectCollection[index] == null)
				{
					miningDistribution = MiningDistributionCollectionInternal.GetMiningDistributionByRow(this.connection, row, this.parentNode);
					this.internalObjectCollection[index] = miningDistribution;
					miningDistribution.ordinal = index;
				}
				else
				{
					miningDistribution = (MiningDistribution)this.internalObjectCollection[index];
				}
				return miningDistribution;
			}
		}

		public int Count
		{
			get
			{
				return this.internalCollection.Length;
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
				return this.internalCollection.SyncRoot;
			}
		}

		internal MiningDistributionCollectionInternal(AdomdConnection connection, MiningContentNode parentNode, DataRow[] rows)
		{
			this.parentNode = parentNode;
			this.internalCollection = rows;
			this.internalObjectCollection = new object[rows.Length];
		}

		public IEnumerator GetEnumerator()
		{
			return new MiningDistributionsEnumerator(this);
		}

		internal static MiningDistribution GetMiningDistributionByRow(AdomdConnection connection, DataRow row, MiningContentNode parentNode)
		{
			return new MiningDistribution(connection, row, parentNode);
		}

		public void CopyTo(Array array, int index)
		{
			this.internalCollection.CopyTo(array, index);
		}
	}
}
