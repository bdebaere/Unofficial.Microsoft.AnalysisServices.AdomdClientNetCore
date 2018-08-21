using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class LevelPropsEnumerator : IEnumerator
	{
		private int currentIndex;

		private LevelPropertyCollectionInternal levelProperties;

		public LevelProperty Current
		{
			get
			{
				LevelProperty result;
				try
				{
					result = this.levelProperties[this.currentIndex];
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

		internal LevelPropsEnumerator(LevelPropertyCollectionInternal levelProperties)
		{
			this.levelProperties = levelProperties;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.levelProperties.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
