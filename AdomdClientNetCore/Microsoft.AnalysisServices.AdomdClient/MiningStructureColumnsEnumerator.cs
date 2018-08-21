using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningStructureColumnsEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningStructureColumnCollectionInternal miningStructureColumns;

		public MiningStructureColumn Current
		{
			get
			{
				MiningStructureColumn result;
				try
				{
					result = this.miningStructureColumns[this.currentIndex];
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
				return this.miningStructureColumns[this.currentIndex];
			}
		}

		internal MiningStructureColumnsEnumerator(MiningStructureColumnCollectionInternal miningStructureColumns)
		{
			this.miningStructureColumns = miningStructureColumns;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningStructureColumns.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
