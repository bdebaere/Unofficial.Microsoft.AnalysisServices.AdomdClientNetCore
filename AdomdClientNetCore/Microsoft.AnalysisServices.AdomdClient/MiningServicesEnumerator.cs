using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningServicesEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningServiceCollectionInternal miningServices;

		public MiningService Current
		{
			get
			{
				MiningService result;
				try
				{
					result = this.miningServices[this.currentIndex];
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

		internal MiningServicesEnumerator(MiningServiceCollectionInternal miningServices)
		{
			this.miningServices = miningServices;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningServices.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
