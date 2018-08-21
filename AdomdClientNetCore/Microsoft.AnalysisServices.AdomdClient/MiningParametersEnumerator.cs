using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningParametersEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningParameterCollectionInternal miningParameters;

		public MiningParameter Current
		{
			get
			{
				MiningParameter result;
				try
				{
					result = this.miningParameters[this.currentIndex];
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

		internal MiningParametersEnumerator(MiningParameterCollectionInternal miningParameters)
		{
			this.miningParameters = miningParameters;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningParameters.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
