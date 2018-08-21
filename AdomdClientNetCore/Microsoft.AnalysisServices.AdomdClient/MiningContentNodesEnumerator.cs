using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MiningContentNodesEnumerator : IEnumerator
	{
		private int currentIndex;

		private MiningContentNodeCollectionInternal miningContentNodes;

		public MiningContentNode Current
		{
			get
			{
				MiningContentNode result;
				try
				{
					result = this.miningContentNodes[this.currentIndex];
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

		internal MiningContentNodesEnumerator(MiningContentNodeCollectionInternal miningContentNodes)
		{
			this.miningContentNodes = miningContentNodes;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.miningContentNodes.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
