using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class NamedSetsEnumerator : IEnumerator
	{
		private int currentIndex;

		private NamedSetCollectionInternal namedSets;

		public NamedSet Current
		{
			get
			{
				NamedSet result;
				try
				{
					result = this.namedSets[this.currentIndex];
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

		internal NamedSetsEnumerator(NamedSetCollectionInternal namedSets)
		{
			this.namedSets = namedSets;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.namedSets.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
