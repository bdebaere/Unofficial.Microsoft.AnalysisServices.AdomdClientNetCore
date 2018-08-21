using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class KpisEnumerator : IEnumerator
	{
		private int currentIndex;

		private KpiCollectionInternal kpis;

		public Kpi Current
		{
			get
			{
				Kpi result;
				try
				{
					result = this.kpis[this.currentIndex];
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

		internal KpisEnumerator(KpiCollectionInternal kpis)
		{
			this.kpis = kpis;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.kpis.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
