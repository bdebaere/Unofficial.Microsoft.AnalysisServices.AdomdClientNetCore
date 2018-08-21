using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class LevelsEnumerator : IEnumerator
	{
		private int currentIndex;

		private LevelCollectionInternal levels;

		public Level Current
		{
			get
			{
				Level result;
				try
				{
					result = this.levels[this.currentIndex];
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

		internal LevelsEnumerator(LevelCollectionInternal levels)
		{
			this.levels = levels;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.levels.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
