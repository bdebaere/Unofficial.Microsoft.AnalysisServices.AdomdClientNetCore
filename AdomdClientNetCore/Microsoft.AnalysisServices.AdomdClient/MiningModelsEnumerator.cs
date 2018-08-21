using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningModelsEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningModelCollectionInternal miningModels;

		public MiningModel Current
		{
			get
			{
				MiningModel result;
				try
				{
					result = this.miningModels[this.currentIndex];
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

		internal MiningModelsEnumerator(MiningModelCollectionInternal miningModels)
		{
			this.miningModels = miningModels;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningModels.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
