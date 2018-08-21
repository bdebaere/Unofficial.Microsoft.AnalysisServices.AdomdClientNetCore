using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningValuesEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningValueCollectionInternal miningValues;

		public MiningValue Current
		{
			get
			{
				MiningValue result;
				try
				{
					result = this.miningValues[this.currentIndex];
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

		internal MiningValuesEnumerator(MiningValueCollectionInternal miningValues)
		{
			this.miningValues = miningValues;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningValues.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
