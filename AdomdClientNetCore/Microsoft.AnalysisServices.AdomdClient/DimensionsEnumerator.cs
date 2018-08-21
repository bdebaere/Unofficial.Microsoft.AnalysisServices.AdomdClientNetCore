using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class DimensionsEnumerator : IEnumerator
	{
		private int currentIndex;

		private DimensionCollectionInternal dimensions;

		public Dimension Current
		{
			get
			{
				Dimension result;
				try
				{
					result = this.dimensions[this.currentIndex];
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
				return this.dimensions[this.currentIndex];
			}
		}

		internal DimensionsEnumerator(DimensionCollectionInternal dimensions)
		{
			this.dimensions = dimensions;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.dimensions.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
