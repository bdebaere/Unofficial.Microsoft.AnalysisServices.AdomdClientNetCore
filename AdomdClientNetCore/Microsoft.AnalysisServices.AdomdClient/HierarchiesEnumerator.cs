using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class HierarchiesEnumerator : IEnumerator
	{
		private int currentIndex;

		private HierarchyCollectionInternal hierarchies;

		public Hierarchy Current
		{
			get
			{
				Hierarchy result;
				try
				{
					result = this.hierarchies[this.currentIndex];
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

		internal HierarchiesEnumerator(HierarchyCollectionInternal hierarchies)
		{
			this.hierarchies = hierarchies;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.hierarchies.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
