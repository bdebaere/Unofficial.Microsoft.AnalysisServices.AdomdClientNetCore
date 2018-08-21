using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CubeCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private CubesEnumerator enumer;

			public CubeDef Current
			{
				get
				{
					return this.enumer.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(CubeCollection cubes)
			{
				this.enumer = new CubesEnumerator(cubes.CollectionInternal);
			}

			public bool MoveNext()
			{
				return this.enumer.MoveNext();
			}

			public void Reset()
			{
				this.enumer.Reset();
			}
		}

		private CubeCollectionInternal cubeCollectionInternal;

		public CubeDef this[int index]
		{
			get
			{
				return this.cubeCollectionInternal[index];
			}
		}

		public CubeDef this[string index]
		{
			get
			{
				return this.cubeCollectionInternal[index];
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.cubeCollectionInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.cubeCollectionInternal.Count;
			}
		}

		internal CubeCollectionInternal CollectionInternal
		{
			get
			{
				return this.cubeCollectionInternal;
			}
		}

		internal CubeCollection(AdomdConnection connection)
		{
			this.cubeCollectionInternal = new CubeCollectionInternal(connection);
		}

		public CubeDef Find(string index)
		{
			return this.cubeCollectionInternal.Find(index);
		}

		public void CopyTo(CubeDef[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			AdomdUtils.CheckCopyToParameters(array, index, this.Count);
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index + i);
			}
		}

		public CubeCollection.Enumerator GetEnumerator()
		{
			return new CubeCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
