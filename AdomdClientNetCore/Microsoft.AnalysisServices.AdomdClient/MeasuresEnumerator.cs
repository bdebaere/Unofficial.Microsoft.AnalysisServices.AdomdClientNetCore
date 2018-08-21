using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MeasuresEnumerator : IEnumerator
	{
		private int currentIndex;

		private MeasureCollectionInternal measures;

		public Measure Current
		{
			get
			{
				Measure result;
				try
				{
					result = this.measures[this.currentIndex];
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
				return this.measures[this.currentIndex];
			}
		}

		internal MeasuresEnumerator(MeasureCollectionInternal measures)
		{
			this.measures = measures;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.measures.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
