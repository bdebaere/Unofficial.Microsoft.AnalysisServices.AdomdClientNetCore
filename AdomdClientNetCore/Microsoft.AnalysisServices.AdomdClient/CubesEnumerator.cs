using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class CubesEnumerator : IEnumerator
	{
		private int currentIndex;

		private CubeCollectionInternal cubes;

		public CubeDef Current
		{
			get
			{
				CubeDef result;
				try
				{
					result = this.cubes[this.currentIndex];
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

		internal CubesEnumerator(CubeCollectionInternal cubes)
		{
			this.cubes = cubes;
			this.currentIndex = -1;
		}

		public bool MoveNext()
		{
			return ++this.currentIndex < this.cubes.Count;
		}

		public void Reset()
		{
			this.currentIndex = -1;
		}
	}
}
