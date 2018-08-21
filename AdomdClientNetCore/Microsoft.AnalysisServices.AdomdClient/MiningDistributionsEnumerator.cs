using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningDistributionsEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningDistributionCollectionInternal miningDistributions;

		public MiningDistribution Current
		{
			get
			{
				MiningDistribution result;
				try
				{
					result = this.miningDistributions[this.currentIndex];
				}
				catch (ArgumentException)
				{
					throw new InvalidOperationException();
				}
				return result;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		internal MiningDistributionsEnumerator(MiningDistributionCollectionInternal miningDistributions)
		{
			this.miningDistributions = miningDistributions;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningDistributions.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
