using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningStructuresEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningStructureCollectionInternal miningStructures;

		public MiningStructure Current
		{
			get
			{
				MiningStructure result;
				try
				{
					result = this.miningStructures[this.currentIndex];
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

		internal MiningStructuresEnumerator(MiningStructureCollectionInternal miningStructures)
		{
			this.miningStructures = miningStructures;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningStructures.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
