using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningModelColumnsEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningModelColumnCollectionInternal miningModelColumns;

		public MiningModelColumn Current
		{
			get
			{
				MiningModelColumn result;
				try
				{
					result = this.miningModelColumns[this.currentIndex];
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
				return this.miningModelColumns[this.currentIndex];
			}
		}

		internal MiningModelColumnsEnumerator(MiningModelColumnCollectionInternal miningModelColumns)
		{
			this.miningModelColumns = miningModelColumns;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningModelColumns.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
