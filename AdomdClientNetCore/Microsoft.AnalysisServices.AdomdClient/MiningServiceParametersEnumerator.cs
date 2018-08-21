using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningServiceParametersEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningServiceParameterCollectionInternal miningServiceParameters;

		public MiningServiceParameter Current
		{
			get
			{
				MiningServiceParameter result;
				try
				{
					result = this.miningServiceParameters[this.currentIndex];
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
				return this.miningServiceParameters[this.currentIndex];
			}
		}

		internal MiningServiceParametersEnumerator(MiningServiceParameterCollectionInternal miningServiceParameters)
		{
			this.miningServiceParameters = miningServiceParameters;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningServiceParameters.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
